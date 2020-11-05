using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class BaseRecordViewModel
    {
        
        public int rec_pid { get; set; }        
        public string rec_entity { get; set; }
        public string form_action { get; set; } = "Record";
        
        public string PageTitle { get; set; }
        public int ActiveTabIndex { get; set; } = 1;
        public string Javascript_CallOnLoad { get; set; }

        public void SetJavascript_CallOnLoad(int intPID, string strFlag = null, string jsfunction = "_reload_layout_and_close")
        {
            this.Javascript_CallOnLoad = string.Format(jsfunction + "({0},'{1}');", intPID, strFlag);
        }

        private MyToolbarViewModel _toolbar;


        public MyToolbarViewModel Toolbar
        {
            get
            {
                return _toolbar;
            }
            set
            {
                _toolbar = value;                
                _toolbar.RecordEntity = this.rec_entity;                
            }
        }

        public void MakeClone()
        {
            this.rec_pid = 0;
            
            _toolbar.MakeClone();

        }
    }
}
