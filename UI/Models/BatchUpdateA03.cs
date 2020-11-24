using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class BatchUpdateA03: BaseViewModel
    {
        public string pids { get; set; }
        public int SelectedA21ID { get; set; }
        public string SelectedA21Name { get; set; }

        public int SelectedA05ID { get; set; }
        public string SelectedA05Name { get; set; }

        public int SelectedA28ID { get; set; }
        public string SelectedA28Name { get; set; }

        public int SelectedSupervisoryID { get; set; }
        public string SelectedSupervisory { get; set; }

        public int SelectedParentFlag { get; set; } = -1;

        public IEnumerable<BO.a03Institution> lisA03 { get; set; }
    }
}
