using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a01Record:BaseRecordViewModel
    {
        public BO.a01Event Rec { get; set; }
        public BO.a10EventType RecA10 { get; set; }
        public int a10ID { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
    }
}
