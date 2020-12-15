using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridInput
    {
        public string entity { get; set; }
        public int j72id { get; set; }
        public int go2pid { get; set; }
        public string master_entity { get; set; }
        public BO.baseQuery query { get; set; }
        public string oncmclick { get; set; } = "tg_cm(event)";
        public string ondblclick { get; set; } = "tg_dblclick";
        public string controllername { get; set; } = "TheGrid";
        public string fixedcolumns { get; set; }
    
        public string extendpagerhtml { get; set; }
        public string viewstate { get; set; }
    }
}
