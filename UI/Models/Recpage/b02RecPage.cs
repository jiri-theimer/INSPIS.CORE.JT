using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class b02RecPage:BaseViewModel
    {
        public BO.b02WorkflowStatus Rec { get; set; }

        public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
