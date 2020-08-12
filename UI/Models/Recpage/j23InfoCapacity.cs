using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class j23InfoCapacity:BaseViewModel
    {
        public bool IsHover { get; set; }
        public int pid { get; set; }
        public BO.j23NonPerson Rec { get; set; }
        
        public IEnumerable<BO.a38NonPersonEventPlan> lisA38 { get; set; }
        public IEnumerable<BO.a01Event> lisA01 { get; set; }


        public string TagHtml { get; set; }
    }
}
