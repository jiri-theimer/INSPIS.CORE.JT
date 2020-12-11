using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridViewModel
    {        
        public string Entity { get; set; }
        public string ControllerName { get; set; }
        public int MasterPID { get; set; }
        public string MasterEntity { get; set; }
        public string MasterFlag { get; set; }
        public string ondblclick { get; set; }
        public BO.TheGridState GridState { get; set; }
        
        public IEnumerable<BO.TheGridColumn> Columns { get; set; }

        public List<BO.TheGridColumnFilter> AdhocFilter { get; set; }
        
        
        public TheGridOutput firstdata { get; set; }

        public string GridMessage { get; set; }     //zpráva dole za navigací pageru
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
        public string addfilterid { get; set; } //fixní filtrování podle období, typu akce, tématu a zapojení do akce/úkolu
        public string fixedcolumns { get; set; }
        public int viewflag { get; set; }   //1: flatview s možností přepnutí na masterview, 2:masterview s možností přepnutí na flatview, 3: vždy pouze flatview bez možnosti přepnout
        public string pathname { get; set; }   //volající url v prohlížeči

    }
}
