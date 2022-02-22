
using ExperienceMiddleware.M;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ExperienceMiddleware
{
    class HIKoperation
    {
        operationKey operatio = new operationKey();
        readonly GetDataSet GetSetData = new GetDataSet();

        class RawData
        { 
        
            public string data { get; set; }

            public Boolean bl { get; set; }

        }

        private async Task<RawData> DataHIKHttpRaw(SecretKey KEY, Int32 GetKEYIndex, JToken Parameter)
        {
            var Data = operatio.GetKeyHttpPostRaw(GetSetData.StitchingParameters(KEY), KEY, Parameter, out Boolean bl, GetKEYIndex);
            return new RawData { 
                data = Data,
                bl = bl
            };
        }


        #region ISCGETDATA
        /// <summary>
        /// 综合数据获取方法(异步)
        /// </summary>
        /// <returns></returns>
        public void HIKGETDATA(JToken Parameter, Int32 GetKEYType = 0, Int32 GetKEYIndex = 0)
        {
            var KEY = GetSetData.GetISCkey(GetKEYIndex, GetKEYType);
            //String parameter = operatio.GetKeyHttpPostRaw(GetSetData.StitchingParameters(KEY), KEY, Parameter, out bl, GetKEYIndex);
            Task<RawData> t = DataHIKHttpRaw(KEY, GetKEYIndex,Parameter);
            Task.Run(async () =>
            {
                RawData msg = await t;
                //if (msg.bl)
                //    server.Send(e, msg.data);
                //else
                //    server.Send(e, JsonConvert.SerializeObject(new
                //    {
                //        msg = "error",
                //        Remarks = "接口超时或报错！" + msg.data
                //    }));
            });
        }
        /// <summary>
        /// 综合数据获取方法(同步)
        /// </summary>
        /// <returns></returns>
        public String HIKGETDATA(JToken Parameter, out Boolean bl, Int32 GetKEYType = 0, Int32 GetKEYIndex = 0)
        {
           
            var KEY = GetSetData.GetISCkey(GetKEYIndex, GetKEYType);

            String parameter = operatio.GetKeyHttpPostRaw(GetSetData.StitchingParameters(KEY), KEY, Parameter, out bl, GetKEYIndex);
            if (bl)
                return parameter;
            else
                return JsonConvert.SerializeObject(new
                {
                    msg = "error",
                    Remarks = "接口超时或报错！" + parameter
                });
        }
        #endregion

    }
}
