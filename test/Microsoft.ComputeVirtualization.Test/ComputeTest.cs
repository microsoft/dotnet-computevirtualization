using System;
using System.Threading;
using System.Threading.Tasks;

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
        public void ContainerKillOnCloseCannotHang()
        {
            var cs = new ContainerSettings
            {
                SandboxPath = sandbox.path,
                Layers = sandbox.layers,
                KillOnClose = true,
                NetworkId = HostComputeService.FindNatNetwork(),
            };
            var container = HostComputeService.CreateContainer(id.ToString(), cs);
            container.Start();
            Assert.True(Task.Factory.StartNew(() => { container.Dispose(); }).Wait(10000));
            Assert.ThrowsAny<Exception>(() => { HostComputeService.GetComputeSystem(id.ToString()); });
        }

        [Fact]
        public void ContainerKillOnCloseWithShutdownCannotHang()
        {
            var cs = new ContainerSettings
            {
                SandboxPath = sandbox.path,
                Layers = sandbox.layers,
                KillOnClose = true,
                NetworkId = HostComputeService.FindNatNetwork(),
            };
            var container = HostComputeService.CreateContainer(id.ToString(), cs);
            container.Start();
            container.Shutdown();
            Assert.True(Task.Factory.StartNew(() => { container.Dispose(); }).Wait(10000));
            Assert.ThrowsAny<HcsException>(() => { HostComputeService.GetComputeSystem(id.ToString()); });
        }

        [Fact]
        public void ContainerKillOnCloseWithKillCannotHang()
        {
            var cs = new ContainerSettings
            {
                SandboxPath = sandbox.path,
                Layers = sandbox.layers,
                KillOnClose = true,
                NetworkId = HostComputeService.FindNatNetwork(),
            };
            var container = HostComputeService.CreateContainer(id.ToString(), cs);
            container.Start();
            container.Kill();
            Assert.True(Task.Factory.StartNew(() => { container.Dispose(); }).Wait(10000));
            Assert.ThrowsAny<HcsException>(() => { HostComputeService.GetComputeSystem(id.ToString()); });
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
                    KillOnClose = false,
                };
                using (var process = container.CreateProcess(si))
                {
                    Assert.True(process.WaitForExit(10000), "Process did not exit after 10 seconds");
                    Assert.Equal(0, process.ExitCode);
                }
                container.Shutdown();
            }
        }

        [Fact]
        public void RunContainerExit1()
        {
            var command = "cmd /c exit 1";
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
                    KillOnClose = false,
                };
                using (var process = container.CreateProcess(si))
                {
                    Assert.True(process.WaitForExit(10000), "Process did not exit after 10 seconds");
                    Assert.Equal(1, process.ExitCode);
                }
                container.Shutdown();
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
                    KillOnClose = false,
                };
                using (var process = container.CreateProcess(si))
                {
                    Thread.Sleep(1000);
                    Assert.True(process.WaitForExit(10000), "Container did not exit after 10 seconds");
                    Assert.Equal(0, process.ExitCode);
                }
                container.Shutdown();
            }
        }

        [Fact]
        public void ProcessKillOnCloseCannotHang()
        {
            var command = "powershell -c sleep 10000";
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
                var process = container.CreateProcess(si);
                Assert.True(Task.Factory.StartNew(() => { process.Dispose(); }).Wait(10000));
                container.Shutdown();
            }
        }

        [Fact]
        public void ProcessKillOnCloseWithKillCannotHang()
        {
            var command = "powershell -c sleep 10000";
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
                var process = container.CreateProcess(si);
                process.Kill();
                Assert.True(Task.Factory.StartNew(() => { process.Dispose(); }).Wait(10000));
                container.Shutdown();
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