using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a03Record:BaseRecordViewModel
    {
        public BO.a03Institution Rec { get; set; }
        public string FounderName { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
    }
}
