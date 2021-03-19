using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class h07RecPage:BaseViewModel
    {
        public BO.h07ToDoType Rec { get; set; }

        public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
