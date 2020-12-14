using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class a01CreateController : BaseController
    {
        public IActionResult Index(int a10id,int a03id)
        {
            var v = new a01CreateViewModel() { a10ID = a10id, a03ID = a03id };            
            if (v.a10ID > 0)
            {
                var c = Factory.a10EventTypeBL.Load(a10id);
                if (string.IsNullOrEmpty(c.a10ViewUrl_Insert)==false)
                {
                    if (c.a10ViewUrl_Insert.Contains("?"))
                    {
                        c.a10ViewUrl_Insert += "&a10id=" + v.a10ID.ToString();
                    }
                    else
                    {
                        c.a10ViewUrl_Insert += "?a10id=" + v.a10ID.ToString();
                    }
                    if (v.a03ID > 0)
                    {
                        c.a10ViewUrl_Insert += "&a03id=" + v.a03ID.ToString();
                    }
                    return Redirect(c.a10ViewUrl_Insert);
                    //switch (c.a10Aspx_Insert)
                    //{
                    //    case "a01_create_res.aspx":
                    //    case "a42/create":
                    //        return RedirectToAction("Create", "a42", new { a10id = a10id,a03id=a03id });
                    //    case "a01_create_aus.aspx":
                    //    case "a01create/aus":
                    //        return RedirectToAction("Aus", new { a10id = a10id });
                    //    case "a01create/helpdesk":
                    //    case "a01_create_hd.aspx":
                    //        return RedirectToAction("Helpdesk", new { a10id = a10id });
                    //    default:
                    //        return RedirectToAction("Standard", new { a10id = a10id });

                    //}
                }
                else
                {
                    return RedirectToAction("Standard", new { a10id = a10id,a03id=a03id });
                }
                
            }
            RefreshState(v);


            return View(v);
        }

        public IActionResult Standard(int a10id, int j02id,int clonebypid,int a03id)
        {
            if (a10id == 0 && clonebypid == 0)
            {
                return RedirectToAction("Index");
            }
            var v = new a01CreateViewModel() { j02ID = j02id,a10ID=a10id,CloneByPid=clonebypid };
            if (a03id > 0)
            {
                v.a03ID = a03id;
                v.Institution = Factory.a03InstitutionBL.Load(a03id).NamePlusRedizo;
            }
            if (clonebypid > 0)
            {
                //kopírování akce do nové                
                v.Rec = Factory.a01EventBL.Load(clonebypid);
                       
                v.a10ID = v.Rec.a10ID;
                v.Rec.pid = 0;
                v.a03ID = v.Rec.a03ID;
                v.Institution = v.Rec.a03Name;
                
                v.lisA11 = Factory.a11EventFormBL.GetList(new BO.myQueryA11() { a01id = clonebypid }).ToList();
                foreach(var c in v.lisA11)
                {                   
                    c.TempGuid = BO.BAS.GetGuid();
                }
                
                v.lisA41 = Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = clonebypid }).ToList();
                foreach (var c in v.lisA41)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                    c.PersonCombo = c.PersonDesc;
                    
                }
            }

            RefreshState(v);
            RefreshInstitution(v);
            
            
            
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Standard(Models.a01CreateViewModel v, string oper,int f06id,int krat,string guid)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper== "a08_change")
            {
                v.lisA11.Clear();
                return View(v);
            }
            if (oper == "f06_add" && f06id>0)
            {
                for(int i = 1; i <= krat; i++)
                {
                    var c = new BO.a11EventForm() { f06ID = f06id,TempGuid=BO.BAS.GetGuid() };
                    c.f06Name = Factory.f06FormBL.Load(f06id).f06Name;
                    v.lisA11.Add(c);
                }
                
                return View(v);
            }
            if (oper == "f06_delete")
            {
                v.lisA11.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper== "f06_clear")
            {
                v.lisA11.Clear();
                return View(v);
            }
            if (oper == "j02_add" && v.SelectedJ02ID>0)
            {
                var c = new BO.a41PersonToEvent() { TempGuid = BO.BAS.GetGuid() };

                if (v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).Count() > 0)
                {
                    if (v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).Count() > 0)
                    {
                        c = v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).First();
                        if (c.IsTempDeleted == true)
                        {
                            c.IsTempDeleted = false;
                            return View(v);
                        }
                        else
                        {
                            this.AddMessage("Tato osoba již je v seznamu.");
                            return View(v);
                        }
                    }
                }
                c.j02ID = v.SelectedJ02ID;
                c.PersonCombo = Factory.j02PersonBL.Load(c.j02ID).FullNameDesc;                
                c.a45ID = BO.EventRoleENUM.Resitel;
                c.a45Name = Factory.FBL.LoadA45((int)c.a45ID).a45Name;
                c.a45IsManual = true;
                v.lisA41.Add(c);
                return View(v);
            }
            if (oper == "j02_delete")
            {
                v.lisA41.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }

            if (ModelState.IsValid)
            {                

                BO.a01Event c = new BO.a01Event();
                c.a10ID = v.a10ID;
                c.a03ID = v.a03ID;
                c.j02ID_Issuer = v.j02ID;
                c.a08ID = v.Rec.a08ID;
                c.a01DateFrom = v.Rec.a01DateFrom;
                c.a01DateUntil = v.Rec.a01DateUntil;
                c.a01CaseCode = v.Rec.a01CaseCode;
                c.a01Name = v.Rec.a01Name;
                c.a01Description = v.Rec.a01Description;

                c.pid = Factory.a01EventBL.Create(c, true, v.lisA11.Where(p=>p.IsTempDeleted==false).ToList(), v.lisA41.Where(p=>p.IsTempDeleted==false).ToList(), null,null);
                if (c.pid > 0)
                {
                    if (v.CloneByPid>0 && v.a46ID > 0)
                    {
                        var recA24 = new BO.a24EventRelation() { a01ID_Right = v.CloneByPid, a01ID_Left = c.pid, a46ID = v.a46ID };
                        Factory.a01EventBL.SaveA24Record(recA24);
                    }
                    return RedirectToAction("RecPage", "a01", new { pid = c.pid });

                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        public IActionResult Injury(int a10id,int a03id)
        {
            var v = new a01CreateInjuryViewModel() { a10ID = a10id, a03ID = a03id };
            if (v.a10ID == 0)
            {
                return RedirectToAction("Index");
            }
            if (v.a03ID == 0)
            {
                return StopPage(false, Factory.tra("Na vstupu chybí vazba na instituci."));
            }
            
            RefreshStateInjury(v);


            return View(v);
        }
        [HttpPost]
        public IActionResult Injury(a01CreateInjuryViewModel v)
        {
            RefreshStateInjury(v);
            if (ModelState.IsValid)
            {

                BO.a01Event c = new BO.a01Event() {a01DateFrom=DateTime.Today, a01IsTemporary = true, a10ID = v.a10ID, a03ID = v.a03ID, j03ID_Creator = Factory.CurrentUser.pid, j02ID_Issuer = Factory.CurrentUser.j02ID };
                c.a08ID = Factory.GlobalParams.LoadParamInt("a08ID_Injury", 0); //téma se bere z globálních parametrů
                if (c.a08ID == 0)
                {
                    this.AddMessage("V nastavení globálních parametrů chybí definice klíče [a08ID_Injury].");return View(v);
                }
                
                var lisF06 = Factory.f06FormBL.GetList(new BO.myQueryF06() { a08id = c.a08ID, IsRecordValid = true });
                if (lisF06.Count() == 0)
                {
                    this.AddMessage("Pro téma akce není k dispozici ani jeden platný formulář."); return View(v);
                }
                var lisA11 = new List<BO.a11EventForm>();
                foreach(var cF06 in lisF06)
                {                    
                    lisA11.Add(new BO.a11EventForm() { f06ID = cF06.pid, a37ID = v.a37ID });
                }
                c.pid = Factory.a01EventBL.Create(c, true, lisA11, null, null, null);
                if (c.pid > 0)
                {
                    c = Factory.a01EventBL.Load(c.pid);
                    return Redirect(Factory.a01EventBL.GetPageUrl(c));
                    

                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }
        private void RefreshStateInjury(Models.a01CreateInjuryViewModel v)
        {
            v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
        }

        public IActionResult Helpdesk(int a10id, int j02id)
        {
            if (a10id == 0)
            {
                return RedirectToAction("Index");
            }
            var v = new a01CreateViewModel() { j02ID = j02id };

            RefreshState(v);
            RefreshInstitution(v);
            v.a10ID = a10id;
            
            v.Rec = new BO.a01Event();
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Helpdesk(Models.a01CreateViewModel v, string oper)
        {
            RefreshState(v);
            if (oper == "j02_change")
            {
                v.a03ID = 0;
                v.Institution = "";
                RefreshInstitution(v);
                return View(v);
            }
            RefreshInstitution(v);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.Rec.a01Description) == true || v.Rec.a01Description.Length < 5)
                {
                    this.AddMessage("Popis požadavku je příliš stručný.");
                    return View(v);
                }

                BO.a01Event c = new BO.a01Event();
                c.a10ID = v.a10ID;
                c.a03ID = v.a03ID;
                c.j02ID_Issuer = v.j02ID;
                c.a08ID = v.Rec.a08ID;
                c.a01Description = v.Rec.a01Description;

                c.pid = Factory.a01EventBL.Create(c, true, null, null, null, null);
                if (c.pid > 0)
                {
                    return RedirectToAction("RecPage", "a01", new { pid = c.pid });

                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }
        private void RefreshInstitution(Models.a01CreateViewModel v)
        {
            var mq = new BO.myQueryA03() { IsRecordValid = true, j02id = v.j02ID };
            var lis = Factory.a03InstitutionBL.GetList(mq);
            if (lis.Count() > 0)
            {
                v.a03ID = lis.First().pid;
                v.Institution = lis.First().NamePlusRedizo;
                v.IsComboA03 = true;
            }
            else
            {
                v.IsComboA03 = false;
            }
        }
        private void RefreshState(Models.a01CreateViewModel v)
        {
            if (v.CloneByPid > 0)
            {
                v.RecCloneByPid = Factory.a01EventBL.Load(v.CloneByPid);
            }
            if (v.Rec == null)
            {
                v.Rec = new BO.a01Event();
            }
            if (v.a10ID > 0)
            {
                v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
            }
            
            if (v.j02ID == 0)
            {
                v.j02ID = Factory.CurrentUser.j02ID;
            }
            v.RecJ02 = Factory.j02PersonBL.Load(v.j02ID);
            v.Person = v.RecJ02.FullNameDesc;
            if (v.Rec.a08ID > 0)
            {
                v.lisA12=Factory.a08ThemeBL.GetListA12(v.Rec.a08ID);
            }
            if (v.lisA11 == null)
            {
                v.lisA11 = new List<BO.a11EventForm>();
            }
            if (v.lisA41 == null)
            {
                v.lisA41 = new List<BO.a41PersonToEvent>();
            }
            switch (v.MasterPrefixComboJ02)
            {
                case "a04":
                    v.MasterPidComboJ02 = v.RecJ02.a04ID;
                    break;
                case "a05":
                    v.MasterPidComboJ02 = v.RecJ02.a05ID;
                    break;
            }
           
        }
    }
}