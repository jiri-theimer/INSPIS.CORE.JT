using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Tab;

namespace UI.Controllers
{
    public class a39Controller : BaseController
    {        
        public IActionResult RecordByPerson(int pid, int j02id, bool isclone)
        {
            if (pid == 0 && j02id == 0)
            {
                return this.StopPage(true, "j02id missing");
            }
            var v = new a39RecordByPerson() { rec_pid = pid, rec_entity = "a39", j02ID = j02id,form_action= "RecordByPerson" };


            v.Rec = new BO.a39InstitutionPerson();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a39InstitutionPersonBL.Load(v.rec_pid);
                v.j02ID = v.Rec.j02ID;
                if (v.Rec.j04ID_Explicit > 0)
                {
                    v.RoleName = v.Rec.SchoolRoleName;
                }
                v.a03Name = v.Rec.a03Name;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);

            RefreshStateRecordByPerson(v);

            if (isclone)
            {
                v.MakeClone();

            }
            return ViewTup(v, BO.j05PermValuEnum.A03Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecordByPerson(Models.Record.a39RecordByPerson v)
        {
            RefreshStateRecordByPerson(v);

            if (ModelState.IsValid)
            {
                var c = new BO.a39InstitutionPerson();
                if (v.rec_pid > 0) c = Factory.a39InstitutionPersonBL.Load(v.rec_pid);
                c.j02ID = v.j02ID;
                c.a03ID = v.Rec.a03ID;
                c.j04ID_Explicit = v.Rec.j04ID_Explicit;
                c.a39RelationFlag = v.Rec.a39RelationFlag;
                c.a39Description = v.Rec.a39Description;
                c.a39IsAllowInspisWS = v.Rec.a39IsAllowInspisWS;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a39InstitutionPersonBL.Save(c);
                if (c.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }
        private void RefreshStateRecordByPerson(a39RecordByPerson v)
        {            
            v.RecJ02 = Factory.j02PersonBL.Load(v.j02ID);

        }
        public IActionResult Record(int pid, int a03id,int j02id, bool isclone)
        {
            if (pid == 0 && a03id == 0 && j02id==0)
            {
                return this.StopPage(true, "a03id or j02id missing");
            }
            var v = new a39Record() { rec_pid = pid, rec_entity = "a39", a03ID = a03id };

            v.Rec = new BO.a39InstitutionPerson();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a39InstitutionPersonBL.Load(v.rec_pid);
                v.a03ID = v.Rec.a03ID;
                if (v.Rec.j04ID_Explicit > 0)
                {
                    v.RoleName = v.Rec.SchoolRoleName;
                }                
                v.Person = v.Rec.Person;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);

            RefreshStateRecord(v);

            if (isclone)
            {
                v.MakeClone();

            }
            return ViewTup(v, BO.j05PermValuEnum.A03Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a39Record v)
        {
            RefreshStateRecord(v);

            if (ModelState.IsValid)
            {
                var c = new BO.a39InstitutionPerson();
                if (v.rec_pid > 0) c = Factory.a39InstitutionPersonBL.Load(v.rec_pid);
                c.a03ID = v.a03ID;
                c.j02ID = v.Rec.j02ID;
                c.j04ID_Explicit = v.Rec.j04ID_Explicit;
                c.a39RelationFlag = v.Rec.a39RelationFlag;
                c.a39Description = v.Rec.a39Description;
                c.a39IsAllowInspisWS = v.Rec.a39IsAllowInspisWS;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a39InstitutionPersonBL.Save(c);
                if (c.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateRecord(a39Record v)
        {
            v.PageTitle = Factory.tra("Kontaktní osoba");
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);

        }

        public IActionResult SchoolAccount(int a39id)
        {
            if (a39id == 0)
            {
                return this.StopPage(true, "a39id missing");
            }
            var v = new a39SchoolAccount() { a39ID = a39id };

            RefreshStateSchoolAccount(v);
            v.SelectedJ04ID = v.Rec.j04ID_Explicit.ToString();           
            if (v.IsNotSchoolAccount)
            {
                return this.StopPage(true, "Tento uživatelský účet je možné spravovat pouze přes ČŠI administrátorské rozhraní systému.",true);
            }
            if (!TestIfSchoolAdmin(v.Rec))
            {
                return this.StopPage(true, "Nedisponujete oprávněním k administraci uživatelských účtů v instituci.",true);
            }

            return View(v);
        }

        private bool TestIfSchoolAdmin(BO.a39InstitutionPerson rec)
        {
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal))
            {
                return true;
            }
            var lisMyA39 = Factory.a39InstitutionPersonBL.GetList(new BO.myQuery("a39") { a03id = rec.a03ID }).Where(p=>p.j02ID==Factory.CurrentUser.j02ID);
            if (lisMyA39.Count() == 0)
            {
                return false;
            }
            int intMyJ04ID = lisMyA39.First().j04ID_Explicit;
            if (intMyJ04ID == 0)
            {
                intMyJ04ID = Factory.CurrentUser.j04ID;
            }
            var recMyJ04 = Factory.j04UserRoleBL.Load(intMyJ04ID);
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.SchoolAdminUser, recMyJ04.j04RoleValue))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SchoolAccount(Models.a39SchoolAccount v, string oper)
        {
            RefreshStateSchoolAccount(v);
            if (oper == "cut")
            {

                if (Factory.CBL.DeleteRecord("a39", v.a39ID) == "1")
                {
                    v.SetJavascript_CallOnLoad(v.a39ID);
                    return View(v);
                }

            }
            if (ModelState.IsValid)
            {
                var cJ02 = Factory.j02PersonBL.Load(v.Rec.j02ID);
                cJ02.j02TitleBeforeName = v.RecJ02.j02TitleBeforeName;
                cJ02.j02FirstName = v.RecJ02.j02FirstName;
                cJ02.j02LastName = v.RecJ02.j02LastName;
                cJ02.j02TitleAfterName = v.RecJ02.j02TitleAfterName;
                cJ02.j02Email = v.RecJ02.j02Email;
                cJ02.j02Mobile = v.RecJ02.j02Mobile;
                cJ02.j02Phone = v.RecJ02.j02Phone;
                if (Factory.j02PersonBL.Save(cJ02) == 0)
                {
                    return View(v);
                }
                var recJ03 = Factory.j03UserBL.LoadByJ02(cJ02.pid);

                var recA39 = Factory.a39InstitutionPersonBL.Load(v.a39ID);
                recA39.a39Description = v.Rec.a39Description;
                recA39.a39IsAllowInspisWS = v.Rec.a39IsAllowInspisWS;
                recA39.j04ID_Explicit = Convert.ToInt32(v.SelectedJ04ID);

                recA39.pid = Factory.a39InstitutionPersonBL.Save(recA39);
                if (recA39.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(recA39.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateSchoolAccount(a39SchoolAccount v)
        {
            if (v.Rec == null)
            {
                v.Rec = Factory.a39InstitutionPersonBL.Load(v.a39ID);
            }
            if (v.RecJ02 == null)
            {
                v.RecJ02 = Factory.j02PersonBL.Load(v.Rec.j02ID);
            }
            if (v.RecJ03 == null)
            {
                v.RecJ03 = Factory.j03UserBL.LoadByJ02(v.Rec.j02ID);
            }
            v.RecA03 = Factory.a03InstitutionBL.Load(v.Rec.a03ID);

           
            v.lisJ04 = Factory.j04UserRoleBL.GetList(new BO.myQueryJ04() { IsRecordValid = true });

            var mq = new BO.myQuery("j03") { j02id = v.Rec.j02ID };
            foreach (var c in Factory.j03UserBL.GetList(mq))
            {

                if (v.lisJ04.Where(p => p.j04ID == c.j04ID && p.j04IsAllowInSchoolAdmin == false).Count() > 0)
                {
                    v.IsNotSchoolAccount = true;

                }
            }


            v.lisJ04 = v.lisJ04.Where(p => p.j04IsAllowInSchoolAdmin == true);
        }

        public IActionResult CreateSchoolAccountBySearch(int a03id)
        {
            if (a03id == 0)
            {
                return this.StopPage(true, "a03id missing");
            }
            var v = new a39CreateSchoolAccount() { a03ID = a03id,j03LangIndex=Factory.App.DefaultLangIndex };

            RefreshStateCreateSchoolAccount(v);
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSchoolAccountBySearch(Models.a39CreateSchoolAccount v, string oper)
        {
            RefreshStateCreateSchoolAccount(v);
            if (oper == "load")
            {
                var recJ03 = Factory.j03UserBL.LoadByLogin(v.SearchLogin, 0);
                if (recJ03 == null)
                {
                    this.AddMessage("Uživatelský účet nebyl nalezen.");
                }
                else
                {
                    var mq = new BO.myQuery("a39") {a03id=v.a03ID, j02id = recJ03.j02ID };
                    if (Factory.a39InstitutionPersonBL.GetList(mq).Count() > 0)
                    {
                        this.AddMessage("Vyhledaný uživatelský účet již má přiřazenou roli v této instituci.");return View(v);
                    }
                    v.SearchJ03ID = recJ03.pid;
                }
                RefreshStateCreateSchoolAccount(v);
                return View(v);
            }
            if (ModelState.IsValid)
            {
                if (v.SearchJ03ID == 0)
                {
                    this.AddMessage("Nebyl načten uživatelský účet."); return View(v);
                }
                var recJ03 = Factory.j03UserBL.LoadByLogin(v.SearchLogin, 0);

                var recA39 = new BO.a39InstitutionPerson() { j02ID = recJ03.j02ID, a03ID = v.a03ID, j04ID_Explicit = Convert.ToInt32(v.SelectedJ04ID), a39Description = v.a39Description, a39IsAllowInspisWS = v.a39IsAllowInspisWS };

                recA39.pid = Factory.a39InstitutionPersonBL.Save(recA39);
                if (recA39.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(recA39.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        public IActionResult CreateSchoolAccount(int a03id)
        {
            if (a03id == 0)
            {
                return this.StopPage(true, "a03id missing");
            }
            var v = new a39CreateSchoolAccount() { a03ID = a03id,j03LangIndex=Factory.App.DefaultLangIndex };

            RefreshStateCreateSchoolAccount(v);
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSchoolAccount(Models.a39CreateSchoolAccount v, string oper)
        {
            RefreshStateCreateSchoolAccount(v);
            if (oper == "newpwd")
            {
                var c = new BO.CLS.PasswordChecker();
                v.Password = c.RandomPassword(Factory.App.PasswordMinLength);
                v.VerifyPassword = v.Password;
                return View(v);
            }

            if (ModelState.IsValid)
            {
                var recJ03 = new BO.j03User() { j03Login = v.RecJ02.j02Email, j04ID = Convert.ToInt32(v.SelectedJ04ID),j03LangIndex=v.j03LangIndex };

                var recJ02 = new BO.j02Person() { j02TitleBeforeName = v.RecJ02.j02TitleBeforeName, j02FirstName = v.RecJ02.j02FirstName, j02LastName = v.RecJ02.j02LastName, j02TitleAfterName = v.RecJ02.j02TitleAfterName, j02Email = v.RecJ02.j02Email, j02Mobile = v.RecJ02.j02Mobile, j02Phone = v.RecJ02.j02Phone };

                if (!ValidatePreSaveSchoolAccount(recJ03, recJ02))
                {
                    return View(v);
                }
                if (!ValidateUserPassword(v))
                {
                    return View(v);
                }

                recJ03.pid = Factory.j03UserBL.SaveWithNewPersonalProfile(recJ03, recJ02);
                if (recJ03.pid == 0)
                {
                    return View(v);
                }

                recJ03 = Factory.j03UserBL.Load(recJ03.pid);  //zakládáme nový účet - je třeba pře-generovat j03PasswordHash
                var lu = new BO.LoggingUser();
                recJ03.j03PasswordHash = lu.Pwd2Hash(v.Password, recJ03);
                recJ03.pid = Factory.j03UserBL.Save(recJ03);

                var recA39 = new BO.a39InstitutionPerson() { j02ID = recJ03.j02ID, a03ID = v.a03ID, j04ID_Explicit = recJ03.j04ID, a39Description = v.a39Description, a39IsAllowInspisWS = v.a39IsAllowInspisWS };

                recA39.pid = Factory.a39InstitutionPersonBL.Save(recA39);
                if (recA39.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(recA39.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }


        private void RefreshStateCreateSchoolAccount(a39CreateSchoolAccount v)
        {
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
            if (v.RecJ02 == null) v.RecJ02 = new BO.j02Person();
            
            v.lisJ04 = Factory.j04UserRoleBL.GetList(new BO.myQueryJ04() { IsRecordValid = true }).Where(p => p.j04IsAllowInSchoolAdmin == true);
            if (v.SearchJ03ID > 0)
            {
                v.SearchRecJ03 = Factory.j03UserBL.Load(v.SearchJ03ID);
            }
        }

        private bool ValidatePreSaveSchoolAccount(BO.j03User c, BO.j02Person d)
        {
            if (string.IsNullOrEmpty(c.j03Login) || c.j04ID == 0)
            {
                this.AddMessage("Přihlašovací jméno (login) a Aplikační role jsou povinná pro uživatelský účet."); return false;
            }
            if (d != null && Factory.j02PersonBL.ValidateBeforeSave(d) == false)
            {
                return false;
            }
            if (Factory.j03UserBL.LoadByLogin(d.j02Email, 0) != null)    //otestovat, zda neexistuje e-mail již v membership databázi
            {
                this.AddMessage("Účet {0} je již zaveden u jiného uživatele."); return false;
            }
            return true;
        }

        private bool ValidateUserPassword(a39CreateSchoolAccount v)
        {
            var c = new BO.CLS.PasswordChecker();
            var res = c.CheckPassword(v.Password, Factory.App.PasswordMinLength, Factory.App.PasswordMaxLength, Factory.App.PasswordRequireDigit, Factory.App.PasswordRequireUppercase, Factory.App.PasswordRequireLowercase, Factory.App.PasswordRequireNonAlphanumeric);
            if (res.Flag == BO.ResultEnum.Failed)
            {
                this.AddMessage(res.Message); return false;
            }
            if (v.Password != v.VerifyPassword)
            {
                this.AddMessage("Heslo nesouhlasí s jeho ověřením."); return false;
            }

            return true;
        }



    }
}