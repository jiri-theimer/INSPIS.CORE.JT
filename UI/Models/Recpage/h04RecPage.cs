using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class h04RecPage: BaseViewModel
    {
        public bool IsHover { get; set; }
        public BO.h04ToDo Rec { get; set; }

        public BO.a01Event RecA01 { get; set; }
        
        public BO.h07ToDoType RecH07 { get; set; }       
        public string TagHtml { get; set; }

        public int pid { get; set; }

        public string MenuCode { get; set; }

        public IEnumerable<BO.j02Person> lisJ02 { get; set; }
    }
}
