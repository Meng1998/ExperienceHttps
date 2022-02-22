using ExperienceMiddleware.C.Hik;
using ExperienceMiddleware.C.MQTT;
using ExperienceMiddleware.C.NBR;
using ExperienceMiddleware.C.Websocket;
using ExperienceMiddleware.External;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Serilog;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ExperienceMiddleware
{
    class Program
    {
        

        static void Main(string[] args)
        {


            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           // .WriteTo.Console()
           .MinimumLevel.Debug()
           .WriteTo.File(Path.Combine(DateTime.Now.ToString("yyyyMM") + "logs", $"log.txt"),
               rollingInterval: RollingInterval.Day,
               rollOnFileSizeLimit: true)
           .CreateLogger();
            #region MQTT服务
            new Mqttserver();
            new Mqttclient();
            #endregion
            new Encryption().InitRestart();
            new GetDataSet().InitParameters();

            //new MQTTsubscribe();

            new GatedNBR();
            //new socketclient();


            new WebsocketServer().WebSocketInit();


            var input = Console.ReadLine();
            while (input != "exit")
            {
                input = Console.ReadLine();
            }
            
        }

      
        

        

       

    }
   

}
