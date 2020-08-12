using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace BO
{
    public class LoggingUser
    {
        [Display(Name ="Přihlašovací jméno (login)")]
        public string Login { get; set; }
        [Display(Name ="Heslo")]
        public string Password { get; set; }
        public int CookieExpiresInHours { get; set; } = 1;      

        public string Message { get; set; }

        public string Browser_UserAgent { get; set; }
        public int Browser_AvailWidth { get; set; }
        public int Browser_AvailHeight { get; set; }
        public int Browser_InnerWidth { get; set; }
        public int Browser_InnerHeight { get; set; }
        public string Browser_DeviceType { get; set; }
        public string Browser_Host { get; set; }

        public Result ValidatePassword(string strPwd)
        {            
            if (string.IsNullOrEmpty(strPwd) || strPwd.Length<6)
            {
                return new Result(true,"Délka hesla musí být minimálně 6 znaků!");
            }
            return new Result(false);
        }
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
        public Result ValidateChangePassword(string strNewPwd,string strCurPwd,string strVerify, j03User cSavedJ03)
        {
            var ret= ValidatePassword(strNewPwd);
            if (ret.Flag == BO.ResultEnum.Failed) { ret.PreMessage = "Nové heslo"; return ret; }
            
            ret = ValidatePassword(strCurPwd);
            if (ret.Flag == BO.ResultEnum.Failed) { ret.PreMessage = "Současné heslo";return ret; }

            if (strNewPwd != strVerify) { return new Result(true, "Nové heslo nesouhlasí s ověřením."); }

            ret = VerifyHash(strCurPwd, cSavedJ03.j03Login, cSavedJ03);
            if (ret.Flag == BO.ResultEnum.Failed) { return new Result(true, "Stávající heslo se nepodařilo ověřit."); }

            if (strNewPwd == strCurPwd) { return new Result(true, "Nové heslo se musí lišit od současného!"); }
           
            return new Result(false);

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
