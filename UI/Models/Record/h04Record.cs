using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class h04Record: BaseRecordViewModel
    {
        public BO.h04ToDo Rec { get; set; }
        public int a01ID { get; set; }

        public string j02IDs { get; set; }

        public int SelectedH07ID { get; set; }
        public string SelectedH07Name { get; set; }
        public BO.h07ToDoType RecH07 { get; set; }

        public int SelectedJ02ID { get; set; }
        public string SelectedPerson { get; set; }

        public string j11IDs { get; set; }

        public int SelectedJ11ID { get; set; }
        public string SelectedTeam { get; set; }

        public bool IsDefineCapacityPlan { get; set; }
        public bool IsNotifyAfterSave { get; set; }
    }
}
