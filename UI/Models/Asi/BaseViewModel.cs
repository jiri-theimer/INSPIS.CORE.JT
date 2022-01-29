using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class BaseViewModel
    {
        private string _pagetitle;
        public string PageTitle
        {
            get
            {
                return _pagetitle;
            }
            set
            {
                _pagetitle = value;
            }
        }
        

        public string Javascript_CallOnLoad { get; set; }
        
       
        public void SetJavascript_CallOnLoad(int intPID, string strFlag = null,string jsfunction= "_reload_layout_and_close")
        {
            if (jsfunction.Contains("("))
            {
                this.Javascript_CallOnLoad = jsfunction;
            }
            else
            {                
                this.Javascript_CallOnLoad = string.Format(jsfunction + "({0},'{1}');", intPID, strFlag);
            }
            
        }

        public void SetJavascript_CallOnLoad(string reloadurl_parentsite)
        {
            this.Javascript_CallOnLoad = "_close_and_reload_parentsite('"+reloadurl_parentsite+"')";
        }

        
    }
}
