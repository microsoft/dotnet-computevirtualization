using System.Runtime.InteropServices;

namespace Microsoft.Windows.ComputeVirtualization
{
    public class HnsFactory
    {
        public static IHns GetHns()
        {
            return new Hns();
        }

        private class Hns : IHns
        {
            void IHns.Call(string method, string path, string request, out string response)
            {
                HNSCall(method, path, request, out response);
            }

            [DllImport("vmcompute.dll", PreserveSig = false, ExactSpelling = true)]
            private static extern void HNSCall(string method, string path, string request, [MarshalAs(UnmanagedType.LPWStr)] out string response);
        }
    }
}

