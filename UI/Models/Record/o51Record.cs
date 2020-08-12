using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class o51Record:BaseRecordViewModel
    {
        public BO.o51Tag Rec { get; set; }
        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
    }
}
