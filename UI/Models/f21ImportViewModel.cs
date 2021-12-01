using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class f21ImportViewModel:BaseViewModel
    {
        public string guid { get; set; }
        public string source { get; set; }
        public bool istested { get; set; }
        public int datafirstrowindex { get; set; }

        public List<BO.f21ReplyUnit> lisPreview { get; set; }

        public string saved_pids { get; set; }
    }
}
