using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class SelectFunction: BaseViewModel
    {
        public IEnumerable<BO.x27EvalFunction> lisX27 { get; set; }

        public string ElementID { get; set; }
    }
}
