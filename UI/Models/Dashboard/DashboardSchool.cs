using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Dashboard
{
    public class DashboardSchool: BaseViewModel
    {
        public BO.j02Person Rec { get; set; }
        public BO.a03Institution RecA03 { get; set; }
        public BO.j04UserRole RecJ04 { get; set; }
        public int pid { get; set; }

        public int a03ID { get; set; }
       
        
        
        public IEnumerable<BO.h11NoticeBoard> lisH11 { get; set; }

        public IEnumerable<BO.a03Institution> lisA03 { get; set; }

        

        public bool IsAllowCreateA01 { get; set; }

        public string GridColumns { get; set; }

        public IEnumerable<BO.a39InstitutionPerson> lisA39 { get; set; }

        public List<NavTab> NavTabs { get; set; }
        public string DefaultNavTabUrl { get; set; }
    }
}
