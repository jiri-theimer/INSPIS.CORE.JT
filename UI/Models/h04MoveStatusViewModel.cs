using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class h04MoveStatusViewModel: BaseViewModel
    {
        public int pid { get; set; }
        public BO.h04ToDo Rec { get; set; }

        public int SelectedH05ID { get; set; }
        public string SelectedH05Name { get; set; }
        public bool IsNotifyAfterSave { get; set; }
    }
}
