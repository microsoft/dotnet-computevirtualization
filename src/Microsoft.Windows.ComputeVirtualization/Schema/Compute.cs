using System;
using System.Runtime.Serialization;

namespace Microsoft.Windows.ComputeVirtualization.Schema
{    
    public enum SystemType
    {
        Container,
        VirtualMachine = 1
    }


    public enum NatPortProtocol
    {
        TCP,
        UDP = 1
    }


    public enum NetworkMode
    {
        NAT,
        Transparent = 1,
        L2Bridge = 2,
        L2Tunnel = 3,
        Private = 4,
    }


    public enum PolicyType
    {
        NAT,
        QOS = 1,
        VLAN = 2,
        VSID = 3

    }


    [DataContract]
    struct Layer
    {
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public string Path
        {
            get;
            set;
        }
    }


    [DataContract]
    struct MappedDirectory
    {
        [DataMember(IsRequired = true)]
        public string HostPath
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public string ContainerPath
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool ReadOnly
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<uint> IOPSMaximum
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<uint> BandwidthMaximum
        {
            get;
            set;
        }
    }


    [DataContract]
    struct DeviceBase
    {
        [DataMember]
        public string DeviceType
        {
            get;
            set;
        }
    }


    [DataContract]
    struct UtilityVmNetworkSettings
    {
        [DataMember(EmitDefaultValue = false)]
        public string SwitchName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string PortName
        {
            get;
            set;
        }
    }


    [DataContract]
    struct UtilityVmSettings
    {
        [DataMember(EmitDefaultValue = false)]
        public string ImagePath
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Guid RuntimeId
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool EnableConsole
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public UtilityVmNetworkSettings NetworkSettings
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Com1PipeName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Com2PipeName
        {
            get;
            set;
        }
    }


    [DataContract]
    struct SettingsBase
    {
        [DataMember(Name = "SystemType")]
        private string _SystemType;
        public SystemType SystemType
        {
            get
            {
                if (this._SystemType == null)
                {
                    return default(SystemType);
                }

                return (SystemType)Enum.Parse(typeof(SystemType), this._SystemType, true);
            }
            set
            {
                this._SystemType = value.ToString();
            }
        }
    }


    [DataContract]
    struct ContainerSettings
    {
        [DataMember(Name = "SystemType")]
        private string _SystemType;
        public SystemType SystemType
        {
            get
            {
                if (this._SystemType == null)
                {
                    return default(SystemType);
                }

                return (SystemType)Enum.Parse(typeof(SystemType), this._SystemType, true);
            }
            set
            {
                this._SystemType = value.ToString();
            }
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Owner
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool IsDummy
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool HvPartition
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public UtilityVmSettings HvRuntime
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string HostName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool TerminateOnLastHandleClosed
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<long> MemoryMaximumInMB
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<long> ProcessorMaximum
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<long> ProcessorWeight
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool IgnoreFlushesDuringBoot
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string SandboxPath
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string VolumePath
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string LayerFolderPath
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Layer[] Layers
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public MappedDirectory[] MappedDirectories
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<uint> StorageIOPSMaximum
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<uint> StorageBandwidthMaximum
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Nullable<uint> StorageSandboxSize
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string[] Devices
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public NetworkEndpoint[] NetworkEndpoints
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public Guid[] EndpointList
        {
            get;
            set;
        }
    }


    [DataContract]
    struct NatPortBinding
    {
        [DataMember(Name = "Protocol")]
        private string _Protocol;
        public NatPortProtocol Protocol
        {
            get
            {
                if (this._Protocol == null)
                {
                    return default(NatPortProtocol);
                }

                return (NatPortProtocol)Enum.Parse(typeof(NatPortProtocol), this._Protocol, true);
            }
            set
            {
                this._Protocol = value.ToString();
            }
        }

        [DataMember]
        public ushort InternalPort
        {
            get;
            set;
        }

        [DataMember]
        public ushort ExternalPort
        {
            get;
            set;
        }
    }


    [DataContract]
    struct NatSettings
    {
        [DataMember(EmitDefaultValue = false)]
        public string Name
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public NatPortBinding[] PortBindings
        {
            get;
            set;
        }
    }


    [DataContract]
    struct NetworkConnection
    {
        [DataMember]
        public string NetworkName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string Ip4Address
        {
            get;
            set;
        }

        [DataMember]
        public bool EnableNat
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public NatSettings Nat
        {
            get;
            set;
        }

        [DataMember]
        public Nullable<uint> MaximumOutgoingBandwidthInBytes
        {
            get;
            set;
        }
    }


    [DataContract]
    struct NetworkSettings
    {
        [DataMember(EmitDefaultValue = false)]
        public string MacAddress
        {
            get;
            set;
        }
    }


    [DataContract]
    struct NetworkDevice
    {
        [DataMember]
        public string DeviceType
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public NetworkConnection Connection
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public NetworkSettings Settings
        {
            get;
            set;
        }
    }


    [DataContract]
    public struct Subnet
    {
        [DataMember(EmitDefaultValue = false)]
        public string GatewayAddress
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string AddressPrefix
        {
            get;
            set;
        }
    }


    [DataContract]
    public struct MacPool
    {
        [DataMember(EmitDefaultValue = false)]
        public string StartMacAddress
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string EndMacAddress
        {
            get;
            set;
        }
    }


    [DataContract]
    public struct Policy
    {
        [DataMember(EmitDefaultValue = false, Name = "Type")]
        private string _Type;
        public PolicyType Type
        {
            get
            {
                if (this._Type == null)
                {
                    return default(PolicyType);
                }

                return (PolicyType)Enum.Parse(typeof(PolicyType), this._Type, true);
            }
            set
            {
                this._Type = value.ToString();
            }
        }
    }


    [DataContract]
    public struct NatPolicyData
    {
        [DataMember(EmitDefaultValue = false, Name = "Type")]
        private string _Type;
        public PolicyType Type
        {
            get
            {
                if (this._Type == null)
                {
                    return default(PolicyType);
                }

                return (PolicyType)Enum.Parse(typeof(PolicyType), this._Type, true);
            }
            set
            {
                this._Type = value.ToString();
            }
        }

        [DataMember(EmitDefaultValue = false, Name = "Protocol")]
        private string _Protocol;
        public NatPortProtocol Protocol
        {
            get
            {
                if (this._Protocol == null)
                {
                    return default(NatPortProtocol);
                }

                return (NatPortProtocol)Enum.Parse(typeof(NatPortProtocol), this._Protocol, true);
            }
            set
            {
                this._Protocol = value.ToString();
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public ushort InternalPort
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public ushort ExternalPort
        {
            get;
            set;
        }
    }


    [DataContract]
    public struct QosPolicyData
    {
        [DataMember(EmitDefaultValue = false, Name = "Type")]
        private string _Type;
        public PolicyType Type
        {
            get
            {
                if (this._Type == null)
                {
                    return default(PolicyType);
                }

                return (PolicyType)Enum.Parse(typeof(PolicyType), this._Type, true);
            }
            set
            {
                this._Type = value.ToString();
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public uint MaximumOutgoingBandwidthInBytes
        {
            get;
            set;
        }
    }

    [DataContract]
    public struct VlanPolicyData
    {
        [DataMember(EmitDefaultValue = false, Name = "Type")]
        private string _Type;
        public PolicyType Type
        {
            get
            {
                if (this._Type == null)
                {
                    return default(PolicyType);
                }

                return (PolicyType)Enum.Parse(typeof(PolicyType), this._Type, true);
            }
            set
            {
                this._Type = value.ToString();
            }
        }

        [DataMember]
        public ulong VLAN
        {
            get;
            set;
        }
    }
    
    [DataContract]
    public struct VsidPolicyData
    {
        [DataMember(EmitDefaultValue = false, Name = "Type")]
        private string _Type;
        public PolicyType Type
        {
            get
            {
                if (this._Type == null)
                {
                    return default(PolicyType);
                }

                return (PolicyType)Enum.Parse(typeof(PolicyType), this._Type, true);
            }
            set
            {
                this._Type = value.ToString();
            }
        }

        [DataMember]
        public ulong VSID
        {
            get;
            set;
        }
    }

    [DataContract]
    public struct NetworkEndpoint
    {
        [DataMember(IsRequired = true)]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string EndpointName
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public bool DynamicIPAddress
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public string StaticIPAddress
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public Guid NetworkId
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
    }
}
