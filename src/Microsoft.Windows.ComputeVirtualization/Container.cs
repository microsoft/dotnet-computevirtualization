using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Windows.ComputeVirtualization
{
    public class ProcessStartInfo
    {
        public string ApplicationName;
        public string CommandLine;
        public string User;
        public string WorkingDirectory;
        public bool RedirectStandardInput;
        public bool RedirectStandardOutput;
        public bool RedirectStandardError;
        public bool EmulateConsole;
        public bool RestrictedToken;
        public bool KillOnClose;
    }

    /// <summary>
    /// Represents an instantiated Windows container on the current machine.
    /// </summary>
    public class Container : IDisposable
    {
        private IHcs _hcs;
        private IntPtr _cs;
        private bool _killOnClose;
        private bool _dead;
        private HcsNotificationWatcher _watcher;

        private Container(string id, IntPtr computeSystem, bool terminateOnClose, HcsNotificationWatcher watcher, IHcs hcs)
        {
            _hcs = hcs;
            _killOnClose = terminateOnClose;
            _cs = computeSystem;
            _watcher = watcher;
        }

        internal static Container Initialize(string id, IntPtr computeSystem, bool terminateOnClose, bool createNewContainer, IHcs hcs = null)
        {
            var h = hcs ?? HcsFactory.GetHcs();
            var watcher = new HcsNotificationWatcher(
                computeSystem,
                h.RegisterComputeSystemCallback,
                h.UnregisterComputeSystemCallback,
                new HCS_NOTIFICATIONS[]{
                    HCS_NOTIFICATIONS.HcsNotificationSystemExited,
                    HCS_NOTIFICATIONS.HcsNotificationSystemCreateCompleted,
                    HCS_NOTIFICATIONS.HcsNotificationSystemStartCompleted
                    }
                );
            var container = new Container(
                id,
                computeSystem,
                terminateOnClose,
                watcher,
                h);
            if (createNewContainer)
            {
                watcher.Wait(HCS_NOTIFICATIONS.HcsNotificationSystemCreateCompleted);
            }

            return container;
        }

        /// <summary>
        /// Starts the container.
        /// </summary>
        public void Start()
        {
            StartAsync().Wait();
        }

        public async Task StartAsync()
        {
            if (_hcs.StartComputeSystem(_cs, null))
            {
                await _watcher.WatchAsync(HCS_NOTIFICATIONS.HcsNotificationSystemStartCompleted);
            }
        }

        /// <summary>
        /// Attempts to cleanly shut down the container.
        /// </summary>
        /// <param name="timeout">Timeout to wait for the shutdown to complete, in milliseconds.</param>
        public void Shutdown(int timeout = Timeout.Infinite)
        {
            ShutdownAsync().Wait(timeout);
        }

        public async Task ShutdownAsync()
        {
            if (!_dead && _hcs.ShutdownComputeSystem(_cs, null))
            {
                await _watcher.WatchAsync(HCS_NOTIFICATIONS.HcsNotificationSystemExited);
            }
            _dead = true;
        }

        /// <summary>
        /// Terminates the container without cleanly shutting down.
        /// </summary>
        public void Kill()
        {
            KillAsync().Wait();
        }

        public async Task KillAsync()
        {
            if (!_dead && _hcs.TerminateComputeSystem(_cs, null))
            {
                await _watcher.WatchAsync(HCS_NOTIFICATIONS.HcsNotificationSystemExited);
            }
            _dead = true;
        }

        /// <summary>
        /// Starts a process in the container.
        /// </summary>
        /// <param name="startInfo">Process start parameters.</param>
        /// <returns></returns>
        public Process CreateProcess(ProcessStartInfo startInfo)
        {
            // Use UTF-8 encoding on the streams by default (without a BOM). The user can override this with whatever they
            // want by using .BaseStream, but this is a good default.
            var encoding = new UTF8Encoding(false);

            var parameters = new Schema.ProcessParameters
            {
                ApplicationName = startInfo.ApplicationName,
                CommandLine = startInfo.CommandLine,
                EmulateConsole = startInfo.EmulateConsole,
                RestrictedToken = startInfo.RestrictedToken,
                User = startInfo.User,
                WorkingDirectory = startInfo.WorkingDirectory,
                CreateStdInPipe = startInfo.RedirectStandardInput,
                CreateStdOutPipe = startInfo.RedirectStandardOutput,
                CreateStdErrPipe = startInfo.RedirectStandardError,
            };

            IntPtr process;
            HCS_PROCESS_INFORMATION procInfo;
            _hcs.CreateProcess(_cs, JsonHelper.ToJson(parameters), out procInfo, out process);

            StreamWriter stdinHandle = null;
            StreamReader stdoutHandle = null, stderrHandle = null;
            if (startInfo.RedirectStandardInput)
            {
                stdinHandle = new StreamWriter(new FileStream(new SafeFileHandle(procInfo.StdInput, true), FileAccess.Write), encoding);
            }
            if (startInfo.RedirectStandardOutput)
            {
                stdoutHandle = new StreamReader(new FileStream(new SafeFileHandle(procInfo.StdOutput, true), FileAccess.Read), encoding);
            }
            if (startInfo.RedirectStandardError)
            {
                stderrHandle = new StreamReader(new FileStream(new SafeFileHandle(procInfo.StdError, true), FileAccess.Read), encoding);
            }

            return new Process(process, stdinHandle, stdoutHandle, stderrHandle, startInfo.KillOnClose, _hcs);
        }

        /// <summary>
        /// Gets an existing process object from the container.
        /// </summary>
        /// <param name="pid">The process ID.</param>
        /// <returns></returns>
        public Process GetProcess(int pid)
        {
            IntPtr process;
            _hcs.OpenProcess(_cs, (uint)pid, out process);
            return new Process(process, null, null, null, false, _hcs);
        }

        /// <summary>
        /// Disposes of the container object.
        /// </summary>
        public void Dispose()
        {
            if (_killOnClose && !_dead)
            {
                Kill();
            }

            _watcher.Dispose();

            if (_cs != IntPtr.Zero)
            {
                _hcs.CloseComputeSystem(_cs);
                _cs = IntPtr.Zero;
            }
        }
    }
}
