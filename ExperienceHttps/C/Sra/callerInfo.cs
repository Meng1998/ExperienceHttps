using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ExperienceHttps.C.Sra
{
    public class callerInfo
    {
        public JObject CallDeff(string message,
            [CallerMemberName] string callerMembername = "",//用来获取方法调用者的名称
            [CallerFilePath] string callerFilePath = "",//用来获取方法调用者的源代码文件路径
            [CallerLineNumber] int callerLineNumber = 0)//用来获取方法调用者所在的行号
        {
            var rng = new Random();

            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new
            {
                Date = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),

                    //Summary = Summaries[rng.Next(Summaries.Length)]
                })
            .ToArray(),
                callerMembername,
                callerFilePath,
                callerLineNumber
            }));
        }
    }
}
