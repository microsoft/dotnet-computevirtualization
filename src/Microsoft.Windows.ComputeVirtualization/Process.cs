using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Windows.ComputeVirtualization
{
    /// <summary>
    /// Represents a running or exited process inside a container.
    /// </summary>
    public class Process : IDisposable
    {
        private IHcs _hcs;
        private IntPtr _p;
        private HcsNotificationWatcher _watcher;
        private StreamWriter _stdin;
        private StreamReader _stdout, _stderr;
        private int _exitCode;
        private bool _exited;
        private bool _killOnClose;
        private bool _killed;

        private const int E_WAIT_TIMEOUT = unchecked((int)0x80070102);

        /// <summary>
        /// The stdin stream for the process, if started with <see cref="ProcessStartInfo.RedirectStandardInput"/>.
        /// </summary>
        public StreamWriter StandardInput { get { return _stdin; } }

        /// <summary>
        /// The stdout stream for the process, if started with <see cref="ProcessStartInfo.RedirectStandardOutput"/>.
        /// </summary>
        public StreamReader StandardOutput { get { return _stdout; } }

        /// <summary>
        /// The stderr stream for the process, if started with <see cref="ProcessStartInfo.RedirectStandardError"/>.
        /// </summary>
        public StreamReader StandardError { get { return _stderr; } }

        /// <summary>
        /// The exit code for the process, if it has exited.
        /// </summary>
        public int ExitCode
        {
            get
            {
                if (!_exited)
                {
                    throw new InvalidOperationException();
                }
                return _exitCode;
            }
        }

        internal Process(IntPtr process, StreamWriter stdin, StreamReader stdout, StreamReader stderr, bool killOnClose, IHcs hcs)
        {
            _hcs = hcs;
            _p = process;
            _stdin = stdin;
            _stdout = stdout;
            _stderr = stderr;
            _killOnClose = killOnClose;
            _watcher = new HcsNotificationWatcher(
                _p,
                _hcs.RegisterProcessCallback,
                _hcs.UnregisterProcessCallback,
                new HCS_NOTIFICATIONS[]{
                    HCS_NOTIFICATIONS.HcsNotificationProcessExited
                }
            );
        }

        /// <summary>
        /// Resizes the console that was allocated for the process if <see cref="ProcessStartInfo.EmulateConsole"/> was set.
        /// </summary>
        /// <param name="height">The new height, in character cells.</param>
        /// <param name="width">The new width, in character cells.</param>
        public void ResizeConsole(ushort height, ushort width)
        {
            Schema.ProcessConsoleSize procSize = new Schema.ProcessConsoleSize();
            procSize.Height = height;
            procSize.Width = width;

            Schema.ProcessModifyRequest procModReq = new Schema.ProcessModifyRequest();
            procModReq.Operation = Schema.ProcessModifyOperation.ConsoleSize;
            procModReq.ConsoleSize = procSize;

            _hcs.ModifyProcess(_p, JsonHelper.ToJson(procModReq));
        }

        /// <summary>
        /// Waits for the process to exit.
        /// </summary>
        /// <param name="timeout">The number of milliseconds to wait. Default is infinite</param>
        /// <returns>True if the process exited, false if the wait timed out.</returns>
        public bool WaitForExit(int timeout = Timeout.Infinite)
        {
            return WaitForExitAsync().Wait(timeout);
        }

        public async Task WaitForExitAsync()
        {
            var result = await _watcher.WatchAsync(HCS_NOTIFICATIONS.HcsNotificationProcessExited);
            var processData = JsonHelper.FromJson<Schema.ProcessStatus>(result.Data);
            _exitCode = (int)processData.ExitCode;
            _exited = true;
        }

        /// <summary>
        /// Kills the process.
        /// </summary>
        /// <remarks>
        /// Note that this does not wait for the process to finish terminating; the caller may still wish to
        /// call WaitForExit() if it's necessary to ensure that the process has actually terminated.
        /// </remarks>
        public void Kill()
        {
            KillAsync().Wait();
        }

        public async Task KillAsync()
        {
            if (!_killed && !_exited && _hcs.TerminateProcess(_p))
            {
                await _watcher.WatchAsync(HCS_NOTIFICATIONS.HcsNotificationProcessExited);
            }
            _killed = true;
        }

        /// <summary>
        /// Disposes of resources used for the process. If <see cref="ProcessStartInfo.KillOnClose"/> was set, this also terminates the process.
        /// </summary>
        public void Dispose()
        {
            if (_stdin != null)
            {
                _stdin.Dispose();
            }
            if (_stdout != null)
            {
                _stdout.Dispose();
            }
            if (_stderr != null)
            {
                _stderr.Dispose();
            }
            if (_killOnClose)
            {
                Kill();
            }
            _watcher.Dispose();
            if (_p != IntPtr.Zero)
            {
                _hcs.CloseProcess(_p);
                _p = IntPtr.Zero;
            }
        }
    }
}
