using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabSchoolGrid:BaseViewModel
    {
        public int a10ID { get; set; }
        public int a03ID { get; set; }
        public BO.j04UserRole RecJ04 { get; set; }  //role uživatele ve škole
        public BO.a03Institution RecA03 { get; set; }

        public int j72id { get; set; }
        public int go2pid { get; set; }
        public string GridColumns { get; set; }
        public PeriodViewModel period { get; set; } //fixní filtr v horním pruhu
        public bool IsAllowCreateA01 { get; set; }
    }
}
