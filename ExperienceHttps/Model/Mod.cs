using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.Model
{
    public class Mod
    {
        public String type_name { get; set; }
    }
    public class Orgzind
    {
        public String id { get; set; }
        public String pid { get; set; }
        public String organiz_name { get; set; }
        public String organiz_id { get; set; }
        public List<camera> order_num { get; set; }
    }
    public class camera
    {
        public String id { get; set; }
        public String type_name { get; set; }
        public String device_code { get; set; }
        public String device_name { get; set; }
        public String device_info { get; set; }
        public Int32 order { get; set; }
        public String orgin_id { get; set; }
    }

    public class allGroup
    {
        public String granularity { get; set; }
        public String statTime { get; set; }
    }

    public class crossRecords
    {
        public String startTime { get; set; }
        public String endTime { get; set; }
        public Int32 pageNo { get; set; }
        public Int32 pageSize { get; set; }
    }
    public class peopleRecords
    {
        public String ids { get; set; }
        public String granularity { get; set; }
        public String startTime { get; set; }
        public String endTime { get; set; }
    }
}
