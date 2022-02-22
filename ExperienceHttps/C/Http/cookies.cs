using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.C.Http
{
    public class cookies
    {
        public static String GetRespson(String url)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                JObject msg = JObject.Parse(response.Content);
                return url = msg["token"].ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }
        public static JObject PostRespson(String url, String data)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return JObject.Parse(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject DWhttpGet(String url, String token)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", token);
                IRestResponse response = client.Execute(request);
                return JObject.Parse(response.Content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static String Yrtoken()
        {
            try
            {
                var client = new RestClient("http://172.30.248.113:7001/api/login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\r\n\t \"username\":\"map3d\",\r\n     \"password\":\"123456\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                JObject msg = JObject.Parse(response.Content);
                return "Bearer " + msg["data"]["token"].ToString();

            }
            catch (Exception)
            {

                return "";
            }
        }

    }
}
