using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a08Record:BaseRecordViewModel
    {
        public BO.a08Theme Rec { get; set; }
        public List<BO.a12ThemeForm> lisA12 { get; set; }

        public int SelectedF06ID { get; set; }
        public string SelectedForm { get; set; }
    }
}
