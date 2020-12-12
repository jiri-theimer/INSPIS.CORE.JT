using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridViewModel
    {        
        //public string Entity { get; set; }
        //public string ControllerName { get; set; }
        //public string FixedColumns { get; set; }
        //public string MasterEntity { get; set; }
        
        //public string ondblclick { get; set; }
        //public string oncmclick { get; set; }
        //public string viewstate { get; set; }   //informance, kterou grid přenáší z klienta na server: oddělovač pipe |

        public TheGridInput GridInput { get; set; }
        public BO.TheGridState GridState { get; set; }
        
        public IEnumerable<BO.TheGridColumn> Columns { get; set; }

        public List<BO.TheGridColumnFilter> AdhocFilter { get; set; }
        
        
        public TheGridOutput firstdata { get; set; }

        public string GridMessage { get; set; }     //zpráva dole za navigací pageru
    }

   public class TheGridUIContext
    {
        public string entity { get; set; }
        public string prefix { get
            {
                return entity.Substring(0, 3);
            }
        }
        public int j72id { get; set; }
        public int go2pid { get; set; }
        public string oper { get; set; }    //pagerindex/pagesize/sortfield
        public string key { get; set; } 
        public string value { get; set; }
        
        public string master_entity { get; set; }
        //public int master_pid { get; set; }
        
        public string ondblclick { get; set; }
        public string oncmclick { get; set; }

       
        public string fixedcolumns { get; set; }
        
        public string pathname { get; set; }   //volající url v prohlížeči
        public List<string> viewstate { get; set; }   //data ze serveru, aby se přenášela s gridem z klienta na server: oddělovač pipe

    }
}
