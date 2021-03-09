using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public class TheGlobalParams
    {
        private IEnumerable<BO.x35GlobalParam> _lis;
        private readonly BL.RunningApp _app;

        public TheGlobalParams(BL.RunningApp app)
        {
            _app = app;
            _lis = new List<BO.x35GlobalParam>();
            SetupPallete();
        }
        private void SetupPallete()
        {
            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            _lis = db.GetList<BO.x35GlobalParam>("select a.*," + db.GetSQL1_Ocas("x35") + " from x35GlobalParam a");
           
        }

        public string LoadParam(string strKey, string strDefault = null)
        {
            if (_lis.Where(p => p.x35Key == strKey).Count() > 0)
            {
                return _lis.Where(p => p.x35Key == strKey).First().x35Value;
            }
            else
            {
                return strDefault;
            }            
        }
        public int LoadParamInt(string strKey, int intDefault)
        {
            string s = LoadParam(strKey);
            if (s == null)
            {
                return intDefault;
            }
            else
            {
                return BO.BAS.InInt(s);
            }
        }
        public DateTime? LoadParamDate(string strKey)
        {
            string s = LoadParam(strKey);
            if (String.IsNullOrEmpty(s) == true)
            {
                return null;
            }
            else
            {
                return BO.BAS.String2Date(s);
            }
        }

    }
}
