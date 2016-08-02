using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

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
        public bool CreateStdInPipe
        {
            get;
            set;
        }

        [DataMember]
        public bool CreateStdOutPipe
        {
            get;
            set;
        }

        [DataMember]
        public bool CreateStdErrPipe
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
        public Dictionary<string, string> Environment
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
    struct ProcessStatus
    {
        [DataMember]
        public uint ProcessId
        {
            get;
            set;
        }

        [DataMember]
        public bool Exited
        {
            get;
            set;
        }

        [DataMember]
        public uint ExitCode
        {
            get;
            set;
        }

        [DataMember]
        public int LastWaitResult
        {
            get;
            set;
        }
    }

    [DataContract]
    struct ProcessConsoleSize
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

    enum ProcessModifyOperation
    {
        ConsoleSize,
        CloseHandle
    }

    [DataContract]
    [KnownType(typeof(ProcessConsoleSize))]
    struct ProcessModifyRequest
    {
        [DataMember(IsRequired = true, Name = "Operation")]
        private string _Operation;
        public ProcessModifyOperation Operation
        {
            get
            {
                if (this._Operation == null)
                {
                    return default(ProcessModifyOperation);
                }

                return (ProcessModifyOperation)Enum.Parse(typeof(ProcessModifyOperation), this._Operation, true);
            }
            set
            {
                this._Operation = value.ToString();
            }
        }
        [DataMember(EmitDefaultValue = false)]
        public ProcessConsoleSize? ConsoleSize
        {
            get;
            set;
        }
    }
}
