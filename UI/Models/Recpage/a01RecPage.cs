using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class a01RecPage:BaseViewModel
    {
        public BO.a01Event Rec { get; set; }
        
        public BO.b05Workflow_History RecLastEvent { get; set; }
        public BO.j02Person RecIssuer { get; set; }
        public int pid { get; set; }

        public string SearchBox { get; set; }

        public List<NavTab> NavTabs;

        public string DefaultNavTabUrl { get; set; }

        public string TagHtml { get; set; }
    }
}
