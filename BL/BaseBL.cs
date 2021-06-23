using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    class BaseBL
    {
        protected BL.Factory _mother;
        protected DL.DbHandler _db;
        private readonly System.Text.StringBuilder _sb;

        public BaseBL(BL.Factory mother)
        {
            _mother = mother;
            _db = new DL.DbHandler(_mother.App.ConnectString, _mother.CurrentUser,_mother.App.LogFolder);
            _sb = new System.Text.StringBuilder();
        }

        public void sb(string s=null)
        {   
            if (s != null)
            {
                _sb.Append(s);
            }
            
        }
        public void sbinit()
        {
            _sb.Clear();
        }
        public string sbret()
        {
            string s= _sb.ToString();
            sbinit();
            return s;
        }
        public void AddMessageWithPars(string strMessage,string strPar1,string  strPar2=null,string template = "error")
        {
            string s = _mother.tra(strMessage);
            
            if (!string.IsNullOrEmpty(strPar2))
            {
                s = string.Format(s,strPar1, strPar2);
            }
            else
            {
                s = string.Format(s, strPar1);
            }
            _mother.CurrentUser.AddMessage(s, template);  //automaticky podléhá překladu do ostatních jazyků

        }
        public void AddMessage(string strMessage, string template = "error")
        {
            _mother.CurrentUser.AddMessage(_mother.tra(strMessage), template);  //automaticky podléhá překladu do ostatních jazyků

        }
        public void AddMessageTranslated(string strMessage, string template = "error")
        {
            _mother.CurrentUser.AddMessage(strMessage, template);  //nepodléhá překladu do ostatních jazyků
        }

        public void ChangeDB(string strConString)
        {
            //zatím prázdné - pouze kvůli kompatibilitě syntaxe s původním EPIS
        }
    }
}
