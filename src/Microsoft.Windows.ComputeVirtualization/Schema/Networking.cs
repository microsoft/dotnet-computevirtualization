using System;
using System.Runtime.Serialization;

namespace Microsoft.Windows.ComputeVirtualization.Schema
{
    
    [DataContract]
    public struct HNSNetwork
    {
        [DataMember(EmitDefaultValue = false)]
        public string ID
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Name
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "Type")]
        private string _Type;
        public NetworkMode Type
        {
            get
            {
                if (this._Type == null)
                {
                    return default(NetworkMode);
                }

                return (NetworkMode)Enum.Parse(typeof(NetworkMode), this._Type, true);
            }
            set
            {
                this._Type = value.ToString();
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public string NetworkAdapterName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string SourceMac
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Policy[] Policies
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public MacPool[] MacPools
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Subnet[] Subnets
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string DNSSuffix
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string DNSServerList
        {
            get;
            set;
        }
    }

    
    [DataContract]
    public struct HNSEndpoint
    {
        [DataMember(EmitDefaultValue = false)]
        public Guid ID
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Name
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Guid VirtualNetwork
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string VirtualNetworkName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string[] Policies
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string MacAddress
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string IPAddress
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string DNSServerList
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string DNSSuffix
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string GatewayAddress
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public ushort PrefixLength
        {
            get;
            set;
        }
    }

    
    [DataContract]
    struct HNSNetworkResponse
    {
        [DataMember(IsRequired = true)]
        public bool Success
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Error
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public HNSNetwork[] Output
        {
            get;
            set;
        }
    }

    
    [DataContract]
    struct HNSSingleNetworkResponse
    {
        [DataMember(IsRequired = true)]
        public bool Success
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Error
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public HNSNetwork Output
        {
            get;
            set;
        }
    }

    
    [DataContract]
    struct HNSEndpointResponse
    {
        [DataMember(IsRequired = true)]
        public bool Success
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Error
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public HNSEndpoint Output
        {
            get;
            set;
        }
    }
}
