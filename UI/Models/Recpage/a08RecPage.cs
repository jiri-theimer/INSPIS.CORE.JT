using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class a08RecPage: BaseViewModel
    {
        public BO.a08Theme Rec { get; set; }

        public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
