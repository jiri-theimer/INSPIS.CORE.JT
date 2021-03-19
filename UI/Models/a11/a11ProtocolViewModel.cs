using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a11ProtocolViewModel: BaseViewModel
    {
        public BO.a11EventForm Rec { get; set; }
        public int a11ID { get; set; }
        public IEnumerable<BO.f32FilledValueExtened> lisF32 { get; set; }
    }
}
