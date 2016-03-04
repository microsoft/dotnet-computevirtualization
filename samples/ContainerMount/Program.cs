using Microsoft.Windows.ComputeVirtualization;
using System;

namespace ContainerMount
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = Guid.Parse("716025c3-441b-4ae5-a985-1b44fa698530");
            Layer[] layers;
            switch (args[0])
            {
                case "-dismount":
                    ContainerStorage.DismountSandbox(args[1]);
                    break;

                case "-mount":
                    layers = new Layer[] { new Layer { Id = id, Path = args[2] } };
                    var sandbox = ContainerStorage.MountSandbox(args[1], layers);
                    Console.Out.WriteLine(sandbox.MountPath);
                    break;

                case "-create":
                    layers = new Layer[] { new Layer { Id = id, Path = args[2] } };
                    ContainerStorage.CreateSandbox(args[1], layers);
                    break;

                case "-destroy":
                    ContainerStorage.DestroyLayer(args[1]);
                    break;

                case "-process":
                    ContainerStorage.ProcessBaseLayer(args[1]);
                    break;

                case "-processvm":
                    ContainerStorage.ProcessUtilityVMImage(args[1]);
                    break;
            }
        }
    }
}
