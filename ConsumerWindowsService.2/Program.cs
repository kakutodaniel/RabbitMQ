using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;

namespace ConsumerWindowsService._2
{
    class Program
    {
        static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Process>();
                });

            if (isService)
            {
                builder.RunAsServiceAsync().Wait();
            }
            else
            {
                builder.RunConsoleAsync().Wait();
            }
        }
    }
}



//sc create consumer1 binPath = "C:\publish\consumer1\ConsumerWindowsService.2.exe"
// sc start consumer1
// sc stop consumer1
// after that go to windows services and configure to automatically start
