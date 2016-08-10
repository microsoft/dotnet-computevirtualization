using System;

namespace Microsoft.Windows.ComputeVirtualization
{
    public interface IHcs
    {
        bool EnumerateComputeSystems(string query, out string computeSystems);

        bool CreateComputeSystem(string id, string configuration, IntPtr identity, out IntPtr computeSystem);

        bool OpenComputeSystem(string id, out IntPtr computeSystem);

        bool CloseComputeSystem(IntPtr computeSystem);

        bool StartComputeSystem(IntPtr computeSystem, string options);

        bool ShutdownComputeSystem(IntPtr computeSystem, string options);

        bool TerminateComputeSystem(IntPtr computeSystem, string options);

        bool GetComputeSystemProperties(IntPtr computeSystem, string propertyQuery, out string properties);

        bool ModifyComputeSystem(IntPtr computeSystem, string configuration);

        void RegisterComputeSystemCallback(IntPtr computeSystem, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

        void UnregisterComputeSystemCallback(IntPtr callbackHandle);

        bool CreateProcess(IntPtr computeSystem, string processParameters, out HCS_PROCESS_INFORMATION processInformation, out IntPtr process);

        bool OpenProcess(IntPtr computeSystem, uint processId, out IntPtr process);

        bool CloseProcess(IntPtr process);

        bool TerminateProcess(IntPtr process);

        bool GetProcessInfo(IntPtr process, out HCS_PROCESS_INFORMATION processInformation);

        bool GetProcessProperties(IntPtr process, out string properties);

        bool ModifyProcess(IntPtr process, string settings);

        void RegisterProcessCallback(IntPtr process, NotificationCallback callback, IntPtr context, out IntPtr callbackHandle);

        void UnregisterProcessCallback(IntPtr callbackHandle);
    }
}