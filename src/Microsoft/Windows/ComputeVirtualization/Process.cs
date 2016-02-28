using System;
using System.IO;
using System.ComponentModel;

namespace Microsoft.Windows.ComputeVirtualization
{
    /// <summary>
    /// Represents a running or exited process inside a container.
    /// </summary>
    public class Process : IDisposable
    {
        private string _id;
        private int _pid;
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

        internal Process(string id, int pid, StreamWriter stdin, StreamReader stdout, StreamReader stderr, bool killOnClose)
        {
            _id = id;
            _pid = pid;
            _stdin = stdin;
            _stdout = stdout;
            _stderr = stderr;
            _killOnClose = killOnClose;
        }

        /// <summary>
        /// Resizes the console that was allocated for the process if <see cref="ProcessStartInfo.EmulateConsole"/> was set.
        /// </summary>
        /// <param name="height">The new height, in character cells.</param>
        /// <param name="width">The new width, in character cells.</param>
        public void ResizeConsole(short height, short width)
        {
            HcsFunctions.ResizeConsoleInComputeSystem(_id, _pid, height, width, 0);
        }

        /// <summary>
        /// Waits forever for the process to exit.
        /// </summary>
        public void WaitForExit()
        {
            int exitCode;
            int hr = HcsFunctions.WaitForProcessInComputeSystem(_id, _pid, -1, out exitCode);
            if (hr < 0)
            {
                throw new Win32Exception(hr);
            }
            _exitCode = exitCode;
            _exited = true;
        }

        /// <summary>
        /// Waits for the process to exit.
        /// </summary>
        /// <param name="timeout">The number of milliseconds to wait.</param>
        /// <returns>True if the process exited, false if the wait timed out.</returns>
        public bool WaitForExit(int timeout)
        {
            int exitCode;
            int hr = HcsFunctions.WaitForProcessInComputeSystem(_id, _pid, timeout, out exitCode);
            if (hr < 0)
            {
                if (hr == E_WAIT_TIMEOUT)
                {
                    return false;
                }

                throw new Win32Exception(hr);
            }
            _exitCode = exitCode;
            _exited = true;
            return true;
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
            if (!_killed)
            {
                // Ignore the error code.
                HcsFunctions.TerminateProcessInComputeSystem(_id, _pid);
                _killed = true;
            }
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
        }
    }
}
