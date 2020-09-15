using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MailBatchFramework:BaseViewModel
    {
        public string BatchGuid { get; set; }
        public IEnumerable<BO.x40MailQueue> lisX40 { get; set; }
        public int TotalCountX40 { get; set; }

        public int LimitTopRecs { get; set; }
        public int QueryByStatusID { get; set; }
    }
}
