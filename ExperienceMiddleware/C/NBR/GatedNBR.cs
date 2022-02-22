using ExperienceMiddleware.C.Websocket;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace ExperienceMiddleware.C.NBR
{
    public class GatedNBR
    {
        static Boolean Reconnect = false;
        static AsyncTcpClient client;

        public GatedNBR()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
            Console.BackgroundColor = ConsoleColor.Green; //设置背景色

            Log.Debug($"Newbell disconnect auto reconnect status:{Reconnect}");//自动重连状态为

            Console.ResetColor(); //将控制台的前景色和背景色设为默认值

            {//连接TCP服务
                client = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse("172.30.248.51"), 6000));
                client.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client_ServerDisconnected);
                client.DatagramReceived += new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(client_PlaintextReceived);
                client.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client_ServerConnected);
                client.Connect();
            }
        }
        static Boolean Initbl = false;


        #region TCP消息处理
        static void client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
            Console.BackgroundColor = ConsoleColor.Green; //设置背景色
            Log.Debug("Newbell access control service·");//纽贝尔和数据连接成功.
            Console.ResetColor(); //将控制台的前景色和背景色设为默认值
            if (!Initbl)
            {
                Initbl = true;
                //初始化通信
                String init = "<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME=\"INIT\" TYPE=\"REQ\" SEQNO=\"1\"> NULL </MESSAGE>";
                client.Send(init);
                ////初始化通信密码
                String CommunicationKey = "<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME=\"AUTH\" TYPE =\"REQ\" SEQNO =\"1\"> <PWD>827CCB0EEA8A706C4C34A16891F84E7B</PWD> </MESSAGE>";
                client.Send(CommunicationKey);
            }
        }
        //public static List<Record> record = new List<Record>();

        static string str = "";
        static void client_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {

            Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
            Console.BackgroundColor = ConsoleColor.DarkGray; //设置背景色
            Log.Debug(string.Format("Server : {0} --> ",
                e.Datagram));
            Console.ResetColor(); //将控制台的前景色和背景色设为默认值

            try
            {

                str = Encoding.GetEncoding("gb2312").GetString(e.Datagram).Remove(0, 12);
                Log.Debug(string.Format("Server : {0} --> ",
                   str));

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(str);
                XmlElement xmlElem = xmlDoc.DocumentElement;//获取根节点
                if (xmlElem.GetElementsByTagName("EmpName").Count > 0)
                {
                    XmlNode rootNode = xmlDoc.SelectSingleNode("MESSAGE");
                    //string doorNo = rootNode.SelectSingleNode("EmpName").InnerText;
                    //Console.WriteLine("姓名：" + doorNo);
                    String mpName = rootNode.SelectSingleNode("EmpName").InnerText;
                    String ventTime = rootNode.SelectSingleNode("EventTime").InnerText;
                    String eptName = rootNode.SelectSingleNode("DeptName").InnerText;
                    String ventName = rootNode.SelectSingleNode("EventName").InnerText;
                    String oorNo = rootNode.SelectSingleNode("DoorNo").InnerText;
                    String oState = rootNode.SelectSingleNode("IoState").InnerText;

                    WebsocketServer.SetWebSocketMsg(JsonConvert.SerializeObject(new { 
                        type="door",
                        data=new {
                            EmpName= rootNode.SelectSingleNode("EmpName").InnerText,
                            EventTime= rootNode.SelectSingleNode("EventTime").InnerText,
                            DeptName = rootNode.SelectSingleNode("DeptName").InnerText,
                            EventName = rootNode.SelectSingleNode("EventName").InnerText,
                            DoorNo = rootNode.SelectSingleNode("DoorNo").InnerText,
                            IoState = rootNode.SelectSingleNode("IoState").InnerText,
                        }
                    }));
                };//取节点名

                if (MessageControlSending.SendMessage.state)
                {
                    MessageControlSending.SendMessage.state = false;
                    //  MessageControlSending.SendMessage.Msg = System.Text.Encoding.UTF8.GetString(temporary);
                }
            }
            catch (Exception)
            {

            }


        }
        public static string utf8_gb2312(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.UTF8;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }

        static void client_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {

            if (Reconnect)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Initbl = false;
                client = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse("172.30.248.51"), 6000));
                client.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client_ServerDisconnected);
                client.DatagramReceived += new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(client_PlaintextReceived);
                client.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client_ServerConnected);
                client.Connect();
            }
            Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
            Console.BackgroundColor = ConsoleColor.Red; //设置背景色
            Log.Debug(string.Format(CultureInfo.InvariantCulture,
                "TCP server {0} has disconnected.",
                e.ToString()));
            Console.ResetColor(); //将控制台的前景色和背景色设为默认值

        }
        #endregion



        /// <summary>
        /// 门状态
        /// </summary>
        /// <param name="DoorNo">门禁编码</param>
        /// <returns></returns>
        public static String GateState(String DoorNo)
        {
            try
            {

                client.Send($"<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME =\"DOORSTATE\" TYPE =\"REQ\" SEQNO =\"1\"> <DoorNo>{DoorNo}</DoorNo> </MESSAGE> ");
                MessageControlSending.SendMessage.state = true;

                Int32 count = 0;
                Thread.Sleep(500);
                while (true)
                {
                    if (str.IndexOf(DoorNo) != -1)
                    {
                        switch (str.Split(new string[] { "<DoorState>" }, StringSplitOptions.RemoveEmptyEntries)[1].Substring(0, 1))
                        {
                            case "0":
                                return "门开";
                            case "1":
                                return "门关";
                            default:
                                return "异常";
                                //88695352
                        }
                    }
                    else
                    {
                        return "异常";
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                return "TCP 请求错误 纽贝尔服务未被管理员设置启动!";
            }

        }
    }
    public class getTime
    {
        public static DateTime dateBegin { get; set; }
        public static DateTime dateEnd { get; set; }


        public static int SubTest(DateTime dateBegin, DateTime dateEnd)
        {   //TimeSpan类
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return (int)ts3.TotalMinutes;
        }
        public static int TesRecord(DateTime dateBegin, DateTime dateEnd)
        {   //TimeSpan类
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return (int)ts3.TotalHours;
        }



    }

    public class MessageControlSending
    {
        /// <summary>
        /// 发送消息是否等待接收第一条信息保存返回信息并且处理发送
        /// </summary>
        public class SendMessage
        {
            public static Boolean state { get; set; } = false;
            public static String Msg { get; set; }
        }
    }
}
