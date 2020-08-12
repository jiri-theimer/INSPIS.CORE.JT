using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class f22Record:BaseRecordViewModel
    {
        public BO.f22ReplySet Rec { get; set; }
        public string f21IDs { get; set; }

        public int SelectedF21ID { get; set; }
        public string SelectedF21Name { get; set; }
    }
}
