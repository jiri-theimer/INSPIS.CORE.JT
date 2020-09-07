using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabSouvisejici: BaseViewModel
    {
        public IEnumerable<BO.a24EventRelation> lisA24 { get; set; }
        public BO.a01Event RecA01 { get; set; }        
        public int pid { get; set; }

        public bool IsGridView { get; set; }
    }
}
