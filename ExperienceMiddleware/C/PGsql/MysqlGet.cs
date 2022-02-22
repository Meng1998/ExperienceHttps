using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceMiddleware.C.PGsql
{
    public class MysqlGet
    {

      
    }
    class prisonZFlist
    {
        public String priName { get; set; }//监区
        public String totle { get; set; }//人数
    }

    class vehicllist
    {
        public String carcode { get; set; }//车牌号
        public String entertime { get; set; }//进监时间
        public String outtime { get; set; }//出监时间
        public String destination { get; set; }//目的地

    }

    public class AlarmTotals
    {
        public String time { get; set; }
        public Int32 total { get; set; }
    }
}
