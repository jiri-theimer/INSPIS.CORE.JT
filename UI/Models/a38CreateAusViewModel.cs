using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models.Tab;

namespace UI.Models
{
    public class a38CreateAusViewModel: BaseViewModel
    {
        public IEnumerable<BO.j25NonPersonPlanReason> lisJ25 { get; set; }
        public IEnumerable<BO.a38NonPersonEventPlan> lisA38 { get; set; }
        public IEnumerable<BO.a35PersonEventPlan> lisA35 { get; set; }
        public a01TabCapacity CapacityView { get; set; }
        public int ActiveTabIndex { get; set; } = 1;
        public int a01ID { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public BO.a01EventPermission PermA01 { get; set; }
        public string a38Location { get; set; }
        public bool a38IsDriver { get; set; }
        public DateTime? D1 { get; set; }
        public DateTime? D2 { get; set; }
        public int SelectedJ02ID { get; set; }
        public string SelectedPerson { get; set; }
        public string SelectedJ25ID { get; set; }
       
        public int SelectedJ23ID { get; set; }
        public string SelectedJ23Name { get; set; }
        public BO.j23NonPerson RecJ23 { get; set; }
        public int a05ID { get; set; }
        public string a05Name { get; set; }
        public int CurMonth { get; set; }
        public int CurYear { get; set; }
    }
}
