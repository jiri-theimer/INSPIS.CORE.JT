using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public class TheTranslator
    {
        private class TranslateScope
        {
            public string x91Code { get; set; }
            public string Orig { get; set; }
            public string Eng { get; set; }
            public string Ukr { get; set; }
        }
        
        private HashSet<TranslateScope> _hash;
        private readonly BL.RunningApp _app;

        public TheTranslator(BL.RunningApp app)
        {
            _app = app;            
            SetupPallete();
        }
        private void SetupPallete()
        {
            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            var lis = db.GetList<BO.x91Translate>("select * from x91Translate");

            _hash = new HashSet<TranslateScope>(lis.Count());
            foreach(var rec in lis)
            {
                var c = new TranslateScope() { x91Code = rec.x91Code,Orig=rec.x91Orig, Eng = rec.x91Lang1, Ukr = rec.x91Lang2 };
                if (string.IsNullOrEmpty(c.Eng)) c.Eng = "!" + rec.x91Code + "!";
                if (string.IsNullOrEmpty(c.Ukr)) c.Ukr = "!" + rec.x91Code + "!";

                _hash.Add(c);
            }
        }

        public string DoTranslate(string strCode,int langindex)
        {
            try
            {
                switch (langindex)
                {
                    case 1:
                        return _hash.First(p => p.x91Code == strCode).Eng;
                       
                    case 2:
                        return _hash.First(p => p.x91Code == strCode).Ukr;
                        
                    default:
                        return _hash.First(p => p.x91Code == strCode).Orig;
                        
                }                
                
            }
            catch
            {
                if (_app.TranslatorMode == "Collect")
                {
                    DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
                    db.RunSql("INSERT INTO x91Translate(x91Code,x91Orig,x91UserInsert,x91UserUpdate,x91DateInsert,x91DateUpdate) VALUES(@code,@orig,'collect','collect',GETDATE(),GETDATE())", new { code = strCode,orig=strCode });
                    SetupPallete();
                }
                return "?" + strCode + "?";
            }            
            
        }
    }
}
