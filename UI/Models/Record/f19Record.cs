using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class f19Record: BaseRecordViewModel
    {
        public BO.f19Question Rec { get; set; }

        public int f06ID { get; set; }
        public int f18ID { get; set; }

        public BO.f06Form RecF06 { get; set; }
        public BO.f18FormSegment RecF18 { get; set; }


        public string f21IDs { get; set; }
        public int SelectedF21ID { get; set; }
        public string SelectedF21Name { get; set; }

        public string UploadGuid { get; set; }

        public IEnumerable<BO.f26BatteryBoard> lisF26 { get; set; }
        public IEnumerable<BO.f25ChessBoard> lisF25 { get; set; }
    }
}
