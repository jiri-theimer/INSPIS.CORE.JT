using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridInstanceViewModel:BaseViewModel
    {
        public string entity { get; set; }
        public string entityTitle { get; set; }
        public string prefix { get; set; }
        public int j72id { get; set; }
        public int go2pid { get; set; }
        public int contextmenuflag { get; set; }
        
        public string master_entity { get; set; }
        public int master_pid { get; set; }
        public string master_flag { get; set; }

        public List<NavTab> NavTabs;

        public string go2pid_url_in_iframe { get; set; }

        public string dblclick { get; set; } = "tg_dblclick";
        public PeriodViewModel period { get; set; }
        
    }


    public class NavTab
    {
        
        public string Name { get; set; }
        public string Entity { get; set; }
        public string Url { get; set; }

        public string CssClass { get; set; } = "nav-link text-dark";
       

        public string Badge { get; set; }
        
    }
}
