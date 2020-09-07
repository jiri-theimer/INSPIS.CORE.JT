using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a11ValidateForms: BaseViewModel
    {
        public IEnumerable<BO.a11EventForm> lisA11 { get; set; }
        public BO.a01Event RecA01 { get; set; }       
        public int pid { get; set; }
        public int a01ID { get; set; }

        public List<BO.ItemValidationResult> lisResult { get; set; }
    }
}
