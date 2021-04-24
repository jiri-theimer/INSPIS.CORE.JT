using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a01CreateA57ViewModel:BaseViewModel
    {
        public BO.a01Event Rec { get; set; }
        public int a57ID { get; set; }
        public BO.a57AutoEvaluation RecA57 { get; set; }
        public int a10ID { get; set; }        
        public BO.a10EventType RecA10 { get; set; }
        public int a08ID { get; set; }
        public BO.a08Theme RecA08 { get; set; }
        public IEnumerable<BO.a12ThemeForm> lisA12 { get; set; }
        public int a03ID { get; set; }
        public BO.a03Institution RecA03 { get; set; }
        
    }
}
