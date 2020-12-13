using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabForms: BaseViewModel
    {
        public IEnumerable<BO.a11EventForm> lisA11 { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public BO.a10EventType RecA10 { get; set; }
        public int pid { get; set; }

        public bool IsGridView { get; set; }

        public TheGridInput gridinput { get; set; }
    }
}
