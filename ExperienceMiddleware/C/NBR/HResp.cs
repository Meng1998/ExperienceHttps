using ExperienceMiddleware.C.Websocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceMiddleware.C.NBR
{
    class HResp
    {
        public String DeviceState(JObject msg)
        {
            // Clientsend();
            String code = msg["Parameter"].ToString();
            
                return JsonConvert.SerializeObject(new
                {
                    dirId = msg["dirId"],
                    list = GatedNBR.GateState(code)
                });

        }

       
      

    }
}
