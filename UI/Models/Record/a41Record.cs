using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a41Record: BaseRecordViewModel
    {
        public BO.a41PersonToEvent Rec { get; set; }
        public int a01ID { get; set; }
        public BO.a01Event RecA01 { get; set; }

        public string Person { get; set; }
    }
}
