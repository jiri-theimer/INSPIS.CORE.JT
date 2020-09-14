using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a42RecPage: BaseViewModel
    {
        public BO.a42Qes Rec { get; set; }

        
        public int pid { get; set; }
        
        public IEnumerable<BO.a01Event> lisA01 { get; set; }
        public IEnumerable<BO.x40MailQueue> lisX40 { get; set; }
        public string TagHtml { get; set; }
    }
}
