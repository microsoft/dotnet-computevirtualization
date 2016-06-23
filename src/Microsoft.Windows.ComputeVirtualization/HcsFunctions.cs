using System;
using System.Runtime.InteropServices;

[module: DefaultCharSet(CharSet.Unicode)]

namespace Microsoft.Windows.ComputeVirtualization
{
    internal struct HCS_PROCESS_INFORMATION
    {
        public uint ProcessId;
        public uint Reserved;

        public IntPtr StdInput;
        public IntPtr StdOutput;
        public IntPtr StdError;
    }

    internal class HcsFunctions
    {
        [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
        public static extern void HNSCall(string method, string path, string request, [MarshalAs(UnmanagedType.LPWStr)] out string response);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsEnumerateComputeSystems(string query, [MarshalAs(UnmanagedType.LPWStr)] out string computeSystems, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsCreateComputeSystem(string id, string configuration, IntPtr identity, out IntPtr computeSystem, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsOpenComputeSystem(string id, out IntPtr computeSystem, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsCloseComputeSystem(IntPtr computeSystem);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsStartComputeSystem(IntPtr computeSystem, string options, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsShutdownComputeSystem(IntPtr computeSystem, string options, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsTerminateComputeSystem(IntPtr computeSystem, string options, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsGetComputeSystemProperties(IntPtr computeSystem, string propertyQuery, [MarshalAs(UnmanagedType.LPWStr)] out string properties, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsModifyComputeSystem(IntPtr computeSystem, string configuration, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsRegisterComputeSystemCallback(IntPtr computeSystem, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsUnregisterComputeSystemCallback(IntPtr callbackHandle);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsCreateProcess(IntPtr computeSystem, string processParameters, out HCS_PROCESS_INFORMATION processInformation, out IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsOpenProcess(IntPtr computeSystem, uint processId, out IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsCloseProcess(IntPtr process);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsTerminateProcess(IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsGetProcessInfo(IntPtr process, out HCS_PROCESS_INFORMATION processInformation, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsGetProcessProperties(IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string properties, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsModifyProcess(IntPtr process, string settings, [MarshalAs(UnmanagedType.LPWStr)] out string result);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsRegisterProcessCallback(IntPtr process, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

        [DllImport("vmcompute.dll", ExactSpelling = true)]
        public static extern int HcsUnregisterProcessCallback(IntPtr callbackHandle);

        public static bool ProcessHcsCall(int resultCode, string result)
        {
            if (resultCode == HcsException.PENDING)
            {
                return true;
            }
            else if (HcsException.Failed(resultCode))
            {
                throw new HcsException(resultCode, result);
            }
            return false;
        }
    }

    public class HcsException : Exception
    {
        public string ExtendedInfo;

        public const int SUCCESS = 0;
        public const int PENDING = unchecked((int)0xC0370103);
        public const int ALREADY_STOPPED = unchecked((int)0xc0370110);
        public const int E_ABORT = unchecked((int)0x80004004);
        public const int UNEXPECTED_EXIT = unchecked((int)0xC0370106);

        public HcsException(int resultCode, string result) : base("HCS function call returned error.", Marshal.GetExceptionForHR(resultCode))
        {
            HResult = resultCode;         
            ExtendedInfo = result;
        }

        public static bool Failed(int resultCode)
        {
            return resultCode != SUCCESS && resultCode != ALREADY_STOPPED;
        }
    }
}
