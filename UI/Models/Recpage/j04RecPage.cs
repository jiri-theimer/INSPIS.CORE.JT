using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class j04RecPage:BaseViewModel
    {
        public BO.j04UserRole Rec { get; set; }

        public int pid { get; set; }

        public IEnumerable<BO.j05Permission> lisJ05 { get; set; }
        public IEnumerable<BO.j08UserRole_EventType> lisJ08 { get; set; }

    }
}
