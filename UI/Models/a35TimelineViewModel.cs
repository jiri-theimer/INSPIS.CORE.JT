using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a35TimeLineViewModel:BaseViewModel
    {
        public int a05ID { get; set; }
        public string a05Name { get; set; }
        public IEnumerable<BO.j02Person> lisJ02 { get; set; }
        public IEnumerable<BO.a35TimeLine> lisTimeLine { get; set; }
        
        public IEnumerable<BO.a38TimeLinePerson> lisTimeLineA38 { get; set; }
        public IEnumerable<BO.j26Holiday> lisJ26 { get; set; }
        public IEnumerable<BO.h04TodoCapacity> lisH04 { get; set; }

        public IEnumerable<BO.o51Tag> lisO51 { get; set; }  //filtrování podle kategorií

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

        public int QueryByO51ID { get; set; }
    }
}
