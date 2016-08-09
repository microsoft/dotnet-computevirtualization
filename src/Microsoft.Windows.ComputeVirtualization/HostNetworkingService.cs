using System.ComponentModel;
using Microsoft.Windows.ComputeVirtualization.Schema;

namespace Microsoft.Windows.ComputeVirtualization
{
    public class HostNetworkingService
    {
        /// <summary>
        /// Helper function to call into HNS to update/query a single network.
        /// </summary>
        /// <param name="method">The request method type</param>
        /// <param name="path">The path of the request</param>
        /// <param name="request">The request data</param>
        /// <returns>The queried network.</returns>
        public static HNSNetwork HNSNetworkRequest(string method, string path, string request, IHns hns = null)
        {
            string result;

            (hns ?? HnsFactory.GetHns()).Call(method, "/networks/" + path, request, out result);

            var response = JsonHelper.FromJson<Schema.HNSSingleNetworkResponse>(result);
            if (!response.Success)
            {
                throw new Win32Exception(response.Error);
            }

            return response.Output;
        }

        /// <summary>
        /// Helper function to call into HNS to query the list of available networks.
        /// </summary>
        /// <param name="method">The request method type</param>
        /// <param name="path">The path of the request</param>
        /// <param name="request">The request data</param>
        /// <returns>The filtered list of networks.</returns>
        public static HNSNetwork[] HNSListNetworkRequest(string method, string path, string request, IHns hns = null)
        {
            string result;

            (hns ?? HnsFactory.GetHns()).Call(method, "/networks/" + path, request, out result);

            var response = JsonHelper.FromJson<Schema.HNSNetworkResponse>(result);
            if (!response.Success)
            {
                throw new Win32Exception(response.Error);
            }

            return response.Output;
        }

        /// <summary>
        /// Helper function to call into HNS to modify/query a network endpoint.
        /// </summary>
        /// <param name="method">The request method type</param>
        /// <param name="path">The path of the request</param>
        /// <param name="request">The request data</param>
        /// <returns>The queried endpoint.</returns>
        public static HNSEndpoint HNSEndpointRequest(string method, string path, string request, IHns hns = null)
        {
            string result;

            (hns ?? HnsFactory.GetHns()).Call(method, "/endpoints/" + path, request, out result);

            var response = JsonHelper.FromJson<Schema.HNSEndpointResponse>(result);
            if (!response.Success)
            {
                throw new Win32Exception(response.Error);
            }

            return response.Output;
        }
    }
}