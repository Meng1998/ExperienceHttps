using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.Model
{
    public class ApiResult
    {
        public Int32 Code { get; set; } = -1;
       
        /// <summary>
        /// 服务器回应消息提示
        /// </summary>
        public JObject Data { get; set; }
    }

 
    public class ApiResultString
    {
        public Int32 Code { get; set; } = -1;
        /// <summary>
        /// 服务器回应消息提示
        /// </summary>
        public String Data { get; set; }
    }
 

}
