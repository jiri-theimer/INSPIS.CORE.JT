using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a11Record:BaseRecordViewModel
    {
        public BO.a11EventForm Rec { get; set; }
        public int a01ID { get; set; }        
        public BO.a01Event RecA01 { get; set; }
        public BO.f06Form RecF06 { get; set; }

       
        public string a37Name { get; set; }
    }
}
