using HikAlarm.Hik;
using Serilog;
using System;
using System.IO;

namespace HikAlarm
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Information()
           .WriteTo.Console()
          .MinimumLevel.Debug()
          .WriteTo.File(Path.Combine(DateTime.Now.ToString("yyyyMM") + "logs", $"log.txt"),
              rollingInterval: RollingInterval.Day,
              rollOnFileSizeLimit: true)
          .CreateLogger();
          
            new Encryption().InitRestart();
            new GetDataSet().InitParameters();

            new MQTTsubscribe();

            new WebsocketServer().WebSocketInit();


            var input = Console.ReadLine();
            while (input != "exit")
            {
                input = Console.ReadLine();
            }
        }
    }
}
