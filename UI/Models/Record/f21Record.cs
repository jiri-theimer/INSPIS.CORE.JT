using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class f21Record:BaseRecordViewModel
    {
        public BO.f21ReplyUnit Rec { get; set; }

        public IEnumerable<BO.f19Question> lisF19 { get; set; }
        public IEnumerable<BO.f22ReplySet> lisF22 { get; set; }
    }
}
