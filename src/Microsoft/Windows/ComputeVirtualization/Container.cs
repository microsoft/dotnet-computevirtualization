using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
        private string _id;
        private bool _killOnClose;
        private bool _killed;

        internal Container(string id, bool terminateOnClose)
        {
            _id = id;
            _killOnClose = terminateOnClose;
        }

        /// <summary>
        /// Starts the container.
        /// </summary>
        public void Start()
        {
            HcsFunctions.StartComputeSystem(_id);
        }

        /// <summary>
        /// Attempts to cleanly shut down the container.
        /// </summary>
        /// <param name="timeout">Timeout to wait for the shutdown to complete, in milliseconds.</param>
        public void Shutdown(int timeout)
        {
            HcsFunctions.ShutdownComputeSystem(_id, timeout);
        }

        /// <summary>
        /// Terminates the container without cleanly shutting down.
        /// </summary>
        public void Kill()
        {
            if (!_killed)
            {
                // Ignore the return value for now.
                HcsFunctions.TerminateComputeSystem(_id);
                _killed = true;
            }
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

            int pid;
            var handles = new IntPtr[3];
            var gch = GCHandle.Alloc(handles);
            IntPtr stdinAddr = new IntPtr(), stdoutAddr = new IntPtr(), stderrAddr = new IntPtr();

            if (startInfo.RedirectStandardInput)
            {
                stdinAddr = Marshal.UnsafeAddrOfPinnedArrayElement(handles, 0);
            }

            if (startInfo.RedirectStandardOutput)
            {
                stdoutAddr = Marshal.UnsafeAddrOfPinnedArrayElement(handles, 1);
            }

            if (startInfo.RedirectStandardError)
            {
                stderrAddr = Marshal.UnsafeAddrOfPinnedArrayElement(handles, 2);
            }

            var parameters = new Schema.ProcessParameters
            {
                ApplicationName = startInfo.ApplicationName,
                CommandLine = startInfo.CommandLine,
                EmulateConsole = startInfo.EmulateConsole,
                RestrictedToken = startInfo.RestrictedToken,
                User = startInfo.User,
                WorkingDirectory = startInfo.WorkingDirectory,
            };

            HcsFunctions.CreateProcessWithStdHandlesInComputeSystem(_id, JsonHelper.ToJson(parameters), out pid, stdinAddr, stdoutAddr, stderrAddr);
            StreamWriter stdinHandle = null;
            StreamReader stdoutHandle = null, stderrHandle = null;
            if (handles[0].ToInt64() != 0)
            {
                stdinHandle = new StreamWriter(new FileStream(new SafeFileHandle(handles[0], true), FileAccess.Write), encoding);
            }

            if (handles[1].ToInt64() != 0)
            {
                stdoutHandle = new StreamReader(new FileStream(new SafeFileHandle(handles[1], true), FileAccess.Read), encoding);
            }

            if (handles[2].ToInt64() != 0)
            {
                stderrHandle = new StreamReader(new FileStream(new SafeFileHandle(handles[2], true), FileAccess.Read), encoding);
            }

            return new Process(_id, pid, stdinHandle, stdoutHandle, stderrHandle, startInfo.KillOnClose);
        }

        /// <summary>
        /// Gets an existing process object from the container.
        /// </summary>
        /// <param name="pid">The process ID.</param>
        /// <returns></returns>
        public Process GetProcess(int pid)
        {
            return new Process(_id, pid, null, null, null, false);
        }

        /// <summary>
        /// Disposes of the container object, killing the container if <see cref="ContainerSettings.KillOnClose"/> was set.
        /// </summary>
        public void Dispose()
        {
            if (_killOnClose)
            {
                Kill();
            }
        }
    }
}
