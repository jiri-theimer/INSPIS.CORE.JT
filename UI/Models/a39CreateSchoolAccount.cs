using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a39CreateSchoolAccount: BaseViewModel
    {
        public BO.j02Person RecJ02 { get; set; }
        public int a03ID { get; set; }
        public BO.a03Institution RecA03 { get; set; }

        public string Password { get; set; }
        public string VerifyPassword { get; set; }

        public string SelectedJ04ID { get; set; }
        public IEnumerable<BO.j04UserRole> lisJ04 { get; set; }
        public int j03LangIndex { get; set; }
        public string a39Description { get; set; }
        public bool a39IsAllowInspisWS { get; set; }

        public string SearchLogin { get; set; }
        public int SearchJ03ID { get; set; }
        public BO.j03User SearchRecJ03 { get; set; }
        

    }
}
