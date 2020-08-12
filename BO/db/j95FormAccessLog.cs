using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class j95FormAccessLog
    {
        public int j95ID { get; set; }
        public int j03ID { get; set; }
        public int f32ID { get; set; }
        public int f19ID { get; set; }
        public int a11ID { get; set; }
        public DateTime j95Date { get; set; }
        public string j95URL { get; set; }

        //readonly:
        public string j03Login;
        public string f19Name;
    }
}
