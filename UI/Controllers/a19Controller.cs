using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a19Controller : BaseController
    {
        public IActionResult Record(int pid, int a03id, bool isclone)
        {
            if (pid == 0 && a03id == 0)
            {
                return this.StopPage(true, "Na vstupu chybí ID školy",true);
            }
            var v = new a19Record() { rec_pid = pid, a03ID = a03id, rec_entity = "a19" };
            
            v.Rec = new BO.a19DomainToInstitutionDepartment();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a19DomainToInstitutionDepartment.Load(v.rec_pid);
                v.a03ID = v.Rec.a03ID;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec,false);

            RefreshState(v);

            if (isclone)
            {
                v.MakeClone();

            }
            return ViewTup(v, BO.j05PermValuEnum.AdminGlobal_Ciselniky);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a19Record v)
        {
            RefreshState(v);
            if (ModelState.IsValid)
            {
                BO.a19DomainToInstitutionDepartment c = new BO.a19DomainToInstitutionDepartment();
                if (v.rec_pid > 0) c = Factory.a19DomainToInstitutionDepartment.Load(v.rec_pid);
                c.a37ID = v.Rec.a37ID;
                c.a18ID = v.Rec.a18ID;
                c.a19IsShallEnd = v.Rec.a19IsShallEnd;
                c.a19StudyCapacity = v.Rec.a19StudyCapacity;
                
                c.a19StudyDuration = v.Rec.a19StudyDuration;
                c.a19StudyLanguage = v.Rec.a19StudyLanguage;
                c.a19StudyPlatform = v.Rec.a19StudyPlatform;
                
                c.pid = Factory.a19DomainToInstitutionDepartment.Save(c);
                if (c.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(a19Record v)
        {            
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
            v.PageTitle = "Vzdělávací obor";
            
        }
    }
}