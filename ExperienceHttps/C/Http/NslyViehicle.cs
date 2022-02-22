using ExperienceHttps.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.C.Http
{
    public class NslyViehicle
    {
        public static async Task<ApiResult> GetViehicleinfo()
        {
            return await Task<ApiResult>.Run(() =>
            {
                try
                {
                    BigDatapanel.NSLYvehicleInfo();
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { BigDatapanel.datestrvehicle }))
                    };
                }
                catch (Exception)
                {
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>("Viehicle Data error")
                    };
                }
            });
        }

        public static async Task<ApiResult> GetPeopleinfo()
        {
            return await Task<ApiResult>.Run(() =>
            {
                try
                {
                    BigDatapanel.NSLYvpeopleInfo();
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { BigDatapanel.datestrpeople }))
                    };
                }
                catch (Exception)
                {
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>("people Data error")
                    };
                }
            });
        }
    }
}
