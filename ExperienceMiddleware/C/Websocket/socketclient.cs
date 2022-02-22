using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ExperienceMiddleware.C.Websocket
{
    class socketclient
    {
        Byte[] buffer = new Byte[1024];
        private static Socket socket;
        private Thread thread;
        private static String str = "";
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public socketclient()
        {
            try
            {
                //实例化socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //连接服务器
                socket.Connect(new IPEndPoint(IPAddress.Parse("172.30.248.51"), 6000));

                thread = new Thread(StartReceive);
                thread.IsBackground = true;
                thread.Start(socket);

                String init = "<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME=\"INIT\" TYPE=\"REQ\" SEQNO=\"1\"> NULL </MESSAGE>";
                Clientsend(init);

                var timer = new System.Timers.Timer();
                timer.Elapsed += timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;
                timer.Interval = 60000;

                ////初始化通信密码
                String CommunicationKey = "<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME=\"AUTH\" TYPE =\"REQ\" SEQNO =\"1\"> <PWD>827CCB0EEA8A706C4C34A16891F84E7B</PWD> </MESSAGE>";
                Clientsend(CommunicationKey);
                // Clientsend("<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME =\"DOORSTATE\" TYPE =\"REQ\" SEQNO =\"1\"> <DoorNo>0006</DoorNo> </MESSAGE>");

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            }
            catch (Exception ex)
            {
                Console.WriteLine("服务器异常:" + ex.Message);
            }

        }

        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            String init = "<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME=\"INIT\" TYPE=\"REQ\" SEQNO=\"1\"> NULL </MESSAGE>";
            Clientsend(init);
        }


        /// <summary>
        /// 开启接收
        /// </summary>
        /// <param name="obj"></param>
        private void StartReceive(object obj)
        {

            while (true)
            {
                Socket receiveSocket = obj as Socket;
                try
                {
                    int result = receiveSocket.Receive(buffer);
                    if (result == 0)
                    {
                        break;
                    }
                    else
                    {
                        //str = Encoding.UTF8.GetString(buffer);

                        //str = utf8_gb2312(str);

                        str = Encoding.GetEncoding("gb2312").GetString(buffer).Remove(0, 12).Replace("\0", "").Replace("\r\n", "");
                        //XmlDocument xdoc = new XmlDocument();
                        //xdoc.LoadXml(str);
                        //XmlElement root = xdoc.DocumentElement;
                        //Encoding.GetString(datagram, 0, datagram.Length)
                        //.PadLeft(12).Remove(0, 12).Replace("\0", "").Replace("\r\n", "");
                        Log.Debug("接收到服务器数据: " + str);
                    }

                }
                catch (Exception ex)
                {
                    Log.Debug("服务器异常:" + ex.Message);

                }
            }

        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                thread.Abort();
                Console.WriteLine("关闭与远程服务器的连接!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常" + ex.Message);
            }
        }

        public static void Clientsend(String txt)
        {
            socket.Send(Encoding.UTF8.GetBytes(txt));
        }

        public static String Clientsended(String code, Boolean IsR)
        {
            String txt = $"<?xml version=\"1.0\" encoding=\"gb2312\"?> <MESSAGE NAME =\"DOORSTATE\" TYPE =\"REQ\" SEQNO =\"1\"> <DoorNo>{code}</DoorNo> </MESSAGE>";
            socket.Send(Encoding.UTF8.GetBytes(txt));

            Thread.Sleep(500);
            if (str.IndexOf(code) != -1)
            {
                IsR = true;
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
}
