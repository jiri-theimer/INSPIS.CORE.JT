using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a42CompleteJob: BaseViewModel
    {
        public int a42ID { get; set; }
        public BO.a42Qes Rec { get; set; }

        public int CurrentA03ID { get; set; }
        public BO.a03Institution CurrentRecA03 { get; set; }
        
        
    }
}
