using ExperienceMiddleware.C.Hik;
using ExperienceMiddleware.C.NBR;
using ExperienceMiddleware.C.PGsql;
using ExperienceMiddleware.C.Websocket;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExperienceMiddleware.C.MQTT
{
    public class Mqttclient
    {

        private static MqttClient mqttClient = null;

        public Mqttclient()
        {

            Task.Run(async () => { await ConnectMqttServerAsync(); });
        }

        private async Task ConnectMqttServerAsync()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
                mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                mqttClient.Connected += MqttClient_Connected;
                mqttClient.Disconnected += MqttClient_Disconnected;
            }

            try
            {
                var options = new MqttClientTcpOptions
                {
                    Server = "127.0.0.1",
                    ClientId = Guid.NewGuid().ToString().Substring(0, 5),
                    UserName = "ty",
                    Password = "tyaimap",
                    CleanSession = true
                };

                await mqttClient.ConnectAsync(options);
                BtnSubscribe_ClickAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
            }
        }

        private void MqttClient_Connected(object sender, EventArgs e)
        {

            Console.WriteLine("已连接到MQTT服务器！" + Environment.NewLine);
        }

        private void MqttClient_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("已断开MQTT连接！" + Environment.NewLine);
            Thread.Sleep(3000);
            Task.Run(async () => { await ConnectMqttServerAsync(); });
        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            String aa = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            JObject msg = JObject.Parse(aa);
            XhGetmout(msg);
           // Console.WriteLine("<<" + msg);
            //Console.WriteLine($">> {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}{Environment.NewLine}");

        }

        public void BtnSubscribe_ClickAsync()
        {
            string topic = "API";
            try
            {


                if (string.IsNullOrEmpty(topic))
                {
                    Console.WriteLine("订阅主题不能为空！");
                    return;
                }

                if (!mqttClient.IsConnected)
                {
                    Console.WriteLine("MQTT客户端尚未连接！");
                    return;
                }

                mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce)
            });

                Console.WriteLine($"已订阅[{topic}]主题" + Environment.NewLine);
            }
            catch (Exception)
            {

                Task.Run(() =>
                {
                    Boolean R = false;
                    while (R)
                    {
                        Thread.Sleep(1000);
                        if (!mqttClient.IsConnected)
                        {
                            mqttClient.SubscribeAsync(new List<TopicFilter> {
                             new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce)
                                 });

                            Console.WriteLine($"已订阅[{topic}]主题" + Environment.NewLine);
                            return;
                        }
                    }
                });
            }
        }

        public static void XhGetmout(JObject data)
        {
            switch (data["MsgType"].ToString())
            {
                case "Hik":
                    switch (data["eventType"].ToString())
                    {
                        case "Fire":
                            SetPublish(new SetPgsql().GetInfo(data));
                            break;
                        case "Submit":
                            SetPublish(JsonConvert.SerializeObject(new
                            {
                                dirId = data["dirId"],
                                list = new HIKoperation().HIKGETDATA(data["Parameter"], out _, 0, Int32.Parse(data["MsgId"].ToString()))
                            }));
                            break;
                        case "Alarm":
                            SetPublish(JsonConvert.SerializeObject(new
                            {
                                dirId = data["dirId"],
                                list = "请求成功"
                            }));
                            WebsocketServer.SetWebSocketMsg(data["Parameter"].ToString());
                            break;
                        
                    }
                    break;
                case "DH":
                    switch (data["eventType"].ToString())
                    {
                        case "Video":
                            break;
                        default:
                            break;
                    }
                    break;

                //case "Mysql":
                //    switch (data["eventType"].ToString())
                //    {
                //        case "Peoplerook":
                //            break;
                //        case "Peoplerook":
                //            break;
                //        case "Peoplerook":
                //            break;
                //        case "Peoplerook":
                //            break;
                //        case "Peoplerook":
                //            break;
                //        case "Peoplerook":
                //            break;
                //        default:
                //            break;
                //    }
                    break;

                case "NBR":
                    switch (data["eventType"].ToString())
                    {
                        case "State":
                            SetPublish( new HResp().DeviceState(data));
                            break;
                        default:
                            break;
                    }
                    break;
                case "RYDW":
                    switch (data["eventType"].ToString())
                    {
                        case "Name":
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

        }

        public static void SetPublish(String msg)
        {
            string topic = "Mid";

            if (string.IsNullOrEmpty(topic))
            {
                Console.WriteLine("发布主题不能为空！");
                return;
            }

            var appMsg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(msg), MqttQualityOfServiceLevel.AtMostOnce, false);
            mqttClient.PublishAsync(appMsg);
        }

       

    }
}
