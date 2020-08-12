using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class f26Record:BaseRecordViewModel
    {
        public BO.f26BatteryBoard Rec { get; set; }

        public int f06ID { get; set; }

        public BO.f06Form RecF06 { get; set; }
    }
}
