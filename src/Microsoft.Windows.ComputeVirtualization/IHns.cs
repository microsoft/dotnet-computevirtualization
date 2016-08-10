namespace Microsoft.Windows.ComputeVirtualization
{
    public interface IHns
    {
        void Call(string method, string path, string request, out string response);
    }
}