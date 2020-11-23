using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace UI.Models
{
    public class a01CreateViewModel:BaseViewModel
    {
        public int CloneByPid { get; set; }
        public BO.a01Event RecCloneByPid { get; set; }
        public int a46ID { get; set; }
        public BO.a01Event Rec { get; set; }
        public int a10ID { get; set; }
        public string a10Name { get; set; }

        public BO.j02Person RecJ02 { get; set; }

        public int j02ID { get; set; }
        public string Person { get; set; }

        public int a03ID { get; set; }
        public string Institution { get; set; }
        public bool IsComboA03 { get; set; }
        public BO.a10EventType RecA10 { get; set; }

        public int ActiveTabIndex { get; set; } = 1;


        public IEnumerable<BO.a12ThemeForm> lisA12 { get; set; }
        public List<BO.a11EventForm> lisA11 { get; set; }


        public int SelectedJ02ID { get; set; }
        public string SelectedPerson { get; set; }
        public string MasterPrefixComboJ02 { get; set; }
        public int MasterPidComboJ02 { get; set; }
        

        public List<BO.a41PersonToEvent> lisA41 { get; set; }

    }
}
