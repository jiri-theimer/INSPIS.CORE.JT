using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class BatchUpdateA01: BaseViewModel
    {
        public string pids { get; set; }
        public int SelectedA10ID { get; set; }
        public string SelectedA10Name { get; set; }

        public int SelectedA08ID { get; set; }
        public string SelectedA08Name { get; set; }

        public int SelectedB02ID { get; set; }
        public string SelectedB02Name { get; set; }

        public string WorkflowComment { get; set; }

        public IEnumerable<BO.a01Event> lisA01 { get; set; }

        public int CommonB01ID { get; set; }

        public DateTime? a01DateFrom { get; set; }
        public DateTime? a01DateUntil { get; set; }
    }
}
