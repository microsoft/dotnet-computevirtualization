using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Windows.ComputeVirtualization
{
    public class NotificationResult
    {
        public int Status;
        public string Data;
    }

    public enum HCS_NOTIFICATIONS : uint
    {
        HcsNotificationInvalid = 0x00000000,

        /// Notifications for HCS_SYSTEM handles
        HcsNotificationSystemExited = 0x00000001,
        HcsNotificationSystemCreateCompleted = 0x00000002,
        HcsNotificationSystemStartCompleted = 0x00000003,

        /// Notifications for HCS_PROCESS handles
        HcsNotificationProcessExited = 0x00010000,

        /// Common notifications
        HcsNotificationServiceDisconnect = 0x01000000,

        /// The upper 4 bits are reserved for flags. See HCS_NOTIFICATION_FLAGS
        HcsNotificationFlagsReserved = 0xF0000000
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NotificationCallback(uint notificationType, IntPtr context, int notificationStatus, [MarshalAs(UnmanagedType.LPWStr)] string notificationData);

    public delegate void RegisterHcsNotificationCallback(IntPtr handle, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

    public delegate void UnregisterHcsNotificationCallback(IntPtr handle);

    public class HcsNotificationWatcher : IDisposable
    {
        private IntPtr _h;
        private NotificationCallback _callbackFunc;
        private UnregisterHcsNotificationCallback _unreg;
        private readonly Dictionary<HCS_NOTIFICATIONS, TaskCompletionSource<NotificationResult>> _n = new Dictionary<HCS_NOTIFICATIONS, TaskCompletionSource<NotificationResult>>();

        public HcsNotificationWatcher(IntPtr handle, RegisterHcsNotificationCallback register, UnregisterHcsNotificationCallback unregister, HCS_NOTIFICATIONS[] notificationList)
        {
            _unreg = unregister;

            foreach (var notificationType in notificationList)
            {
                var entry = new TaskCompletionSource<NotificationResult>();
                _n.Add(notificationType, entry);
            }

            _callbackFunc = (uint nType, IntPtr ctx, int nStatus, string nData) =>
            {
                var key = (HCS_NOTIFICATIONS)nType;
                if (key == HCS_NOTIFICATIONS.HcsNotificationServiceDisconnect)
                {
                    // Service disconnect should fail all outstanding notifications.
                    foreach (var entry in _n.Values)
                    {
                        entry.TrySetException(new HcsException(HcsException.E_ABORT, null));
                    }
                    return;
                }

                if (key == HCS_NOTIFICATIONS.HcsNotificationSystemExited && _n.ContainsKey(HCS_NOTIFICATIONS.HcsNotificationSystemStartCompleted))
                {
                    // Special handling for exit received while waiting for start.
                    _n[HCS_NOTIFICATIONS.HcsNotificationSystemStartCompleted].TrySetException(new HcsException(HcsException.UNEXPECTED_EXIT, null));
                }

                if (_n.ContainsKey(key))
                {
                    var result = new NotificationResult()
                    {
                        Status = nStatus,
                        Data = nData
                    };

                    if (HcsException.Failed(result.Status))
                    {
                        _n[key].SetException(new HcsException(result.Status, result.Data));
                    }
                    else
                    {
                        _n[key].SetResult(result);
                    }
                }
            };

            register(handle, _callbackFunc, IntPtr.Zero, out _h);
        }

        public Task<NotificationResult> WatchAsync(HCS_NOTIFICATIONS notificationType)
        {
            return _n[notificationType].Task;
        }

        public bool Wait(HCS_NOTIFICATIONS notificationType, int timeout = Timeout.Infinite)
        {
            return WatchAsync(notificationType).Wait(timeout);
        }

        public void Dispose()
        {
            if (_h != IntPtr.Zero)
            {
                _unreg(_h);
                _h = IntPtr.Zero;
            }
        }
    }
}