using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a35TimeLineViewModel:BaseViewModel
    {
        public int a05ID { get; set; }
        public int a01ID { get; set; }
        public string a05Name { get; set; }
        public IEnumerable<BO.j02Person> lisJ02 { get; set; }
        public IEnumerable<BO.a35TimeLine> lisTimeLine { get; set; }

        public IEnumerable<BO.a41PersonToEvent> lisA41 { get; set; }
        public IEnumerable<BO.a38TimeLinePerson> lisTimeLineA38 { get; set; }
        public IEnumerable<BO.j26Holiday> lisJ26 { get; set; }
        public IEnumerable<BO.h04TodoCapacity> lisH04 { get; set; }        

        public List<DateTime> lisDays { get; set; }
        public List<int> lisYears { get; set; }

        public int PracovnichDni { get; set; }
        public int CurMonth { get; set; }
        public int CurYear { get; set; }

        public int NextMonth { get; set; }
        public int NextYear { get; set; }
        public int PrevMonth { get; set; }
        public int PrevYear { get; set; }

        public int PersonQueryFlag { get; set; }
        public int A38QueryFlag { get; set; }

        
        public string o51IDs { get; set; }
        public string o51Names { get; set; }
    }
}
