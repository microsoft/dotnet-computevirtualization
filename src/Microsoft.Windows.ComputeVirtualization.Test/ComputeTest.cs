using System;
using System.Threading;

using Xunit;

namespace Microsoft.Windows.ComputeVirtualization.Test
{
    public class ServercoreContainerTests : IDisposable
    {
        Guid id = Guid.NewGuid();
        Sandbox sandbox;

        public ServercoreContainerTests()
        {
            sandbox = new Sandbox(Sandbox.ContainerType.ServerCore, id);
        }

        [Fact]
        public void RunContainerExit0()
        {
            var command = "cmd /c exit";
            var cs = new ContainerSettings
            {
                SandboxPath = sandbox.path,
                Layers = sandbox.layers,
                KillOnClose = true,
                NetworkId = HostComputeService.FindNatNetwork(),
            };
            using (var container = HostComputeService.CreateContainer(id.ToString(), cs))
            {
                container.Start();
                var si = new ProcessStartInfo
                {
                    CommandLine = command,
                    KillOnClose = true,
                };
                using (var process = container.CreateProcess(si))
                {
                    Assert.True(process.WaitForExit(10000), "Container did not exit after 10 seconds");
                    Assert.Equal(0, process.ExitCode);
                }
            }
        }

        [Fact]
        public void ProcessCannotExitBeforeWait()
        {
            var command = "cmd /c exit";
            var cs = new ContainerSettings
            {
                SandboxPath = sandbox.path,
                Layers = sandbox.layers,
                KillOnClose = true,
                NetworkId = HostComputeService.FindNatNetwork(),
            };
            using (var container = HostComputeService.CreateContainer(id.ToString(), cs))
            {
                container.Start();
                var si = new ProcessStartInfo
                {
                    CommandLine = command,
                    KillOnClose = true,
                };
                using (var process = container.CreateProcess(si))
                {
                    Thread.Sleep(1000);
                    Assert.True(process.WaitForExit(10000), "Container did not exit after 10 seconds");
                    Assert.Equal(0, process.ExitCode);
                }
            }
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }
    }

    public class Sandbox : IDisposable
    {
        public enum ContainerType { ServerCore, NanoServer }
        const string servercoreBaseLayerEnv = "SERVERCORE_BASE_LAYER";
        const string nanoserverBaseLayerEnv = "NANOSERVER_BASE_LAYER";
        public string path { get; }
        public Layer[] layers { get; }
        public Sandbox(ContainerType containerType, Guid id)
        {
            string parent;
            string baseLayerEnv;
            switch (containerType)
            {
                case ContainerType.ServerCore:
                    baseLayerEnv = servercoreBaseLayerEnv;
                    break;
                case ContainerType.NanoServer:
                    baseLayerEnv = nanoserverBaseLayerEnv;
                    break;
                default:
                    throw new ArgumentException("Invalid container type");
            }
            parent = Environment.GetEnvironmentVariable(baseLayerEnv);

            path = String.Format("C:\\\\ComputeVirtualizationTest\\{0}", id.ToString());

            layers = new Layer[]
            {
                    new Layer { Id = id, Path = parent }
            };

            try
            {
                ContainerStorage.CreateSandbox(path, layers);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Failed to create sandbox, ensure that environment variable {0} is set to a valid base layer path", baseLayerEnv), ex);
            }
        }

        public void Dispose()
        {
            ContainerStorage.DestroyLayer(path);
        }
    }
}