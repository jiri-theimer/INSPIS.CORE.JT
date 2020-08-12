using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a41AppendViewModel:BaseViewModel
    {
        public int a01ID { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public List<BO.a41PersonToEvent> lisA41 { get; set; }
        public IEnumerable<BO.a41PersonToEvent> lisA41Saved { get; set; }

        public int SelectedJ02ID { get; set; }
        public string SelectedPerson { get; set; }
    }
}
