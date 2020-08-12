using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class a11RecPage:BaseViewModel
    {
        public BO.a11EventForm Rec { get; set; }
        public BO.a01Event RecA01 { get; set; }

        public int pid { get; set; }

        public BO.a01EventPermission RecPermission { get; set; }
    }
}
