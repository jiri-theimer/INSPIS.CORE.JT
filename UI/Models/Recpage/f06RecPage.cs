using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class f06RecPage: BaseViewModel
    {
        public BO.f06Form Rec { get; set; }

      public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
