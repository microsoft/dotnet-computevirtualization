using System;
using Microsoft.Windows.ComputeVirtualization;

namespace SampleContainerRun
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = args[1];
                var parent = args[0];
                var command = args[2];
                var id = Guid.NewGuid();
                var layers = new Layer[]
                {
                    new Layer { Id = id, Path = parent }
                };

                ContainerStorage.CreateSandbox(path);
                try
                {
                    Console.Out.WriteLine("creating container");

                    var cs = new ContainerSettings
                    {
                        SandboxPath = path,
                        Layers = layers,
                        KillOnClose = true,
                        NetworkId = HostComputeService.FindNatNetwork(),
                    };
                    using (var container = HostComputeService.CreateContainer(id.ToString(), cs))
                    {
                        Console.Out.WriteLine("starting container");
                        Console.Out.Flush();
                        container.Start();
                        var si = new ProcessStartInfo
                        {
                            CommandLine = command,
                            RedirectStandardOutput = true,
                            KillOnClose = true,
                        };
                        using (var process = container.CreateProcess(si))
                        {
                            Console.Out.Write(process.StandardOutput.ReadToEnd());
                            process.WaitForExit(5000);
                            Console.Out.WriteLine("process exited with {0}", process.ExitCode);
                        }
                    }
                }
                finally
                {
                    ContainerStorage.DestroyLayer(path);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
