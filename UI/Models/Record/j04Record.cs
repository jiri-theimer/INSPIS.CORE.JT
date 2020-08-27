using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class j04Record: BaseRecordViewModel
    {
        public BO.j04UserRole Rec { get; set; }

        public List<int> SelectedJ05IDs_EPIS1 { get; set; }
        public List<int> SelectedJ05IDs_EPIS2 { get; set; }
        public List<int> SelectedJ05IDs_PORTAL { get; set; }
        public IEnumerable<BO.j05Permission> lisJ05_EPIS1 { get; set; }
        public IEnumerable<BO.j05Permission> lisJ05_EPIS2 { get; set; }
        public IEnumerable<BO.j05Permission> lisJ05_PORTAL { get; set; }

        public List<BO.j08UserRole_EventType> lisJ08 { get; set; }

    }
}
