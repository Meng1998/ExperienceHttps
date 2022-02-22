using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExperienceHttps.C.Http;
using ExperienceHttps.C.Mqtt;
using ExperienceHttps.C.Sra;
using ExperienceHttps.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExperienceHttps.Controllers
{
    [ApiController]
    [Route("XJ")]
    public class XJ : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecast> _logger;

        public XJ(ILogger<WeatherForecast> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 对讲状态
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IntercomState")]
        public async Task<ApiResult> XJDJstatuc(String name)
        {
           return await RyDJ.DJstatuc(name);
        }

       
        /// <summary>
        /// 门禁状态
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        [Route("DoorState")]
        [HttpPost]
        public async Task<ApiResultString> Doorstatuc( String Parameter)
        {
            var Pam = new
            {
                MsgType = "NBR",
                dirId = Guid.NewGuid().ToString(),
                Parameter,
                eventType = "State"//事件名称代码
            };

            return await Mqttclient.XhGetmoutStr(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(Pam)));
        }

        /// <summary>
        /// 报警提交
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        [Route("AlarmSubmit")]
        [HttpPost]
        public async Task<ApiResult> AlarmSubmit([FromBody] JObject Parameter)
        {
            var Pam = new
            {
                MsgType = "Hik",
                dirId = Guid.NewGuid().ToString(),
                MsgId=7,
                Parameter,
                eventType = "Submit"//事件名称代码
            };

            return await Mqttclient.XhGetmout(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(Pam)));
        }

        /// <summary>
        /// 模拟报警
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        [Route("Alarmsimula")]
        [HttpPost]
        public async Task<ApiResultString> Alarmsimula([FromBody]JObject Parameter)
        {
            var Pam = new
            {
                MsgType = "Hik",
                dirId = Guid.NewGuid().ToString(),
                Parameter=  new {
                   type="alarm",
                   data= Parameter
                } ,
                eventType = "Alarm"//事件名称代码
            };

            return await Mqttclient.XhGetmoutStr(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(Pam)));
        }


        /// <summary>
        /// 人员定位根据名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("GetNameList")]
        [HttpGet]
        public async Task<ApiResult> GetNameList(String name)
        {
            return await RyDJ.RyGetname(name);
        }

        /// <summary>
        /// 人员定位根据区域id查人员计总
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [Route("GetRegionlocation")]
        [HttpGet]
        public async Task<ApiResult> GetRegionlocation(Int32 id)
        {
            return await RyDJ.Rylocation(id);
        }


        /// <summary>
        /// 监内人车数量分布
        /// </summary>
        /// <returns></returns>
        [Route("PrisonPeoplerookList")]
        [HttpGet]
        public async Task<ApiResult> PrisonPeoplerookList()
        {
            return await RyDJ.PrisonPeopleList();
        }

        /// <summary>
        /// 重点罪犯数量分布
        /// </summary>
        /// <returns></returns>
        [Route("PrisonKeycriminalList")]
        [HttpGet]
        public async Task<ApiResult> PrisonKeycriminalList()
        {
            return await RyDJ.PrisonKeyList();
        }

        /// <summary>
        /// 监狱态势
        /// </summary>
        /// <returns></returns>
        [Route("PrisonSituation")]
        [HttpGet]
        public async Task<ApiResult> PrisonSituation()
        {
            return await RyDJ.PrisonSituat();
        }

        /// <summary>
        /// 车辆/人员进出记录
        /// </summary>
        /// <returns></returns>
        [Route("PrisonvehicleRecord")]
        [HttpGet]
        public async Task<ApiResult> PrisonvehicleRecord()
        {
            return await RyDJ.Prisonvehicle();
        }

        /// <summary>
        /// 报警统计管理
        /// </summary>
        /// <returns></returns>
        [Route("AlarmrunList")]
        [HttpGet]
        public async Task<ApiResult> AlarmrunList()
        {
            return await RyDJ.Alarmrun();
        }

        /// <summary>
        /// Token鉴权回传
        /// </summary>
        /// <returns></returns>
        [Route("systemToken")]
        [HttpPost]
        public void systemToken([FromBody]String msg)
        {
             Console.WriteLine("Token鉴权回传:" +msg);
         
        }

    }
}
