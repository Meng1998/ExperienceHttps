using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExperienceMiddleware.C.Hik
{
    class GetMsg
    {
        public String GetCategory(JObject msg)
        {
            String GetData = new HIKoperation().HIKGETDATA(msg["Parameter"], out _,Int32.Parse(msg["MsgId"].ToString()), 0);
            return JsonConvert.SerializeObject(new
            {
                dirId = msg["dirId"],
                list = GetData
            }); ;
        }

    }
}
