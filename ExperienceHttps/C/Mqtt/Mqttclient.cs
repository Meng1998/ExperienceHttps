using ExperienceHttps.Model;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExperienceHttps.C.Mqtt
{
    public class Mqttclient
    {

        private static MqttClient mqttClient = null;

        private static Dictionary<string, string> pass = new Dictionary<string, string>();

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
            var options = new MqttClientTcpOptions
            {
                Server = "127.0.0.1",
                ClientId = Guid.NewGuid().ToString().Substring(0, 5),
                UserName = "ty",
                Password = "tyaimap",
                CleanSession = true
            };
            try
            {
                await mqttClient.ConnectAsync(options);
                BtnSubscribe_ClickAsync();
            }
            catch (Exception ex)
            {



                //Console.WriteLine($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
            }
        }

        private void MqttClient_Connected(object sender, EventArgs e)
        {

            Console.WriteLine("已连接服务器！" + Environment.NewLine);
        }

        private void MqttClient_Disconnected(object sender, EventArgs e)
        {
           // Console.WriteLine("已断开MQTT连接！" + Environment.NewLine);
            Thread.Sleep(1000);
            Task.Run(async () => { await ConnectMqttServerAsync(); });
        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {

            JObject msg = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

            pass[msg["dirId"].ToString()] = msg["list"].ToString();
            // Console.WriteLine($">> {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}{Environment.NewLine}");

        }

        public void BtnSubscribe_ClickAsync()
        {
            string topic = "Mid";

            if (string.IsNullOrEmpty(topic))
            {
                Console.WriteLine("订阅主题不能为空！");
                return;
            }

            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("客户端尚未连接！");
                return;
            }

            mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce)
            });

            Console.WriteLine($"已订阅[{topic}]主题" + Environment.NewLine);

        }

        public static async Task<ApiResult> XhGetmout(JObject data)
        {
            return await Task<ApiResult>.Run(() =>
            {

                String result = "";
                pass.Add(data["dirId"].ToString(), "");
                int a = 0;
                SetPublish(data.ToString());

                while (true)
                {
                    if (pass[data["dirId"].ToString()] != "")
                    {
                        result = pass[data["dirId"].ToString()];
                        pass.Remove(data["dirId"].ToString());
                        try
                        {
                            return new ApiResult()
                            {
                                Code = 200,
                                Data = JsonConvert.DeserializeObject<JObject>(result)
                            };
                        }
                        catch (Exception)
                        {
                            return new ApiResult()
                            {
                                Code = 200,
                                Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(result))
                            };
                        }
                    }
                    else if (a > 4)
                    {
                        return new ApiResult()
                        {
                            Code = 400,
                            Data = null
                        };
                    }
                    Thread.Sleep(1000);
                    a++;
                }


            });
        }

        public static async Task<ApiResultString> XhGetmoutStr(JObject data)
        {
            return await Task<ApiResultString>.Run(() =>
            {
                String result = "";
                pass.Add(data["dirId"].ToString(), "");
                int a = 0;
                SetPublish(data.ToString());

                while (true)
                {
                    if (pass[data["dirId"].ToString()] != "")
                    {
                        result = pass[data["dirId"].ToString()];
                        pass.Remove(data["dirId"].ToString());
                        return new ApiResultString()
                        {
                            Code = 200,
                            Data = result
                        };
                    }
                    else if (a > 4)
                    {
                        return new ApiResultString()
                        {
                            Code = 400,
                            Data = ""
                        };
                    }
                    Thread.Sleep(1000);
                    a++;
                }
            });
        }


        public static ApiResult AyXhGetmout(JObject data)
        {

            String result = "";
            pass.Add(data["dirId"].ToString(), "");
            int a = 0;
            SetPublish(data.ToString());

            while (true)
            {
                if (pass[data["dirId"].ToString()] != "")
                {
                    result = pass[data["dirId"].ToString()];
                    pass.Remove(data["dirId"].ToString());
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(result)
                    };
                }
                else if (a > 5)
                {
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = null
                    };
                }
                Thread.Sleep(1000);
                //a++;
            }



        }

        public static void SetPublish(String msg)
        {

            string topic = "API";

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
