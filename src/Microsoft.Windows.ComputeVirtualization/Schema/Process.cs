using System;
using System.Runtime.Serialization;

namespace Microsoft.Windows.ComputeVirtualization.Schema
{
    [DataContract]
    struct ProcessParameters
    {
        [DataMember]
        public string ApplicationName
        {
            get;
            set;
        }

        [DataMember]
        public string CommandLine
        {
            get;
            set;
        }

        [DataMember]
        public string User
        {
            get;
            set;
        }

        [DataMember]
        public string WorkingDirectory
        {
            get;
            set;
        }

        [DataMember]
        public string StdInPipe
        {
            get;
            set;
        }

        [DataMember]
        public string StdOutPipe
        {
            get;
            set;
        }

        [DataMember]
        public string StdErrPipe
        {
            get;
            set;
        }

        [DataMember]
        public Tuple<string, string> Environment
        {
            get;
            set;
        }

        [DataMember]
        public bool EmulateConsole
        {
            get;
            set;
        }

        [DataMember]
        public bool RestrictedToken
        {
            get;
            set;
        }
    }

    
    [DataContract]
    struct ConsoleSettings
    {
        [DataMember]
        public ushort Height
        {
            get;
            set;
        }

        [DataMember]
        public ushort Width
        {
            get;
            set;
        }
    }
}
