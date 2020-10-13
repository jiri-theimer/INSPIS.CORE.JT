using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j03Controller : BaseController
    {
        
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j03Record() { rec_pid = pid, rec_entity = "j03" };
            if (v.rec_pid == 0)
            {
                v.IsDefinePassword = true;
                v.user_profile_oper = "create";
                var c = new BO.CLS.PasswordChecker();
                v.NewPassword = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.NewPassword;
            }
            else
            {
                v.user_profile_oper = "bind";
            }

            v.Rec = new BO.j03User();
            v.RecJ02 = new BO.j02Person();

            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j03UserBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboPerson = v.Rec.fullname_desc;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.AdminGlobal_Ciselniky);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j03Record v,string oper)
        {
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper== "newpwd")
            {
                v.IsDefinePassword = true;
                var c = new BO.CLS.PasswordChecker();
                v.NewPassword = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.NewPassword;
                return View(v);
            }
            if (oper == "changelogin")
            {
                v.IsDefinePassword = true;
                v.IsChangeLogin = true;
                var c = new BO.CLS.PasswordChecker();
                v.NewPassword = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.NewPassword;
                this.AddMessage("Se změnou přihlašovacího jména je třeba resetovat i přístupové heslo.","info");
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.j03User c = new BO.j03User();
                if (v.rec_pid > 0) c = Factory.j03UserBL.Load(v.rec_pid);                
                c.j03Login = v.Rec.j03Login;
                c.j04ID = v.Rec.j04ID;
                c.j03LangIndex = v.Rec.j03LangIndex;
                c.j03IsDomainAccount = v.Rec.j03IsDomainAccount;
                c.j03IsMustChangePassword = v.Rec.j03IsMustChangePassword;
                c.j03IsSystemAccount = v.Rec.j03IsSystemAccount;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                if (!ValidatePreSave(c))
                {
                    return View(v);
                }
                if (v.IsDefinePassword)
                {
                    if (ValidateUserPassword(v) == false)
                    {
                        return View(v);
                    }
                    var lu = new BO.LoggingUser();
                    c.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, c);
                }
                
                if (v.user_profile_oper == "create")
                {
                    c.j02ID = 0;    //zakládáme nový osobní profil
                    var cJ02 = new BO.j02Person() {
                        j02Email=v.RecJ02.j02Email
                        ,j02TitleBeforeName = v.RecJ02.j02TitleBeforeName
                        ,j02FirstName = v.RecJ02.j02FirstName
                        ,j02LastName = v.RecJ02.j02LastName
                        ,j02TitleAfterName = v.RecJ02.j02TitleAfterName
                        ,j02PID = v.RecJ02.j02PID
                        ,j02Mobile=v.RecJ02.j02Mobile
                        ,j02Phone=v.RecJ02.j02Phone
                    };
                    c.pid = Factory.j03UserBL.SaveWithNewPersonalProfile(c, cJ02);
                }
                else
                {
                    c.j02ID = v.Rec.j02ID;  //uložení s existujícím osobním profilem
                    c.pid = Factory.j03UserBL.Save(c);
                }
                if (v.rec_pid == 0 && c.pid>0 && v.IsDefinePassword)
                {
                    c = Factory.j03UserBL.Load(c.pid);  //zakládáme nový účet - je třeba pře-generovat j03PasswordHash
                    var lu = new BO.LoggingUser();
                    c.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, c);
                    c.pid = Factory.j03UserBL.Save(c);
                }
                
                if (c.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private bool ValidateUserPassword(j03Record v)
        {            
            var c = new BO.CLS.PasswordChecker();
            var res=c.CheckPassword(v.NewPassword, Factory.App.PasswordMinLength, Factory.App.PasswordMaxLength, Factory.App.PasswordRequireDigit, Factory.App.PasswordRequireUppercase, Factory.App.PasswordRequireLowercase, Factory.App.PasswordRequireNonAlphanumeric);
            if (res.Flag == BO.ResultEnum.Failed)
            {
                this.AddMessage(res.Message);return false;
            }
            if (v.NewPassword != v.VerifyPassword)
            {
                this.AddMessage("Heslo nesouhlasí s jeho ověřením.");return false;
            }
            
            return true;
        }
        private bool ValidatePreSave(BO.j03User c)
        {
            if (string.IsNullOrEmpty(c.j03Login) || c.j04ID==0)
            {
                this.AddMessage("Přihlašovací jméno (login) a Aplikační role jsou povinná pro uživatelský účet.");return false;
            }
            return true;
        }
        
    }
}