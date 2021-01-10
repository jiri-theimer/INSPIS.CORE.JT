using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a01CreateIAudit: BaseViewModel
    {
        //společné všem akcím v IA:
        public int a03ID { get; set; }
        public string Institution { get; set; }

        public DateTime? a01DateFrom { get; set; }
        public DateTime? a01DateUntil { get; set; }

        public List<BO.a41PersonToEvent> lisA41 { get; set; }

        public int SelectedJ02ID { get; set; }
        public string SelectedPerson { get; set; }
        public string MasterPrefixComboJ02 { get; set; }
        public int MasterPidComboJ02 { get; set; }



        //individuální nastavení nadřízené akce v IA:
        public BO.a01Event RecA01_Master { get; set; }
        public int a10ID_Master { get; set; }
        public string a10Name_Master { get; set; }
        public BO.a10EventType RecA10_Master { get; set; }
        public IEnumerable<BO.a12ThemeForm> lisA12_Master { get; set; }
        public List<BO.a11EventForm> lisA11_Master { get; set; }

        public int a46ID { get; set; }

        //individuální nastavení první podřízené akce v IA:
        public BO.a01Event RecA01_Slave1 { get; set; }
        public int a10ID_Slave1 { get; set; }
        public string a10Name_Slave1 { get; set; }
        public BO.a10EventType RecA10_Slave1 { get; set; }
        public IEnumerable<BO.a12ThemeForm> lisA12_Slave1 { get; set; }
        public List<BO.a11EventForm> lisA11_Slave1 { get; set; }


        //individuální nastavení druhé podřízené akce v IA:
        public BO.a01Event RecA01_Slave2 { get; set; }
        public int a10ID_Slave2 { get; set; }
        public string a10Name_Slave2 { get; set; }
        public BO.a10EventType RecA10_Slave2 { get; set; }
        public IEnumerable<BO.a12ThemeForm> lisA12_Slave2 { get; set; }
        public List<BO.a11EventForm> lisA11_Slave2 { get; set; }
    }
}
