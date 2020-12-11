using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FsmViewModel:BaseViewModel
    {
        public string entity { get; set; }
        public BO.myQuery myQueryGrid { get; set; }
        public string ExtendPagerHtml { get; set; }
        
        public string entityTitle { get; set; }
        public string prefix { get; set; }
        public int j72id { get; set; }
        public int go2pid { get; set; }
        
        public string master_entity { get; set; }
        public int master_pid { get; set; }
        

        public List<NavTab> NavTabs;

        public string go2pid_url_in_iframe { get; set; }

        public PeriodViewModel period { get; set; } //fixní filtr v horním pruhu

        public string FilterA10ID { get; set; }     //fixní filtr v horním pruhu
        public string FilterA10Name { get; set; }   //fixní filtr v horním pruhu
        public string FilterA08ID { get; set; }     //fixní filtr v horním pruhu
        public string FilterA08Name { get; set; }   //fixní filtr v horním pruhu
        public string FilterMyInvolvement { get; set; } //fixní filtr v horním pruhu
        public string FilterH07ID { get; set; }     //fixní filtr v horním pruhu
        public string FilterH07Name { get; set; }   //fixní filtr v horním pruhu


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
