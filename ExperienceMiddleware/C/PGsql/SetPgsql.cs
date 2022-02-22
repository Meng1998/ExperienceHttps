using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceMiddleware.C.PGsql
{
    class SetPgsql
    {
        public  String GetInfo(JObject msg)
        {
            String sql = "SELECT * FROM device_info LIMIT 10";
            DataSet data = new PGoperation().ExecuteQuery(sql);
            return JsonConvert.SerializeObject(new
            {
                dirId = msg["dirId"],
                list = data.Tables[0]
            }) ;
        }

        public String GetCategory(JObject msg)
        {
            String sql = "SELECT * FROM \"device_category\"";
            DataSet data = new PGoperation().ExecuteQuery(sql);
            return JsonConvert.SerializeObject(new
            {
                dirId = msg["dirId"],
                list = data.Tables[0]
            });
        }

        public static void GetCategoData( )
        {
            String sql = "SELECT * FROM \"device_camera\"";
            DataTable data = new PGoperation().ExecuteQueryData(sql);
          
        }
    }
}
