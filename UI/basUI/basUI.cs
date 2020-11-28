using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace UI
{
    public static class basUI
    {
        public static string render_select_option(string strValue,string strText,string strSelectedValue)
        {
            if (strValue == strSelectedValue)
            {
                return string.Format("<option value='{0}' selected>{1}</option>", strValue, strText);
            }
            else
            {
                return string.Format("<option value='{0}'>{1}</option>", strValue, strText);
            }
            
        }

        
        
       
       
    }

    
}
