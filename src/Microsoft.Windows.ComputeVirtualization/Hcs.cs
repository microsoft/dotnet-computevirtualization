using System;
using System.Runtime.InteropServices;

[module: DefaultCharSet(CharSet.Unicode)]
namespace Microsoft.Windows.ComputeVirtualization
{
    public class HcsFactory
    {
        public static IHcs GetHcs()
        {
            return new Hcs();
        }

        private class Hcs : IHcs
        {
            bool IHcs.EnumerateComputeSystems(string query, out string computeSystems)
            {
                string result;
                return ProcessHcsCall(HcsEnumerateComputeSystems(query, out computeSystems, out result), result);
            }

            bool IHcs.CreateComputeSystem(string id, string configuration, IntPtr identity, out IntPtr computeSystem)
            {
                string result;
                return ProcessHcsCall(HcsCreateComputeSystem(id, configuration, identity, out computeSystem, out result), result);
            }

            bool IHcs.OpenComputeSystem(string id, out IntPtr computeSystem)
            {
                string result;
                return ProcessHcsCall(HcsOpenComputeSystem(id, out computeSystem, out result), result);
            }

            bool IHcs.CloseComputeSystem(IntPtr computeSystem)
            {
                return ProcessHcsCall(HcsCloseComputeSystem(computeSystem), null);
            }

            bool IHcs.StartComputeSystem(IntPtr computeSystem, string options)
            {
                string result;
                return ProcessHcsCall(HcsStartComputeSystem(computeSystem, options, out result), result);
            }

            bool IHcs.ShutdownComputeSystem(IntPtr computeSystem, string options)
            {
                string result;
                return ProcessHcsCall(HcsShutdownComputeSystem(computeSystem, options, out result), result);
            }

            bool IHcs.TerminateComputeSystem(IntPtr computeSystem, string options)
            {
                string result;
                return ProcessHcsCall(HcsTerminateComputeSystem(computeSystem, options, out result), result);
            }

            bool IHcs.GetComputeSystemProperties(IntPtr computeSystem, string propertyQuery, out string properties)
            {
                string result;
                return ProcessHcsCall(HcsGetComputeSystemProperties(computeSystem, propertyQuery, out properties, out result), result);
            }

            bool IHcs.ModifyComputeSystem(IntPtr computeSystem, string configuration)
            {
                string result;
                return ProcessHcsCall(HcsModifyComputeSystem(computeSystem, configuration, out result), result);
            }

            void IHcs.RegisterComputeSystemCallback(IntPtr computeSystem, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle)
            {
                ProcessHcsCall(HcsRegisterComputeSystemCallback(computeSystem, callback, context, out callbackHandle), null);
            }

            void IHcs.UnregisterComputeSystemCallback(IntPtr callbackHandle)
            {
                ProcessHcsCall(HcsUnregisterComputeSystemCallback(callbackHandle), null);
            }

            bool IHcs.CreateProcess(IntPtr computeSystem, string processParameters, out HCS_PROCESS_INFORMATION processInformation, out IntPtr process)
            {
                string result;
                return ProcessHcsCall(HcsCreateProcess(computeSystem, processParameters, out processInformation, out process, out result), result);
            }

            bool IHcs.OpenProcess(IntPtr computeSystem, uint processId, out IntPtr process)
            {
                string result;
                return ProcessHcsCall(HcsOpenProcess(computeSystem, processId, out process, out result), result);
            }

            bool IHcs.CloseProcess(IntPtr process)
            {
                return ProcessHcsCall(HcsCloseProcess(process), null);
            }

            bool IHcs.TerminateProcess(IntPtr process)
            {
                string result;
                return ProcessHcsCall(HcsTerminateProcess(process, out result), result);
            }

            bool IHcs.GetProcessInfo(IntPtr process, out HCS_PROCESS_INFORMATION processInformation)
            {
                string result;
                return ProcessHcsCall(HcsGetProcessInfo(process, out processInformation, out result), result);
            }

            bool IHcs.GetProcessProperties(IntPtr process, out string properties)
            {
                string result;
                return ProcessHcsCall(HcsGetProcessProperties(process, out properties, out result), result);
            }

            bool IHcs.ModifyProcess(IntPtr process, string settings)
            {
                string result;
                return ProcessHcsCall(HcsModifyProcess(process, settings, out result), result);
            }

            void IHcs.RegisterProcessCallback(IntPtr process, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle)
            {
                ProcessHcsCall(HcsRegisterProcessCallback(process, callback, context, out callbackHandle), null);
            }

            void IHcs.UnregisterProcessCallback(IntPtr callbackHandle)
            {
                ProcessHcsCall(HcsUnregisterProcessCallback(callbackHandle), null);
            }

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsEnumerateComputeSystems(string query, [MarshalAs(UnmanagedType.LPWStr)] out string computeSystems, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsCreateComputeSystem(string id, string configuration, IntPtr identity, out IntPtr computeSystem, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsOpenComputeSystem(string id, out IntPtr computeSystem, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsCloseComputeSystem(IntPtr computeSystem);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsStartComputeSystem(IntPtr computeSystem, string options, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsShutdownComputeSystem(IntPtr computeSystem, string options, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsTerminateComputeSystem(IntPtr computeSystem, string options, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsGetComputeSystemProperties(IntPtr computeSystem, string propertyQuery, [MarshalAs(UnmanagedType.LPWStr)] out string properties, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsModifyComputeSystem(IntPtr computeSystem, string configuration, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsRegisterComputeSystemCallback(IntPtr computeSystem, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsUnregisterComputeSystemCallback(IntPtr callbackHandle);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsCreateProcess(IntPtr computeSystem, string processParameters, out HCS_PROCESS_INFORMATION processInformation, out IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsOpenProcess(IntPtr computeSystem, uint processId, out IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsCloseProcess(IntPtr process);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsTerminateProcess(IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsGetProcessInfo(IntPtr process, out HCS_PROCESS_INFORMATION processInformation, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsGetProcessProperties(IntPtr process, [MarshalAs(UnmanagedType.LPWStr)] out string properties, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsModifyProcess(IntPtr process, string settings, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsRegisterProcessCallback(IntPtr process, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

            [DllImport("vmcompute.dll", ExactSpelling = true)]
            private static extern int HcsUnregisterProcessCallback(IntPtr callbackHandle);

            private static bool ProcessHcsCall(int resultCode, string result)
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

    public struct HCS_PROCESS_INFORMATION
    {
        public uint ProcessId;
        public uint Reserved;

        public IntPtr StdInput;
        public IntPtr StdOutput;
        public IntPtr StdError;
    }
}
