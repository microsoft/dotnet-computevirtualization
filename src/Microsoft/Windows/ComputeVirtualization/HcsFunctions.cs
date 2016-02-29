using System;
using System.Runtime.InteropServices;

[module: DefaultCharSet(CharSet.Unicode)]

namespace Microsoft.Windows.ComputeVirtualization
{
    internal class HcsFunctions
    {
        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void CreateComputeSystem(string id, string config);

        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void StartComputeSystem(string id);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int TerminateComputeSystem(string id);

        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void ShutdownComputeSystem(string id, int timeout);

        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern bool ComputeSystemExists(string id);

        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void CreateProcessWithStdHandlesInComputeSystem(string id, string parameters, out int pid, IntPtr stdin, IntPtr stdout, IntPtr stderr);

        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void ResizeConsoleInComputeSystem(string id, int pid, short height, short width, int reserved);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int WaitForProcessInComputeSystem(string id, int pid, int timeout, out int exitCode);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int TerminateProcessInComputeSystem(string id, int pid);

        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void HNSCall(string method, string path, string request, [MarshalAs(UnmanagedType.LPWStr)] out string response);
    }
}
