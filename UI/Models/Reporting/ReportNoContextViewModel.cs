using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class ReportNoContextViewModel : BaseViewModel
    {

        public BO.x31Report RecX31 { get; set; }
        public string ReportFileName { get; set; }

        public int SelectedX31ID { get; set; }
        public string SelectedReport { get; set; }

        public bool IsPeriodFilter { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }

        public PeriodViewModel PeriodFilter { get; set; }

        public string SelectedJ72ID{get;set;}
       
        public IEnumerable<BO.j72TheGridTemplate> lisJ72 { get; set; }

        public int LangIndex { get; set; }

    }
}
