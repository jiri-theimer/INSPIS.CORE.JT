using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a25Record:BaseRecordViewModel
    {
        public BO.a25EventFormGroup Rec { get; set; }

        public int a01ID { get; set; }
        public BO.a01Event RecA01 { get; set; }

        public List<int> SelectedA11IDs { get; set; }
        public IEnumerable<BO.a11EventForm> lisA11 { get; set; }
    }
}
