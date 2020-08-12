using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a38TimelineViewModel:BaseViewModel
    {
        public int a05ID { get; set; }
        public string a05Name { get; set; }
        public IEnumerable<BO.j23NonPerson> lisJ23 { get; set; }
        public IEnumerable<BO.a38TimeLine> lisTimeLine { get; set; }
        public IEnumerable<BO.j26Holiday> lisJ26 { get; set; }
        
        public List<DateTime> lisDays { get; set; }
        public List<int> lisYears { get; set; }

        public int CurMonth { get; set; }
        public int CurYear { get; set; }

        public int NextMonth { get; set; }
        public int NextYear { get; set; }
        public int PrevMonth { get; set; }
        public int PrevYear { get; set; }

 
    }
}
