using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;

namespace HikAlarm.Hik
{
    class MQTTsubscribe
    {
        private static MQTTKEY.MQSecretKey MQlink;//需要发送的mq集合
        

        public class MyConfiguration
        {
            public class m
            {
                public String Event_a { get; set; }
                public String Event_b { get; set; }
            }
            public class k
            {
                public String[] eventTypes { get; set; }
            }
        }

       
        public MQTTsubscribe()
        {
         
            var builder = new ConfigurationBuilder()
                                         .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
                                         .AddJsonFile("appsettings.json");

            //AppContext.BaseDirectory + "appsettings.json";
            IConfiguration configuration = builder.Build();


            // GetMQJsonV2( out _);
            ArrayList eventTypes = new ArrayList();
            switch (configuration["Edition:VersionNo"])
            {
                case "4.0":

                   // string[] yy = ;
                    Int32[] ary = Array.ConvertAll<string, int>(configuration["Edition:Event_Type"].Split(','), s => int.Parse(s));
                    JToken list = JToken.Parse(JsonConvert.SerializeObject(new
                    {
                        eventTypes = ary
                    }));
                    //string aa = "{\"eventTypes\": [131588,786434,786436]}";
                    //JToken list = JToken.Parse(aa);
                    String GetData = new HIKoperation().HIKGETDATA(list, out _, 0, 0);
                    //String GetData = "{\"msg\":\"success\",\"code\":\"0\",\"data\":{\"host\":\"tcp://34.203.114.2:1883\",\"clientId\":\"29044094\",\"userName\":\"artemis_29044094_0RVC2BPF\",\"password\":\"89OUUJ90\",\"topicName\":{\"3187675137\":\"artemis/event_face/3187675137/admin\",\"3204452353\":\"artemis/event_veh/3204452353/admin\"}}}";
                    try
                    {
                        JObject rb = JsonConvert.DeserializeObject<JObject>(GetData);
                        //Console.WriteLine(rb.ToString());
                        if ((String)rb["msg"] == "success")
                        {
                            List<String> topicName = new List<String>();
                            foreach (var item in list["eventTypes"])
                            {
                                topicName.Add((String)rb["data"]["topicName"][item.ToString()]);
                            }
                            MQlink = new MQTTKEY.MQSecretKey
                            {
                                host = (String)rb["data"]["host"],
                                clientId = Guid.NewGuid().ToString("N"),//(String)rb["data"]["clientId"],
                                userName = (String)rb["data"]["userName"],
                                password = (String)rb["data"]["password"],
                                topicName = topicName.ToArray()
                            };
                            try
                            {
                                StartMQMonitoring();
                            }
                            catch (Exception ex)
                            {
                                Log.Debug($"超时无响应或参数错误{ex.Message}");
                            }
                        }
                        else
                        {

                            MQlink = null;
                            Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
                            Console.BackgroundColor = ConsoleColor.Yellow; //设置背景色

                            Log.Debug($"SPCC接口报错，无法正常进行初始化，重启软件！无法连接SPCC服务器,如单模式请忽略。");

                            Console.ResetColor(); //将控制台的前景色和背景色设为默认值

                        }
                    }
                    catch (Exception ex)
                    {

                        MQlink = null;
                        Log.Debug(ex.ToString());
                        Log.Debug($"SPCC接口报错，无法正常进行初始化，重启软件！");

                    }
                    break;
              
                default:
                    Log.Debug("无指定版本信息 请到appsettings.json配置");
                    break;
            }
            //if (!String.IsNullOrEmpty(condition) && condition == "YES" || condition == "NO")
            //    if (condition == "YES" ? !true : !false)
            //        return;
            //    else
            //        ;
            //else
            //    return;


        }
        MqttClient mqttClient;
        /// <summary>
        /// 开启mq通信通道
        /// </summary>
        private void StartMQMonitoring()
        {

            mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
            mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
            mqttClient.Connected += MqttClient_Connected;
            mqttClient.Disconnected += MqttClient_Disconnected;
            var options = new MqttClientTcpOptions
            {
                Server = MQlink.host.Replace("tcp://", "").Split(':')[0],
                Port = Int32.Parse(MQlink.host.Replace("tcp://", "").Split(':')[1]),
                ClientId = MQlink.clientId,
                UserName = MQlink.userName,
                Password = MQlink.password,
                CleanSession = true
            };
            mqttClient.ConnectAsync(options);
        }
        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttClient_Connected(object sender, EventArgs e)
        {

            Log.Debug($"MQTT提示：MQTT链接成功，准备转发中");
           // Console.WriteLine("MQTT链接成功，准备转发中");
            foreach (var item in MQlink.topicName)//连接MQTT后订阅主题接收信息
            {
                SubscriptionTheme(item);
            }

        }
        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttClient_Disconnected(object sender, EventArgs e)
        {

            Log.Debug($"已断开MQTT连接！自动重连中...");
            Thread.Sleep(3000);
            StartMQMonitoring();//断开重新连接 并且重新主城订阅
        }
        /// <summary>
        /// MQTT订阅主题
        /// </summary>
        /// <param name="topicname">订阅主题</param>
        private void SubscriptionTheme(String topicname)
        {
            string topic = topicname.Trim();


            if (string.IsNullOrEmpty(topic))
            {
                Log.Debug($"MQTT提示：订阅主题为空！");
                return;
            }

            if (!mqttClient.IsConnected)
            {
                Log.Debug($"MQTT报错：MQTT通信未开启，因此无法订阅！");
                return;
            }

            mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce)
            });
            Console.WriteLine($"MQTT提示：已订阅[{topic}]主题");


        }
        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {

            String msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            JObject data = JsonConvert.DeserializeObject<JObject>(msg);
           // Console.WriteLine(msg);
            //发送信息
            Console.ForegroundColor = ConsoleColor.White; //设置前景色，即字体颜色
            Console.BackgroundColor = ConsoleColor.DarkGray; //设置背景色
            Log.Debug($"SPCCMQTT：Msg[{msg}]");
            Console.ResetColor(); //将控制台的前景色和背景色设为默认值
            var builder = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
                             .AddJsonFile("appsettings.json");
            //AppContext.BaseDirectory + "appsettings.json";
            IConfiguration configuration = builder.Build();
            switch (configuration["Edition:VersionNo"])
            {
               
                case "4.0":
                    WebsocketServer.SetWebSocketMsg(msg);
                    break;
                default:
                   
                    break;
            }

        }

    }
    public class GetToTime
    {
        public static String DealTimeFormat(String oldDateStr)
        {
            if (!String.IsNullOrEmpty(oldDateStr))
            {
                return Convert.ToDateTime(oldDateStr).ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
