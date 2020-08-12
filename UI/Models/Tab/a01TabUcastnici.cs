using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabUcastnici:BaseViewModel
    {
        public IEnumerable<BO.a41PersonToEvent> lisA41 { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public int pid { get; set; }
    }
}
