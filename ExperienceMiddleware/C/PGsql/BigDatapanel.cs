using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceMiddleware.C.PGsql
{
    public class BigDatapanel
    {
        public static MySqlConnection conn;
        public static String ConnStr = "";
        public static List<AlarmTotals> alarmArray;
        public BigDatapanel()
        {
            //string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            //string week = Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();


            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
            .AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            ConnStr = configuration["MysqlConnection"];
            conn = new MySqlConnection(ConnStr); //建立连接
            conn.Open();
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





        public MySqlDataReader GetReader(String sqlstr)
        {
            //二、生成命令。

            MySqlCommand cmd = new MySqlCommand(sqlstr, conn); //生成命令构造器对象。
            //三、查询结果。
            return cmd.ExecuteReader();
        }

        public async void Alarmlist()
        {
            await Task.Run(() =>
            {
                try
                {

                    String sqlStr = $"select b.name as GJXX,a.created as FS_RQ from xjjy_ztk.t_dw_alarm a  left join xjjy_ztk.t_dw_alarm_type b on a.type_id = b.id where DATE_SUB(CURDATE(), INTERVAL 7 DAY) <= a.created";
                    MySqlCommand cmd = new MySqlCommand(sqlStr, conn); //生成命令构造器对象。
                                                                       //三、查询结果。
                    MySqlDataReader rdr = cmd.ExecuteReader();
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
                    while (rdr.Read())
                    {
                        String num = rdr[1].ToString().Split(' ')[0].ToString();
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
                    Console.WriteLine("同步失败");
                }
            });


        }


    }
}
