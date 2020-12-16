using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class j02PersonalTimeline : BaseViewModel
    {
        public BO.j02Person Rec { get; set; }
        public int pid { get; set; }
        public int CurYear { get; set; }
        public int CurMonth { get; set; }
        public int NextMonth { get; set; }
        public int NextYear { get; set; }
        public int PrevMonth { get; set; }
        public int PrevYear { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
        public List<DateTime> lisDays { get; set; }
        public List<int> lisYears { get; set; }
        public IEnumerable<BO.j26Holiday> lisJ26 { get; set; }
        public IEnumerable<BO.a35TimeLinePersonal> lisTimeLine{ get;set;}
        public IEnumerable<BO.h04TodoCapacity> lisH04 { get; set; }

    }
}
