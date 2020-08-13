using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public class TheTranslator
    {
        private IEnumerable<BO.x91Translate> _lis;
        private readonly BL.RunningApp _app;

        public TheTranslator(BL.RunningApp app)
        {
            _app = app;
            _lis = new List<BO.x91Translate>();
            SetupPallete();
        }
        private void SetupPallete()
        {
            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            _lis = db.GetList<BO.x91Translate>("select x91ID,x91Code,x91Orig,case when isnull(x91Lang1,'')='' then '!'+x91Code+'!' else x91Lang1 end as x91Lang1,case when isnull(x91Lang2,'')='' then '!'+x91Code+'!' else x91Lang2 end as x91Lang2,x91Lang3,x91Lang4 from x91Translate");
           
        }

        public string DoTranslate(string strCode,int langindex)
        {
            try
            {
                switch (langindex)
                {
                    case 1:
                        return _lis.First(p => p.x91Code == strCode).x91Lang1;
                        
                    case 2:
                        return _lis.First(p => p.x91Code == strCode).x91Lang2;
                        
                    default:
                        return _lis.First(p => p.x91Code == strCode).x91Orig;
                        
                }                
                
            }
            catch
            {
                if (_app.TranslatorMode == "Collect")
                {
                    DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
                    db.RunSql("INSERT INTO x91Translate(x91Code,x91Orig) VALUES(@code,@orig)",new { code = strCode,orig=strCode });
                    SetupPallete();
                }
                return "?" + strCode + "?";
            }            
            
        }
    }
}
