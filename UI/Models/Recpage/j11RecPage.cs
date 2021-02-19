using System;
using System.Collections.Generic;


namespace UI.Models.Recpage
{
    public class j11RecPage: BaseViewModel
    {
        public BO.j11Team Rec { get; set; }

        public int pid { get; set; }

        public IEnumerable<BO.j02Person> lisJ02 { get; set; }
    }
}
