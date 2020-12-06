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




        public static BO.ThePeriod InhalePeriodDates(BL.ThePeriodProvider pp, BL.Factory f, string prefix, string masterentity)
        {            
            int x = f.CBL.LoadUserParamInt(get_param_key("grid-period-value-" + prefix, masterentity));  //podformuláře filtrují období za sebe a nikoliv globálně jako flatview/masterview
            switch (x)
            {
                case 0: //nefiltrovat období
                    return pp.ByPid(0);
                case 1:     //ručně zadaný interval d1-d2
                    var ret = pp.ByPid(1);
                    ret.d1 = f.CBL.LoadUserParamDate(get_param_key("grid-period-d1-" + prefix, masterentity));
                    ret.d2 = f.CBL.LoadUserParamDate(get_param_key("grid-period-d2-" + prefix, masterentity));
                    return ret;
                default:    //pojmenované období
                    return pp.ByPid(x);

            }

        }


        private static string get_param_key(string strKey, string strMasterEntity)
        {
            if (strMasterEntity != null)
            {
                return (strKey += "-" + strMasterEntity);
            }
            else
            {
                return strKey;
            }
        }
    }

    
}
