using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Microsoft.Windows.ComputeVirtualization
{
    /// <summary>
    /// Represents a directory mapped from the host into the container.
    /// </summary>
    public class MappedDirectory
    {
        /// <summary>
        /// The path of the directory in the host.
        /// </summary>
        public string HostPath;

        /// <summary>
        /// The path of the directory in the container.
        /// </summary>
        public string ContainerPath;
    }

    /// <summary>
    /// Represents settings used to instantiate a container.
    /// </summary>
    public class ContainerSettings
    {
        /// <summary>
        /// The path to the sandbox for the layer.
        /// </summary>
        public string SandboxPath;

        /// <summary>
        /// A list of parent layers.
        /// </summary>
        public IList<Layer> Layers;

        /// <summary>
        /// A list of mapped directories.
        /// </summary>
        public IList<MappedDirectory> MappedDirectories;

        /// <summary>
        /// The ID of the network to attach to. If Guid.Empty(), do not attach the container to the network.
        /// </summary>
        public Guid NetworkId;

        /// <summary>
        /// If true, the container will be killed when the Container object is disposed.
        /// </summary>
        public bool KillOnClose;

        /// <summary>
        /// If true, create a Hyper-V container for increased isolation or image compatibility. Otherwise, create a process-based container.
        /// </summary>
        public bool HyperVContainer;

        /// <summary>
        /// The path to the utility VM image, if running as a Hyper-V container.
        /// </summary>
        public string UtilityVmPath;
    }

    public class HostComputeService
    {
        /// <summary>
        /// Find a container network that uses a NAT for connectivity.
        /// </summary>
        /// <returns>The ID of the network.</returns>
        public static Guid FindNatNetwork(IHns hns = null)
        {
            string result;
            (hns ?? HnsFactory.GetHns()).Call("GET", "/networks/", "", out result);
            var response = JsonHelper.FromJson<Schema.HNSNetworkResponse>(result);
            if (!response.Success)
            {
                throw new Win32Exception(response.Error);
            }

            string networkId = null;
            foreach (var network in response.Output)
            {
                if (network.Type == Schema.NetworkMode.NAT)
                {
                    networkId = network.ID;
                    break;
                }
            }

            if (networkId == null)
            {
                throw new Exception("could not find NAT network");
            }

            return Guid.Parse(networkId);
        }

        /// <summary>
        /// Creates (but does not start) a new container.
        /// </summary>
        /// <param name="id">The ID of the new container. Must be unique on the machine.</param>
        /// <param name="settings">The settings for the container.</param>
        /// <returns>A Container object that can be used to manipulate the container.</returns>
        public static Container CreateContainer(string id, ContainerSettings settings, IHcs hcs = null)
        {
            var h = hcs ?? HcsFactory.GetHcs();
            var hcsSettings = new Schema.ContainerSettings
            {
                SystemType = Schema.SystemType.Container,
                LayerFolderPath = settings.SandboxPath,
                Layers = settings.Layers.Select(x => new Schema.Layer { Id = x.Id, Path = x.Path }).ToArray(),
                HvPartition = settings.HyperVContainer,
                TerminateOnLastHandleClosed = settings.KillOnClose,
            };

            if (settings.MappedDirectories != null)
            {
                hcsSettings.MappedDirectories = settings.MappedDirectories.Select(x => new Schema.MappedDirectory { HostPath = x.HostPath, ContainerPath = x.ContainerPath }).ToArray();
            }

            if (settings.NetworkId != Guid.Empty)
            {
                hcsSettings.NetworkEndpoints = new Schema.NetworkEndpoint[]
                {
                    new Schema.NetworkEndpoint
                    {
                        NetworkId = settings.NetworkId,
                        EndpointName = id,
                    }
                };
            }

            if (settings.UtilityVmPath != null)
            {
                hcsSettings.HvRuntime = new Schema.UtilityVmSettings
                {
                    ImagePath = settings.UtilityVmPath
                };
            }

            IntPtr computeSystem;
            h.CreateComputeSystem(id, JsonHelper.ToJson(hcsSettings), IntPtr.Zero, out computeSystem);
            return Container.Initialize(id, computeSystem, settings.KillOnClose, true, h);
        }

        /// <summary>
        /// Retrieves an existing container.
        /// </summary>
        /// <param name="id">The ID of the container.</param>
        /// <returns>A Container object that can be used to manipulate the container.</returns>
        public static Container GetComputeSystem(string id, IHcs hcs = null)
        {
            IntPtr computeSystem;
            var h = hcs ?? HcsFactory.GetHcs();
            h.OpenComputeSystem(id, out computeSystem);

            return Container.Initialize(id, computeSystem, false, false, h);
        }
    }
}
