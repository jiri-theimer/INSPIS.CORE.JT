using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Dashboard
{
    public class DashboardSchool: BaseViewModel
    {
        public BO.a03Institution RecA03 { get; set; }
        public BO.j04UserRole RecJ04 { get; set; }
        
        public int a03ID { get; set; }
       
        public string a10Name { get; set; }
        public int a10ID { get; set; }

        
        public IEnumerable<BO.h11NoticeBoard> lisH11 { get; set; }

        public IEnumerable<BO.a03Institution> lisA03 { get; set; }

        public List<BO.a10EventType> lisA10 { get; set; }

        public bool IsAllowCreateA01 { get; set; }
    }
}
