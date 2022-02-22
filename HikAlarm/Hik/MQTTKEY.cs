using Newtonsoft.Json;
using System;

namespace HikAlarm.Hik
{
    class MQTTKEY
    {
        public class MQSecretKey
        {
            //三个必要的通信参数
            public String host { get; set; }
            public String clientId { get; set; }
            public String userName { get; set; }
            public String password { get; set; }
            public String[] topicName { get; set; }

        }
        /// <summary>
        /// 获取根区域信息JOSN
        /// </summary>
        public static String JOSNPOST_Rootregion(dynamic data)
        {
            var Object = new
            {
                msg = new
                {

                    id = data.SeventId,
                    event_type = data.SERevent_type,
                    status = data.SERstatus,
                    start_time = data.SERstart_time,
                    stop_time = data.SERstop_time,
                    event_level = data.SERevent_level,
                    event_name = data.SERevent_name,
                    device_code = data.SERdevice_code,
                    device_name = data.SERdevice_name,
                    event_type_name = data.SERevent_type_name,
                    ext_info = data.SERext_info
                }

            };
            String serJson = JsonConvert.SerializeObject(Object);
            return serJson;
        }

    }
}
