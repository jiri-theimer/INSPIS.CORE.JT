using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Reflection.Metadata;

namespace BO
{
    public class LoggingUser
    {
        
        public string Login { get; set; }        
        public string Password { get; set; }
        public int CookieExpiresInHours { get; set; } = 1;
        public int LangIndex { get; set; }
        public bool IsChangedLangIndex { get; set; }

        public string Message { get; set; }
        public string AppClient { get; set; }

        public string Browser_UserAgent { get; set; }
        public int Browser_AvailWidth { get; set; }
        public int Browser_AvailHeight { get; set; }
        public int Browser_InnerWidth { get; set; }
        public int Browser_InnerHeight { get; set; }
        public string Browser_DeviceType { get; set; }
        public string Browser_Host { get; set; }

        public string ReturnUrl { get; set; }

        public Result VerifyHash(string strPwd,string strLogin,BO.j03User cSavedJ03)
        {           
            var hasher = new BO.CLS.PasswordHasher();            
            var overeni = hasher.VerifyHashedPassword(cSavedJ03.j03PasswordHash, getSul(strLogin,strPwd, cSavedJ03.pid));
            if (overeni == BO.CLS.PasswordVerificationResult.Failed)
            {

                return new Result(true, "Ověření uživatele se nezdařilo - pravděpodobně chybné heslo nebo jméno!");
            }
            else
            {
                return new Result(false);
            }
        }
       

        public string Pwd2Hash(string strPwd,BO.j03User cJ03)
        {
            var hasher = new BO.CLS.PasswordHasher();
           return hasher.HashPassword(getSul(cJ03.j03Login,  strPwd , cJ03.pid));
        }

        private string getSul(string strLogin,string strPwd, int intPid)
        {
            return strLogin.ToUpper() + "+kurkuma+" + strPwd + "+" + intPid.ToString();
        }
    }
}
