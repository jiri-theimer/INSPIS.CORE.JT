using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a39SchoolAccount:BaseViewModel
    {
        public BO.a39InstitutionPerson Rec { get; set; }
        public BO.j02Person RecJ02 { get; set; }
        public BO.j03User RecJ03 { get; set; }

        public BO.a03Institution RecA03 { get; set; }
        public int a39ID { get; set; }
        public string Person { get; set; }  //kvůli combo
        public string SelectedJ04ID { get; set; }
        public IEnumerable<BO.j04UserRole> lisJ04 { get; set; }

        public bool IsNotSchoolAccount { get; set; }
    }
}
