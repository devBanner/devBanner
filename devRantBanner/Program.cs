using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace devBanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseUrls("http://*:5000")
                .Build();

            host.Run();
        }
    }
}
