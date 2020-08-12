using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x31Record:BaseRecordViewModel
    {
        public BO.x31Report Rec { get; set; }
        public string x32IDs { get; set; }
        public string x32Names { get; set; }

        public string j04IDs { get; set; }
        public string j04Names { get; set; }
    }
}
