﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using System.Net.Http;

namespace UI.Controllers
{
    public class j03Controller : BaseController
    {
        private readonly IHttpClientFactory _httpclientfactory; //client pro PIPE api

        public j03Controller(IHttpClientFactory hcf)
        {            
            _httpclientfactory = hcf;   //client pro PIPE api
        }
        public IActionResult Record(int pid, bool isclone,int j02id)
        {
            var v = new j03Record() { rec_pid = pid, rec_entity = "j03" };
            if (v.rec_pid == 0)
            {
                v.IsDefinePassword = true;
                v.user_profile_oper = "create";                
                v.NewPassword =new BL.bas.PasswordSupport().GetRandomPassword();
                v.VerifyPassword = v.NewPassword;                
            }
            else
            {
                v.user_profile_oper = "bind";
            }
            v.lisB65 = Factory.b65WorkflowMessageBL.GetList(new BO.myQuery("b65")).Where(p => p.x29ID == 503);

            v.Rec = new BO.j03User();
            v.Rec.j03LangIndex = Factory.App.DefaultLangIndex;
            v.RecJ02 = new BO.j02Person();            

            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j03UserBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboPerson = v.Rec.fullname_desc;
                if (v.Rec.j02ID == 0)
                {
                    v.user_profile_oper = "nobind"; //uživatel bez osobního profilu
                }
                if (v.Rec.j03GridSelectionModeFlag == 1)
                {
                    v.IsGridClipboard = true;
                }
            }
            else
            {
                if (j02id > 0)
                {
                    v.RecJ02 = Factory.j02PersonBL.Load(j02id);                    
                    v.ComboPerson = v.RecJ02.FullNameDesc;
                    v.user_profile_oper = "bind";
                    v.Rec.j02ID = v.RecJ02.pid;
                }
            }

            v.lisAdminRoleValues = new List<j03RecordAdminRoleValue>();
            ARV("Uživatelé systému", 1, 2, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Inspektoráty", 3, 4, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Nepersonální zdroje", 5, 6, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Instituce", 7, 8, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Akce", 9, 10, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Formuláře", 11, 12, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Přílohy", 13, 14, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Úkoly", 15, 16, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Neobsazeno", 17, 18, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Tiskové sestavy", 19, 20, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);
            ARV("Ostatní", 21, 22, v.lisAdminRoleValues, v.Rec.j03AdminRoleValue);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.uzivatel_er);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j03Record v,string oper,int b65id)
        {
            v.lisB65 = Factory.b65WorkflowMessageBL.GetList(new BO.myQuery("b65")).Where(p => p.x29ID == 503);
            var cPwdSupp = new BL.bas.PasswordSupport();

            if (oper == "postback")
            {      
                if (v.rec_pid == 0)
                {
                    v.IsDefinePassword = !v.Rec.j03IsDomainAccount;
                }
                else
                {
                    if (v.Rec.j03IsDomainAccount) v.IsDefinePassword = false;
                }
              
                if (v.IsDefinePassword && v.NewPassword == null)
                {
                    v.NewPassword = cPwdSupp.GetRandomPassword();
                    v.VerifyPassword = v.NewPassword;
                }


                return View(v);
            }
            if (oper == "clearparams")
            {
                Factory.CBL.TruncateUserParams(v.rec_pid);
                this.AddMessage("Server cache vyčištěna.", "info");
                return View(v);
            }
            if (oper== "newpwd")
            {
                v.IsDefinePassword = true;                
                v.NewPassword = cPwdSupp.GetRandomPassword();
                v.VerifyPassword = v.NewPassword;
                return View(v);
            }
            if (oper == "changelogin")
            {
                v.IsChangeLogin = true;
                if (!Factory.App.PipeIsMembershipProvider)
                {
                    v.IsDefinePassword = true;                    
                    v.NewPassword = cPwdSupp.GetRandomPassword();
                    v.VerifyPassword = v.NewPassword;
                    this.AddMessage("Se změnou přihlašovacího jména je třeba resetovat i přístupové heslo.", "info");
                }                                               
                return View(v);
            }
            if (ModelState.IsValid)
            {
                if (v.user_profile_oper=="bind" && v.Rec.j02ID==0)
                {
                    this.AddMessage("U uživatelského účtu chybí vazba na osobní profil."); return View(v);
                }
                BO.j03User c = new BO.j03User();
                if (v.rec_pid > 0)
                {
                    c = Factory.j03UserBL.Load(v.rec_pid);
                    if (!v.Rec.j03IsDomainAccount && Factory.App.PipeIsMembershipProvider && !v.IsDefinePassword &&  Factory.j03UserBL.LoadMembershipUserId(c.pid) == null)
                    {
                        this.AddMessage("Uživatelský účet nemá vygenerované heslo v membership databázi.");return View(v);
                    }
                   
                }                
                c.j03Login = v.Rec.j03Login.Trim();
                c.j04ID = v.Rec.j04ID;
                c.j03LangIndex = v.Rec.j03LangIndex;
                c.j03IsDomainAccount = v.Rec.j03IsDomainAccount;
                c.j03IsMustChangePassword = v.Rec.j03IsMustChangePassword;
                c.j03IsSystemAccount = v.Rec.j03IsSystemAccount;
                c.j03LangIndex = v.Rec.j03LangIndex;
                if (v.IsGridClipboard)
                {
                    c.j03GridSelectionModeFlag = 1;
                }
                else
                {
                    c.j03GridSelectionModeFlag = 0;
                }
                
                c.j03IsDebugLog = v.Rec.j03IsDebugLog;
                c.j03AdminRoleValue = "";
                foreach(var cc in v.lisAdminRoleValues)
                {
                    if (cc.IsER)
                    {
                        c.j03AdminRoleValue += "1";
                    }
                    else
                    {
                        c.j03AdminRoleValue += "0";
                    }
                    if (cc.IsRO)
                    {
                        c.j03AdminRoleValue += "1";
                    }
                    else
                    {
                        c.j03AdminRoleValue += "0";
                    }
                }
                c.j03AdminRoleValue += "000000000000000000000000000000000000000";                
                c.j03AdminRoleValue = BO.BAS.LeftString(c.j03AdminRoleValue, 50);
                    
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
                    c.j03PasswordHash = cPwdSupp.GetPasswordHash(v.NewPassword, c);
                }
               
                switch (v.user_profile_oper){
                    case "create":
                        c.j02ID = 0;    //zakládáme nový osobní profil
                        var cJ02 = new BO.j02Person()
                        {
                            j02Email = v.RecJ02.j02Email,
                            j02TitleBeforeName = v.RecJ02.j02TitleBeforeName,
                            j02FirstName = v.RecJ02.j02FirstName,
                            j02LastName = v.RecJ02.j02LastName,
                            j02TitleAfterName = v.RecJ02.j02TitleAfterName,
                            j02PID = v.RecJ02.j02PID,
                            j02Mobile = v.RecJ02.j02Mobile,
                            j02Phone = v.RecJ02.j02Phone
                        };
                        if (!Factory.j02PersonBL.ValidateBeforeSave(cJ02))
                        {
                            return View(v);
                        }
                        c.pid = Factory.j03UserBL.SaveWithNewPersonalProfile(c, cJ02);                        
                        break;
                    case "bind":
                        c.j02ID = v.Rec.j02ID;  //uložení s existujícím osobním profilem                        
                        c.pid = Factory.j03UserBL.Save(c);
                        break;
                    default:
                        //účet bez vazby na osobní profil
                        c.j02ID = 0;
                        c.j03IsSystemAccount = true;
                        c.pid = Factory.j03UserBL.Save(c);
                        break;
                }

                if (v.IsChangeLogin && v.rec_pid > 0 && Factory.App.PipeIsMembershipProvider)
                {
                    //uložit nový login do centrální membership databáze
                    var cP = new BL.bas.PipeSupport(_httpclientfactory.CreateClient(), this.Factory);
                    if (!cP.ChangeLogin(Factory.j03UserBL.LoadMembershipUserId(c.pid), c.j03Login).Result)
                    {
                        this.AddMessageTranslated("CHyba: ChangeLogin");
                    }
                }
                if (v.IsDefinePassword)
                {
                    //generování nového hesla
                    if (v.rec_pid == 0 && c.pid > 0)
                    {
                        c = Factory.j03UserBL.Load(c.pid);  //zakládáme nový účet - je třeba pře-generovat j03PasswordHash                        
                        c.j03PasswordHash = cPwdSupp.GetPasswordHash(v.NewPassword, c);
                        c.pid = Factory.j03UserBL.Save(c);
                    }
                    if (Factory.App.PipeIsMembershipProvider)
                    {
                        //uložit nové heslo do centrálního INSPIS membership
                        var cP = new BL.bas.PipeSupport(_httpclientfactory.CreateClient(), this.Factory);
                        string strMembershipID = null;
                        try
                        {
                            strMembershipID= cP.GetUserID(c.j03Login).Result;
                            var strNewPassword = cP.RecoveryPassword(c.j03Login, v.NewPassword).Result;
                        }
                        catch
                        {
                            strMembershipID = cP.CreateUser(c.j03Login, c.j02Email, v.NewPassword).Result;
                        }
                        
                        if (Factory.j03UserBL.LoadMembershipUserId(c.pid) == null)
                        {
                            Factory.j03UserBL.UpdateMembershipUserId(c.pid, strMembershipID);
                        }
                        
                    }
                }
                
                
                
                
                if (c.pid > 0)
                {
                    if (oper == "save_and_send")
                    {
                        return Redirect("/Mail/SendMail?x29id=503&j02id=" + c.j02ID.ToString() + "&x40datapid=" + c.pid.ToString() + "&b65id=" + b65id.ToString()+ "&param1="+v.NewPassword);
                    }
                    else
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                        return View(v);
                    }
                    
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private bool ValidateUserPassword(j03Record v)
        {            
            var res= new BL.bas.PasswordSupport().CheckPassword(v.NewPassword);
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
        private bool ValidatePreSave(BO.j03User recJ03)
        {
            if (string.IsNullOrEmpty(recJ03.j03Login) || recJ03.j04ID==0)
            {
                this.AddMessage("Přihlašovací jméno (login) a Aplikační role jsou povinná pro uživatelský účet.");return false;
            }
            
            return true;
        }

        private void ARV(string strHeader,int intER,int intRO,List<j03RecordAdminRoleValue> lisAdminRoleValues,string strAdminRoleValue)
        {
            var cc = new j03RecordAdminRoleValue() { Header = Factory.tra(strHeader), ER = intER, RO = intRO };
           
            if (strAdminRoleValue !=null && strAdminRoleValue.Length >= intER && strAdminRoleValue.Substring(intER-1,1)=="1")
            {
                cc.IsER = true;
            }
            if (strAdminRoleValue !=null && strAdminRoleValue.Length >= intRO && strAdminRoleValue.Substring(intRO - 1, 1) == "1")
            {
                cc.IsRO = true;
            }
            
            lisAdminRoleValues.Add(cc);
        }
        
    }
}