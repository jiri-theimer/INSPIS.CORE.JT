using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabSchoolA57: BaseViewModel
    {
        public int a03ID { get; set; }
        public int SelectedA57ID { get; set; }
        public IEnumerable<BO.a57AutoEvaluation> lisA57 { get; set; }
        public BO.j04UserRole RecJ04 { get; set; }  //role uživatele ve škole
        public BO.a03Institution RecA03 { get; set; }

        public int j72id { get; set; }
        public int go2pid { get; set; }
        public string FixedGridColumns { get; set; }        
        public bool IsAllowCreateA01 { get; set; }

        public TheGridInput gridinput { get; set; }
    }
}
