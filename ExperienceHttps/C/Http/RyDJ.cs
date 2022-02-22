using ExperienceHttps.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using Serilog;

namespace ExperienceHttps.C.Http
{
    public class RyDJ
    {

        /// <summary>
        /// 姓名查找
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<ApiResult> RyGetname(String name)
        {
            return await Task<ApiResult>.Run(() =>
            {
                try
                {
                    String url = "http://172.30.248.113:7001/api/user/search?key=" + name;
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(cookies.DWhttpGet(url, cookies.Yrtoken())["data"]))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("姓名查找接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { err = "获取异常" }))
                    };
                }
            });
        }

        /// <summary>
        /// 获取区域人数计总
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ApiResult> Rylocation(Int32 id)
        {
            return await Task<ApiResult>.Run(() =>
            {
                String url = "http://172.30.248.113:7001/api/user/real?map=" + id;
                try
                {
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(cookies.DWhttpGet(url, cookies.Yrtoken())["data"]))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("获取区域人数计总接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { err = "获取异常" }))
                    };

                }
            });
        }

        //重点罪犯数量分布
        public static async Task<ApiResult> PrisonKeyList()
        {
            //var data = BigDatapanel.conn.Query();

            return await Task<ApiResult>.Run(() =>
            {
                String sqlStr = "select org.NAME as \"priName\",count(distinct  wx.pr_name) as \"totle\" from xjjy_ztk.t_danger_record wx left join cesauth.t_org org on wx.ar_code = org.CODE where wx.ar_code like '3313%'   and wx.is_danger = '1' group by org.NAME,org.CODE order by org.CODE; ";
                try
                {

                    MySqlDataReader rdd = BigDatapanel.GetReader(sqlStr);
                    DataSet data = BigDatapanel.ExecuteQuery(sqlStr);
                    // List<prisonZFlist> list = new List<prisonZFlist>();

                    //while (rdd.Read())//Read()函数设计的时候是按行查询，查完一行换下一行。
                    //{
                    //    //string n1 = rdr.GetName(0).ToString();
                    //    //string n = rdr.GetName(1).ToString();

                    //    list.Add(new prisonZFlist()
                    //    {
                    //        priName = rdd[0].ToString(),
                    //        totle = rdd[1].ToString()
                    //    });
                    //}
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { list = data.Tables[0] }))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("重点罪犯数量分布接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>("Mysql Connection error")
                    };
                }
            });
        }
        //监狱态势
        public static async Task<ApiResult> PrisonSituat()
        {
            //var data = BigDatapanel.conn.Query();
            //Thread.Sleep(700);

            return await Task<ApiResult>.Run(() =>
            {
                String sqlStr = " SELECT total as \"total\"  ,total -`out`-move as \"yd\"  ,`real` as \"sj\" from xjjy_ztk.t_dw_roll where creator = 1 order by created desc limit 1; ";
                //String sqlWdd = " select count(1) as WDDRS from xjjy_ztk.t_dw_roll_detail where roll_id = (select t_dw_roll.id from xjjy_ztk.t_dw_roll order by created desc limit 1)  and state = '1' and memo in (2, 5, 6, 7,8,9); ";
                try
                {
                    

                    Int32 total = 0, yd = 0, sj = 0, wdd = 0;
                    Console.WriteLine("监狱事态");
                    DataSet data = BigDatapanel.ExecuteQuery(sqlStr);
                    //DataSet data1 = BigDatapanel.ExecuteQuery(sqlWdd);
                    Console.WriteLine("数据已查到：" + data.Tables[0].Rows.Count);
                    total = Int32.Parse(data.Tables[0].Rows[0]["total"].ToString());
                    try
                    {
                        yd = Convert.ToInt32(data.Tables[0].Rows[0]["yd"].ToString());
                    }
                    catch (Exception)
                    {
                        yd = 0;
                    }
                    sj = Int32.Parse(data.Tables[0].Rows[0]["sj"].ToString());
                    wdd = yd - sj;
                    if (wdd<0)
                    {
                        wdd = -wdd;
                    }
                    Console.WriteLine("结果：" + total + "\n" + yd + "\n" + sj + "\n" + wdd);
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { total, yd, sj, wdd }))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("监狱态势接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>("Mysql Connection error")
                    };
                }
            });
        }


        //车辆人员进出记录
        public static async Task<ApiResult> Prisonvehicle()
        {

            return await Task<ApiResult>.Run(() =>
            {

                String sqlStr = "select a.carnumber as \"carcode\" ,a.inname as \"inname\" ,a.policeintime as \"starttime\",a.destination as \"destination\" from xjjy_ztk.t_ab_wlcl a where date_format(updatetime, '%Y-%m-%d') = date_format(curdate(), '%Y-%m-%d') and policeouttime is null order by a.updatetime desc; ";

                String sqlWr = "select a.name as \"wrName\"  ,a.policeintime as \"starttime\"  ,a.policeouttime as \"outtime\"  ,a.destination as \"destination\" from xjjy_ztk.t_ab_wlry a where date_format(updatetime, '%Y-%m-%d') = date_format(curdate(), '%Y-%m-%d') order by a.updatetime desc; ";
                String sqlMj = "select a.name as \"mjName\"  ,a.policeintime as \"starttime\" ,a.policeouttime as \"outtime\"  from xjjy_ztk.t_ab_mjjc a where date_format(updatetime, '%Y-%m-%d') = date_format(curdate(), '%Y-%m-%d') order by a.updatetime desc; ";

                try
                {
                    DataSet data = BigDatapanel.ExecuteQuery(sqlStr + sqlWr + sqlMj);
                    //MySqlDataReader rdd = BigDatapanel.GetReader(sqlStr);
                    //Console.WriteLine("D");
                    // List<vehicllist> list = new List<vehicllist>();
                    //while (rdd.Read())
                    //{
                    //    list.Add(new vehicllist()
                    //    {
                    //        carcode = rdd[0].ToString(),
                    //        inname = rdd[1].ToString(),
                    //        outtime = rdd[2].ToString(),
                    //        destination = rdd[3].ToString()
                    //    });
                    //}
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { list = data.Tables }))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("车辆人员进出记录接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>("Mysql Connection error")
                    };
                }
            });
        }

        //报警统计管理
        public static async Task<ApiResult> Alarmrun()
        {
            return await Task<ApiResult>.Run(() =>
            {
                try
                {
                    var data = new
                    {
                        BigDatapanel.alarmArray
                    };
                    BigDatapanel.Alarmlist();
                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { data }))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("报警统计管理接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>("Mysql Connection error")
                    };
                }
            });
        }
        //监内人车数量分布
        public static async Task<ApiResult> PrisonPeopleList()
        {
            Thread.Sleep(1000);
            return await Task<ApiResult>.Run(() =>
            {
                Int32 Incustody = 0;
                Int32 InMJ = 0;
                Int32 Invehicle = 0;
                Int32 InWLRY = 0;
                //var data = BigDatapanel.conn.Query();
                String sqlStr = "select count(1) as num from xjjy_ztk.t_yz_zfjbxx a left join xjjy_ztk.t_yz_zfdqzt b on a.ZF_BH = b.ZF_BH where a.GYDW = '3313' and b.ZYZT like '10%';";
                String sqlVel = "select a.carnumber as \"车牌号\"  ,a.policeintime as \"进监时间\" ,a.policeouttime as \"出监时间\"  ,a.destination as \"目的地\" from xjjy_ztk.t_ab_wlcl a where date_format(updatetime, '%Y-%m-%d') = date_format(curdate(), '%Y-%m-%d') order by a.policeouttime desc; ";
                //String sqlMj = "select * from xjjy_ztk.v_jq_mjxx;";
                String sqlMj = "select a.name as \"民警姓名\" ,a.policeintime as \"进监时间\"  ,a.policeouttime as \"出监时间\"  from xjjy_ztk.t_ab_mjjc a where date_format(updatetime, '%Y-%m-%d') > date_format(DATE_SUB(NOW(), INTERVAL 4 day), '%Y-%m-%d') and PoliceOutTime is null and PoliceInTime is not null and Name is not null  and MobPhoneNum is not null  order by a.PoliceInTime desc; ";
                String sqlWr = "select a.name as \"外来人员姓名\"  ,a.policeintime as \"进监时间\"  ,a.policeouttime as \"出监时间\"  ,a.destination as \"目的地\" from xjjy_ztk.t_ab_wlry a where date_format(updatetime, '%Y-%m-%d') = date_format(curdate(), '%Y-%m-%d') order by a.policeouttime desc; ";
                try
                {

                    DataSet data = BigDatapanel.ExecuteQuery(sqlStr + sqlVel + sqlMj + sqlWr);
                    Incustody = Convert.ToInt32(data.Tables[0].Rows[0]["num"].ToString());//当日在押
                    Invehicle = data.Tables[1].Rows.Count;//民警
                    InMJ = data.Tables[2].Rows.Count;//车辆
                    InWLRY = data.Tables[3].Rows.Count;//外来人员

                    //MySqlDataReader stody = BigDatapanel.GetReader(sqlStr);
                    //Console.WriteLine("1");
                    //MySqlDataReader mj = BigDatapanel.GetReader(sqlMj);
                    //Console.WriteLine("2");
                    //MySqlDataReader veh = BigDatapanel.GetReader(sqlVel);
                    //Console.WriteLine("3");
                    //MySqlDataReader wlry = BigDatapanel.GetReader(sqlWr);
                    //Console.WriteLine("4");


                    //while (stody.Read())
                    //{
                    //    Incustody = Int32.Parse(stody[0].ToString());
                    //}
                    //while (mj.Read())
                    //{
                    //    InMJ++;
                    //}
                    //while (veh.Read())
                    //{
                    //    Invehicle++;
                    //}
                    //while (wlry.Read())
                    //{
                    //    InWLRY++;
                    //}


                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { Incustody, Invehicle, InMJ, InWLRY }))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("监内人车数量分布接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>(ex.ToString())
                    };
                }
            });
        }

        /// <summary>
        /// 获取对讲设备状态
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<ApiResult> DJstatuc(String name)
        {
            return await Task<ApiResult>.Run(() =>
            {
                String token = cookies.GetRespson("http://172.30.248.15:8280/sys/web/login.do?action=login&password=GECB6FD5YW9WPN6369A7E9E22935F2B&username=admin");
                if (token == "")
                {
                    Log.Debug("token获取接口异常:");

                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { err = "token获取失败" }))
                    };
                }
                String url = $"http://172.30.248.15:8280/terminal/device.do?action=getMainSubDevList&authorization={token}";
                String data = JsonConvert.SerializeObject(new
                {
                    data = new
                    {
                        filter = new
                        {
                            name = name
                        }
                    }
                });
                try
                {
                    String str = "";
                    JToken strmsg = null;
                    JObject msg = cookies.PostRespson(url, data);
                    if (msg["result"]["maindevList"].Count() > 0)
                    {
                        //str = JsonConvert.SerializeObject(new { Data=msg["result"]["maindevList"] });
                        strmsg = msg["result"]["maindevList"];
                    }
                    else
                    {
                        //str = JsonConvert.SerializeObject(new { list msg["result"]["subdevList"] } );
                        strmsg = msg["result"]["subdevList"];
                    }

                    return new ApiResult()
                    {
                        Code = 200,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { strmsg }))
                    };
                }
                catch (Exception ex)
                {
                    Log.Debug("获取对讲设备状态接口异常:" + ex.ToString());
                    return new ApiResult()
                    {
                        Code = 400,
                        Data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { err = "对讲服务异常" }))
                    };
                }


            });
        }
    }
}
