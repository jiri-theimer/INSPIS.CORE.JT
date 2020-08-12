using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class j02InfoCapacity : BaseViewModel
    {
        public bool IsHover { get; set; }
        public int pid { get; set; }
        public BO.j02Person Rec { get; set; }
        public BO.j03User RecJ03 { get; set; }
        public BO.a04Inspectorate RecA04 { get; set; }
        public IEnumerable<BO.a35PersonEventPlan> lisA35 {get;set;}
        public IEnumerable<BO.a01Event> lisA01 { get; set; }

       
        public string TagHtml { get; set; }
    }
}
