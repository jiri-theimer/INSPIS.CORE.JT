using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class a03Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a03Record() { rec_pid = pid, rec_entity = "a03" };            
            v.Rec = new BO.a03Institution();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a03InstitutionBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("a03", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

                if (v.Rec.a03ID_Founder > 0)
                {
                    var c = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Founder);
                    if (c != null)
                    {
                        v.FounderName = c.a03Name;
                    }
                }
                if (v.Rec.a03ID_Supervisory > 0)
                {
                    var c = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Supervisory);
                    if (c != null)
                    {
                        v.SupervisoryName = c.a03Name;
                    }
                }
                if (v.Rec.a03ID_Parent > 0)
                {
                    var c = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Parent);
                    if (c != null)
                    {
                        v.ParentName = c.a03Name;
                    }
                }
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a03Record v,string oper)
        {
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.a03Institution c = new BO.a03Institution();
                if (v.rec_pid > 0) c = Factory.a03InstitutionBL.Load(v.rec_pid);
                c.a03Name = v.Rec.a03Name;
                c.a03ShortName = v.Rec.a03ShortName;
                c.a03REDIZO = v.Rec.a03REDIZO;
                c.a03IsTestRecord = v.Rec.a03IsTestRecord;
                c.a03ICO = v.Rec.a03ICO;
                c.a03DirectorFullName = v.Rec.a03DirectorFullName;
                c.a06ID = v.Rec.a06ID;
                c.a21ID = v.Rec.a21ID;
                c.a05ID = v.Rec.a05ID;
                c.a03ID_Founder = v.Rec.a03ID_Founder;
                c.a03ID_Supervisory = v.Rec.a03ID_Supervisory;
                c.a09ID = v.Rec.a09ID;
                c.a03Street = v.Rec.a03Street;
                c.a03City = v.Rec.a03City;
                c.a03PostCode = v.Rec.a03PostCode;
                c.a03Email = v.Rec.a03Email;
                c.a03Web = v.Rec.a03Web;
                c.a03Mobile = v.Rec.a03Mobile;
                c.a03Phone = v.Rec.a03Phone;
                c.a03SchoolPortalFlag = v.Rec.a03SchoolPortalFlag;
                c.a03ParentFlag = v.Rec.a03ParentFlag;
                c.a03ID_Parent = v.Rec.a03ID_Parent;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a03InstitutionBL.Save(c);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("a03", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            
            this.Notify_RecNotSaved();
            return View(v);
        }
        public IActionResult Info(int pid)
        {
            var v = new a03RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a03InstitutionBL.Load(v.pid);
                if (v.Rec != null)
                {
                    v.TagHtml = Factory.o51TagBL.GetTagging("a03", v.pid).TagHtml;                                        
                    if (v.Rec.a03ID_Founder > 0)
                    {
                        v.RecFounder = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Founder);
                    }
                    if (v.Rec.a03ID_Supervisory > 0)
                    {
                        v.RecSupervisory = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Supervisory);
                    }
                    if (v.Rec.a03ID_Parent > 0)
                    {
                        v.RecParent = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Parent);
                    }

                }                
            }

            return View(v);
        }
        public IActionResult RecPage(int pid)
        {           
            var v = new a03RecPage();
            v.NavTabs = new List<NavTab>();
            v.pid = pid;
            if (v.pid == 0)
            {
                v.pid = Factory.CBL.LoadUserParamInt("a03-RecPage-pid");
            }
            if (v.pid > 0)
            {
                v.Rec = Factory.a03InstitutionBL.Load(v.pid);
                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {                    
                    
                    if (pid>0)
                    {
                        Factory.CBL.SetUserParam("a03-RecPage-pid", pid.ToString());
                    }
                    v.MenuCode = v.Rec.a03REDIZO;
                    if (v.Rec.a06ID == 2)
                    {
                        v.MenuCode = v.Rec.a03FounderCode;
                    }
                    if (v.Rec.a03ID_Founder > 0)
                    {
                        v.RecFounder = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Founder);
                    }
                    if (v.Rec.a03ID_Supervisory > 0)
                    {
                        v.RecSupervisory = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Supervisory);
                    }
                    if (v.Rec.a03ID_Parent > 0)
                    {
                        v.RecParent = Factory.a03InstitutionBL.Load(v.Rec.a03ID_Parent);
                    }
                    v.TagHtml = Factory.o51TagBL.GetTagging("a03", v.pid).TagHtml;

                    RefreshNavTabs(v);
                }
                

            }
           
            if (v.pid == 0)
            {
                v.Rec = new BO.a03Institution();
            }

            return View(v);

        }

        private void RefreshNavTabs(a03RecPage v)
        {
            v.NavTabs.Add(AddTab(Factory.App.Terminology_Akce, "a01Event", "/TheGrid/SlaveView?prefix=a01", false));
            v.NavTabs.Add(AddTab("Kontaktní osoby", "j02Person", "/TheGrid/SlaveView?prefix=a39"));
            v.NavTabs.Add(AddTab("Činnosti školy", "a37InstitutionDepartment", "/TheGrid/SlaveView?prefix=a37"));
            v.NavTabs.Add(AddTab("Vzdělávací obory", "a19DomainToInstitutionDepartment", "/TheGrid/SlaveView?prefix=a19"));
            v.NavTabs.Add(AddTab("Učitelé", "k01Teacher", "/TheGrid/SlaveView?prefix=k01"));
            v.NavTabs.Add(AddTab("Školy zřizovatele", "a03Institution", "/TheGrid/SlaveView?prefix=a03&master_flag=founder"));
            v.NavTabs.Add(AddTab("Dohled", "a03Institution", "/TheGrid/SlaveView?prefix=a03&master_flag=supervisor"));
            v.NavTabs.Add(AddTab("Podřízené", "a03Institution", "/TheGrid/SlaveView?prefix=a03&master_flag=parent"));
            v.NavTabs.Add(AddTab("INEZ", "a42Qes", "/TheGrid/SlaveView?prefix=a42"));
            v.NavTabs.Add(AddTab("Pojmenované seznamy", "a29InstitutionList", "/TheGrid/SlaveView?prefix=a29"));

            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-a03");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                if (tab.Entity== "a03Institution")
                {
                    if (v.Rec.a06ID == 1)
                    {
                        tab.Url += "&master_entity=a03Institution&master_pid=" + v.Rec.a03ID_Founder.ToString();
                    }
                    else
                    {
                        tab.Url += "&master_entity=a03Institution&master_pid=" + v.Rec.pid.ToString();
                    }
                }
                else
                {
                    tab.Url += "&master_entity=a03Institution&master_pid=" + v.pid.ToString();
                }
                
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }
    }

    
}