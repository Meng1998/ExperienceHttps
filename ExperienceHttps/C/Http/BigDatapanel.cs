using Dapper;
using ExperienceHttps.Model;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.C.Http
{
    public class BigDatapanel
    {
        public static MySqlConnection conn;
        public static String ConnStr = "";
        public static List<AlarmTotals> alarmArray;
        public static ApiResult vehRes;
        public static String[] datestrvehicle = new String[7];
        public static String[] datestrpeople = new String[7];

        public BigDatapanel()
        {
            //string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            //string week = Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();
            #region 西郊

            //var builder = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
            //.AddJsonFile("appsettings.json");
            //IConfiguration configuration = builder.Build();
            //ConnStr = configuration["MysqlConnection"];
            ////告警统计
            //Alarmlist();
            #endregion

            #region 南山陵园
            //车流统计
            NSLYvehicleInfo();
            //客流统计
            NSLYvpeopleInfo();
            #endregion
        }

        public JObject GetDataSet(String sqlstr)
        {
            //二、生成命令。

            MySqlCommand cmd = new MySqlCommand(sqlstr, conn); //生成命令构造器对象。
            //三、查询结果。
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<prisonZFlist> list = new List<prisonZFlist>();
            try
            {

                while (rdr.Read())//Read()函数设计的时候是按行查询，查完一行换下一行。
                {
                    //string n1 = rdr.GetName(0).ToString();
                    //string n = rdr.GetName(1).ToString();

                    list.Add(new prisonZFlist()
                    {
                        priName = rdr[0].ToString(),
                        totle = rdr[1].ToString()
                    });
                }
                return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(new { list }));
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static DataSet GetDataMy(string sqlstr)
        {
            MySqlDataAdapter da = new MySqlDataAdapter();//实例化sqldataadpter
            MySqlCommand cmd1 = new MySqlCommand(sqlstr);
            da.SelectCommand = cmd1;//设置为已实例化SqlDataAdapter的查询命令
            DataSet ds = new DataSet();//实例化dataset
            da.Fill(ds);//把数据填充到datase
            return ds;
        }

        public static DataSet ExecuteQuery(string sqrstr)
        {

            MySqlConnection conn = new MySqlConnection(ConnStr);

            DataSet ds = new DataSet();
            try
            {
                using (MySqlDataAdapter sqldap = new MySqlDataAdapter(sqrstr, conn))
                {
                    sqldap.Fill(ds);
                }
                return ds;
            }
            catch (System.Exception ex)
            {
                conn.Close();
                Log.Debug("数据库执行："+ sqrstr + "语句异常；" + ex.ToString());
                return ds;
            }
        }



        public static MySqlDataReader GetReader(String sqlstr)
        {
            //二、生成命令。
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlstr, conn); //生成命令构造器对象。
            //三、查询结果。
            return cmd.ExecuteReader();
        }

        public static async void Alarmlist()
        {
            await Task.Run(() =>
            {
                try
                {

                    String sqlStr = $"select b.name as GJXX,a.created as FS_RQ from xjjy_ztk.t_dw_alarm a  left join xjjy_ztk.t_dw_alarm_type b on a.type_id = b.id where DATE_SUB(CURDATE(), INTERVAL 6 DAY) <= a.created;";

                    DataSet data = ExecuteQuery(sqlStr);

                    //MySqlConnection conn = new MySqlConnection(ConnStr);
                    //conn.Open();
                    //MySqlCommand cmd = new MySqlCommand(sqlStr, conn); //生成命令构造器对象。
                    //                                                   //三、查询结果。
                    //MySqlDataReader rdr = cmd.ExecuteReader();
                    alarmArray = new List<AlarmTotals>();
                    //Dictionary<String, Int32> pairs = new Dictionary<string, int>();
                    //for (int i = 0; i > -7; i--)
                    //{
                    //    pairs.Add(DateTime.Now.AddDays(i).ToShortDateString(), 0);
                    //}

                    String[] Timeary = new String[7] {
                DateTime.Now.AddDays(0).ToShortDateString(),
                DateTime.Now.AddDays(-1).ToShortDateString() ,
                DateTime.Now.AddDays(-2).ToShortDateString() ,
                DateTime.Now.AddDays(-3).ToShortDateString() ,
                DateTime.Now.AddDays(-4).ToShortDateString() ,
                DateTime.Now.AddDays(-5).ToShortDateString() ,
                DateTime.Now.AddDays(-6).ToShortDateString()
            };
                    Int32[] totalary = new Int32[7] { 0, 0, 0, 0, 0, 0, 0 };
                    Int32 Atol = 0, Btol = 0, Ctol = 0, Dtol = 0, Etol = 0, Ftol = 0, Gtol = 0;

                    int i = 0;
                    foreach (DataRow item in data.Tables[0].Rows)
                    {
                        String num = item["FS_RQ"].ToString().Split(' ')[0].ToString();
                        if (Timeary[0] == num)
                        {
                            totalary[0]++;
                        }
                        else if (Timeary[1] == num)
                        {
                            totalary[1]++;
                        }
                        else if (Timeary[2] == num)
                        {
                            totalary[2]++;
                        }
                        else if (Timeary[3] == num)
                        {
                            totalary[3]++;
                        }
                        else if (Timeary[4] == num)
                        {
                            totalary[4]++;
                        }
                        else if (Timeary[5] == num)
                        {
                            totalary[5]++;
                        }
                        else if (Timeary[6] == num)
                        {
                            totalary[6]++;
                        }
                    }
                    //while (rdr.Read())
                    //{

                    //}
                    Int32 a = 0;
                    foreach (var item in Timeary)
                    {
                        alarmArray.Add(new AlarmTotals()
                        {
                            time = item,
                            total = totalary[a]
                        });
                        a++;
                    }
                }
                catch (Exception)
                {
                    Log.Debug("报警数据同步失败");
                }
            });


        }

        static Boolean Isr = true;
        static Int32 index = 6;
        public static async void NSLYvehicleInfo()
        {
            await Task.Run(() =>
            {
                try
                {
                    crossRecords Parameter = new crossRecords();
                    //DateTime a = DateTime.Parse(datestr[0].Split('_')[0]);
                    //DateTime b = DateTime.Parse(DateTime.Now.Date.ToString());
                    //string c = datestr[0];
                    //if (DateTime.Parse(datestr[0].Split('_')[0]) < DateTime.Parse(DateTime.Now.Date.ToString())&& datestr[0]!=null)
                    //{

                    //}
                    for (int i = 6; i > -1; i--)
                    {
                        String start = DateTime.Now.Date.AddDays(-i).ToString();
                        String end = DateTime.Now.Date.AddDays(-(i - 1)).ToString();
                        Parameter.startTime = DateTime.Parse(start).ToString("s") + "+08:00";
                        Parameter.endTime = DateTime.Parse(end).ToString("s") + "+08:00";
                        Parameter.pageNo = 1;
                        Parameter.pageSize = 800;
                        var Pam = new
                        {
                            MsgType = "Hik",
                            dirId = Guid.NewGuid().ToString(),
                            MsgId = 8,
                            Parameter,
                            eventType = "Submit"//事件名称代码
                        };

                        vehRes = Mqtt.Mqttclient.AyXhGetmout(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(Pam)));
                        datestrvehicle[i] = start.Split(' ')[0] + "_" + vehRes.Data["data"]["total"].ToString();
                    }
                }
                catch (Exception ex)
                {

                    Log.Debug("车流请求异常:" + ex.ToString());
                }

            });
        }

        public static async void NSLYvpeopleInfo()
        {
            await Task.Run(() =>
            {
                try
                {
                    peopleRecords Parameter = new peopleRecords();
                    //DateTime a = DateTime.Parse(datestr[0].Split('_')[0]);
                    //DateTime b = DateTime.Parse(DateTime.Now.Date.ToString());
                    //string c = datestr[0];
                    //if (DateTime.Parse(datestr[0].Split('_')[0]) < DateTime.Parse(DateTime.Now.Date.ToString())&& datestr[0]!=null)
                    //{

                    //}
                    Parameter.ids = "25764725-ad6b-4694-89ab-fa5acfab41c9,57ff6e41-68f0-4043-b4e8-6b464a353d06,263c0acc-f8a8-4b8c-8e74-8f4eb93e9a71,6206036c-be2a-4b36-bdc2-3fb2a73eb9d4";//西小门,大门总,右边大门,中间大门6206036c-be2a-4b36-bdc2-3fb2a73eb9d4
                    Parameter.granularity = "hourly";
                    for (int i = 6; i > -1; i--)
                    {
                        String start = DateTime.Now.Date.AddDays(-i).ToString();
                        String end = DateTime.Now.Date.AddDays(-(i - 1)).AddSeconds(-1).ToString();
                        Int32 totel = 0;
                        Parameter.startTime = DateTime.Parse(start).ToString("s") + "+08:00";
                        Parameter.endTime = DateTime.Parse(end).ToString("s") + "+08:00";
                        var Pam = new
                        {
                            MsgType = "Hik",
                            dirId = Guid.NewGuid().ToString(),
                            MsgId = 9,
                            Parameter,
                            eventType = "Submit"//事件名称代码
                        };

                        vehRes = Mqtt.Mqttclient.AyXhGetmout(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(Pam)));
                        foreach (var item in vehRes.Data["data"]["list"])
                        {
                            totel += Int32.Parse(item["flowInNum"].ToString());
                        }
                        datestrpeople[i] = start.Split(' ')[0] + "_" + totel;
                    }
                }
                catch (Exception ex)
                {

                    Log.Debug("车流请求异常:" + ex.ToString());
                }

            });
        }


    }

    class prisonZFlist
    {
        public String priName { get; set; }//监区
        public String totle { get; set; }//人数
    }

    class vehicllist
    {
        public String carcode { get; set; }//车牌号
        public String inname { get; set; }//进监时间
        public String outtime { get; set; }//出监时间
        public String destination { get; set; }//目的地

    }

    public class AlarmTotals
    {
        public String time { get; set; }
        public Int32 total { get; set; }
    }


}
