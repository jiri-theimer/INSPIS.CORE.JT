using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class PeriodViewModel
    {
        public DateTime? d1 { get; set; }
        public DateTime? d2 { get; set; }

        public int PeriodValue { get; set; }
        
        public bool IsShowButtonRefresh { get; set; }

        public string d1_iso
        {
            get
            {
                if (d1 == null) return "2000-01-01";
                return Convert.ToDateTime(d1).ToString("o").Substring(0,10);
            }
        }
        public string d2_iso
        {
            get
            {
                if (d2 == null) return "2000-01-01";
                return Convert.ToDateTime(d2).ToString("o").Substring(0,10);
            }
        }
    }
}
