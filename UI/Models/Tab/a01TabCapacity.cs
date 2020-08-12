using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabCapacity: BaseViewModel
    {
        public IEnumerable<BO.a41PersonToEvent> lisA41 { get; set; }
        public IEnumerable<BO.a35PersonEventPlan> lisA35 { get; set; }
        public IEnumerable<BO.a38NonPersonEventPlan> lisA38 { get; set; }
        public IEnumerable<BO.j26Holiday> lisJ26 { get; set; }
        public IEnumerable<BO.h04TodoCapacity> lisH04 { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public BO.a01EventPermission PermA01 { get; set; }
        public int pid { get; set; }

        public List<DateTime> lisDays { get; set; }
       
        public int PracovnichDni { get; set; }

        public bool IsMonthView { get; set; }
        public int CurMonth { get; set; }
        public int CurYear { get; set; }

        public int NextMonth { get; set; }
        public int NextYear { get; set; }
        public int PrevMonth { get; set; }
        public int PrevYear { get; set; }
    }
}
