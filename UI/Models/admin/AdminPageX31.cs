using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class AdminPageX31:AdminPage
    {
        public int QueryX32ID { get; set; }
        public IEnumerable<BO.x32ReportType> lisX32 { get; set; }
        public string explicitquery { get; set; }
    }
}
