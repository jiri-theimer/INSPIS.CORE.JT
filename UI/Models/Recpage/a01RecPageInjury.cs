using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class a01RecPageInjury:a01RecPage
    {
        public IEnumerable<BO.a11EventForm> lisA11 { get; set; }
    }
}
