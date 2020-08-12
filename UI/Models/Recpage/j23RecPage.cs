using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class j23RecPage:BaseViewModel
    {
        public BO.j23NonPerson Rec { get; set; }

        public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
