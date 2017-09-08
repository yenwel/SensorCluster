using SensorCluster.SensorSystem;
using Topshelf;

namespace SensorCluster
{
    // execute add Administrative prompt to add direct rout to scale:
    // route add 10.10.5.5 mask 255.255.255.255 10.10.5.5
    public class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SensorActorService>(s =>
                {
                    s.ConstructUsing(n => new SensorActorService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    //continue and restart directives are also available
                });
                x.RunAsLocalSystem();
                x.UseAssemblyInfoForServiceInfo();
            });
        }
    }
}
