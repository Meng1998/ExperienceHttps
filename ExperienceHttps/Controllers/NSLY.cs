using ExperienceHttps.C.Http;
using ExperienceHttps.C.Mqtt;
using ExperienceHttps.C.PG;
using ExperienceHttps.Model;
using ExperienceHttps.server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.Controllers
{
    [ApiController]
    [Route("NSLY")]
    public class NSLY : ControllerBase
    {

        private readonly IAccountServices _account;

        public NSLY(IAccountServices account)
        {
            _account = account;
        }
        /// <summary>
        /// 设备结构详情
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        [Route("DoorState")]
        [HttpPost]
        public JObject Doorstatuc([FromBody] Mod mod)
        {
            DataSet Org = new Pgsql().PgExecute("select * from grave_camera where type_name='" + mod.type_name + "'");

            DataSet Root = new Pgsql().PgExecute("select * from orginzed where organiz_id='root000000' ");

            DataSet orginzedList = new Pgsql().PgExecute("SELECT * FROM \"orginzed\"");


            //String sql ="select * from grave_camera where type_name='" + mod.type_name +"'";
            //var sqlorder = "select * from orginzed ";
            //var sqlOrg = "select * from orginzed where orginzed_id='root000000'";
            List<String> lis = new List<String>();

            List<Orgzind> orgList = new List<Orgzind>();
            List<camera> camList = new List<camera>();

            foreach (DataRow item in orginzedList.Tables[0].Rows)
            {
                foreach (DataRow arg in Org.Tables[0].Rows)
                {
                    if (item["organiz_id"].ToString() == arg["orgin_id"].ToString())
                    {
                        camera cam = new camera()
                        {
                            id = arg["id"].ToString(),
                            type_name = arg["type_name"].ToString(),
                            device_code = arg["device_code"].ToString(),
                            device_name = arg["device_name"].ToString(),
                            device_info = arg["device_info"].ToString(),
                            order = Int32.Parse(arg["order"].ToString()),
                            orgin_id = arg["orgin_id"].ToString()
                        };
                        camList.Add(cam);

                        orgList.Add(new Orgzind()
                        {
                            id = item["id"].ToString(),
                            pid = item["pid"].ToString(),
                            organiz_name = item["organiz_name"].ToString(),
                            organiz_id = item["organiz_id"].ToString(),
                            order_num = camList

                        });
                    }
                }
            }

            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { orgList }));
            // return await Mqttclient.XhGetmoutStr(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(Pam)));
        }

        /// <summary>
        /// 客流统计
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        [Route("peopleTotal")]
        [HttpPost]
        public async Task<ApiResult> peopleTotal()
        {
            return await NslyViehicle.GetPeopleinfo();
        }

        /// <summary>
        /// 车流统计
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        [Route("vehicleTotal")]
        [HttpPost]
        public async Task<ApiResult> vehicleTotal()
        {
            //Parameter.startTime = DateTime.Parse(Parameter.startTime).ToString("s") + "+08:00";
            //Parameter.endTime = DateTime.Parse(Parameter.endTime).ToString("s") + "+08:00";
            //var Pam = new
            //{
            //    MsgType = "Hik",
            //    dirId = Guid.NewGuid().ToString(),
            //    MsgId = 8,
            //    Parameter,
            //    eventType = "vehicle"//事件名称代码
            //};

            return await NslyViehicle.GetViehicleinfo();
        }

        [Route("Getvalue")]
        [HttpGet]
        public String Getvalue()
        {
            String a = _account.Context();
            return a;
        }

    }
}
