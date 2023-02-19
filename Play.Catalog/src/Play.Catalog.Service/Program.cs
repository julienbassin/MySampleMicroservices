using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Play.Catalog.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create a hostbuilder using commandline then we build and create an instance of host
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            /** 

            Initializes a new instance of the HostBuilder class with pre-configured defaults.
            use Kestrel as the web server and configure it using the application's configuration providers
            Specify the startup type to be used by the web host.
                        
            **/
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
