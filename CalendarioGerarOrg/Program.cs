using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CalendarioGerarOrg
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseUrls(new string[] { "https://*:5001", "http://*:5000" }) // change your custom port
                //.UseUrls(new string[] { "http://*:80", "https://*:443" }) // change your custom port
                //.UseUrls(new string[] { "http://*:80" }) // change your custom port
                .UseStartup<Startup>();
    }
}
