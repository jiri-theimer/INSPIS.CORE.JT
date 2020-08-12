using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class a03RecPage:BaseViewModel
    {
        public BO.a03Institution Rec { get; set; }

        public BO.a03Institution RecFounder { get; set; }

        public string MenuCode { get; set; }

        public int pid { get; set; }

        public string SearchBox { get; set; }

        public List<NavTab> NavTabs;

        public string DefaultNavTabUrl { get; set; }

        public string TagHtml { get; set; }
    }
}
