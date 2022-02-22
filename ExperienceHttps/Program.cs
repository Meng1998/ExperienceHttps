using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExperienceHttps.C.Http;
using ExperienceHttps.Sra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ExperienceHttps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new C.Mqtt.Mqttclient();
           // new Encryption().InitRestart();
            //new BigDatapanel();

            Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Information()
          // .WriteTo.Console()
          .MinimumLevel.Debug()
          .WriteTo.File(Path.Combine(DateTime.Now.ToString("yyyyMM") + "logs", $"log.txt"),
              rollingInterval: RollingInterval.Day,
              rollOnFileSizeLimit: true)
          .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
