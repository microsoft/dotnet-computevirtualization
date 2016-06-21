using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Microsoft.Windows.ComputeVirtualization
{
    internal class JsonHelper
    {
        public static string ToJson<T>(T obj)
        {
            var s = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                s.WriteObject(stream, obj);
                return Encoding.ASCII.GetString(stream.ToArray());
            }
        }

        public static T FromJson<T>(string json)
        {
            var s = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(json)))
            {
                return (T)s.ReadObject(stream);
            }
        }
    }
}
