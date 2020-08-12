using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridViewModel
    {        
        public string Entity { get; set; }
        public int MasterPID { get; set; }
        public string MasterEntity { get; set; }
        public string MasterFlag { get; set; }
        public string ondblclick { get; set; }
        public BO.j72TheGridState GridState { get; set; }
        
        public IEnumerable<BO.TheGridColumn> Columns { get; set; }

        public List<BO.TheGridColumnFilter> AdhocFilter { get; set; }
        
        
        public TheGridOutput firstdata { get; set; }
    }

   public class TheGridUIContext
    {
        public string entity { get; set; }
        public int j72id { get; set; }
        public int go2pid { get; set; }
        public string oper { get; set; }    //pagerindex/pagesize/sortfield
        public string key { get; set; }
        public string value { get; set; }
        public int master_pid { get; set; }
        public string master_entity { get; set; }
        public int contextmenuflag { get; set; }

        public string ondblclick { get; set; }

        public string master_flag { get; set; }

    }
}
