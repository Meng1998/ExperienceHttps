using Fleck;

using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace HikAlarm
{

    public class WebsocketServer
    {
        static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        static WebSocketServer server;
        private String ipstr;
        public void WebSocketInit()
        {

            FleckLog.Level = LogLevel.Debug;
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            List<IPAddress> ipv4 = new List<IPAddress>();
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    ipv4.Add(ipa);
            }

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
               .AddJsonFile("appsettings.json");
            //AppContext.BaseDirectory + "appsettings.json";
            IConfiguration configuration = builder.Build();

            ipstr = configuration["WebSocket:ip"];
            //server = new WebSocketServer($"ws://{ipv4[0]}:4649");//34.203.114.7
            server = new WebSocketServer($"ws://{ipstr}");
            try
            {
                Log.Debug("初始化WebSocketServer连接");
                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        Log.Debug($"{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort}  => Open!");
                        allSockets.Add(socket);
                    };
                    socket.OnClose = () =>
                    {
                        Log.Debug($"{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort}  => Close!");
                        allSockets.Remove(socket);
                    };
                    socket.OnMessage = message =>
                    {
                        Log.Debug($"{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort}  => " + message);
                        allSockets.ToList().ForEach(s => s.Send(message));
                    };
                });
            }
            catch (Exception)
            {

                IntPtr Exceptionstate = new IntPtr(0);
                while (Exceptionstate == new IntPtr(0))
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
                        Console.BackgroundColor = ConsoleColor.Red; //设置背景色

                        Log.Debug($"Starting WebSocket and terminal communication");//正在启动 WebSocket和终端通信

                        Console.ResetColor(); //将控制台的前景色和背景色设为默认值

                        Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
                        Console.BackgroundColor = ConsoleColor.Yellow; //设置背景色

                        Log.Debug("重新设置本机IP地址：");

                        Console.ResetColor(); //将控制台的前景色和背景色设为默认值
                        //var ip = Console.ReadLine();
                        Log.Debug("等待....");
                        server = new WebSocketServer($"ws://{ipstr}");//34.203.114.7
                        server.Start(socket =>
                        {
                            socket.OnOpen = () =>
                            {
                                Log.Debug($"{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort}  => Open!");
                                allSockets.Add(socket);
                            };
                            socket.OnClose = () =>
                            {
                                Log.Debug($"{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort}  => Close!");
                                allSockets.Remove(socket);
                            };
                            socket.OnMessage = message =>
                            {
                                Log.Debug($"{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort} =>" + message);
                                allSockets.ToList().ForEach(s => s.Send(message));
                            };
                        });
                        Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
                        Console.BackgroundColor = ConsoleColor.Green; //设置背景色

                        Log.Debug("重新定位IP地址 并且服务初始化完成。。。");

                        Console.ResetColor(); //将控制台的前景色和背景色设为默认值

                        Exceptionstate = new IntPtr(1);

                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex.Message);
                    }
                }

            }




            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);

                    Log.Information($"Send:{input} =>{socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort}");
                }
                input = Console.ReadLine();
            }

            //ImHelper.SendChanMessage();
        }
        public static void SetWebSocketMsg(byte[] msg)
        {
            if (server != null)
            {
                Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
                Console.BackgroundColor = ConsoleColor.Green; //设置背景色
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(msg);
                }
                Console.ResetColor(); //将控制台的前景色和背景色设为默认值

            }
          
        }
        public static void SetWebSocketMsg(String msg)
        {
            if (server != null)
            {
                Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
                Console.BackgroundColor = ConsoleColor.Green; //设置背景色
                foreach (var socket in allSockets.ToList())
                {
                    //Log.Debug(msg);
                    socket.Send(msg);
                }
                Console.ResetColor(); //将控制台的前景色和背景色设为默认值

            }
        }
    }
}
