using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class a10RecPage:BaseViewModel
    {
        public BO.a10EventType Rec { get; set; }

        public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
