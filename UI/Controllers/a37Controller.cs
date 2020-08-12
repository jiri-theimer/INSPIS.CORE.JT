using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a37Controller : BaseController
    {
        public IActionResult Record(int pid,int a03id, bool isclone)
        {
            if (pid==0 && a03id == 0)
            {
                return this.StopPage(true, "a03id missing");
            }
            var v = new a37Record() { rec_pid = pid, rec_entity = "a37",a03ID=a03id };

            v.Rec = new BO.a37InstitutionDepartment();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a37InstitutionDepartmentBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.a03ID = v.Rec.a03ID;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);

            RefreshState(v);

            if (isclone)
            {
                v.MakeClone();

            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a37Record v)
        {
            RefreshState(v);
            if (ModelState.IsValid)
            {
                BO.a37InstitutionDepartment c = new BO.a37InstitutionDepartment();
                if (v.rec_pid > 0) c = Factory.a37InstitutionDepartmentBL.Load(v.rec_pid);
                c.a03ID = v.a03ID;
                c.a37Name = v.Rec.a37Name;
                c.a37IZO = v.Rec.a37IZO;                
                c.a17ID = v.Rec.a17ID;
                         
                c.a37Street = v.Rec.a37Street;
                c.a37City = v.Rec.a37City;
                c.a37PostCode = v.Rec.a37PostCode;
                c.a37Email = v.Rec.a37Email;
                c.a37Web = v.Rec.a37Web;
                c.a37Mobile = v.Rec.a37Mobile;
                c.a37Phone = v.Rec.a37Phone;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.rec_pid = Factory.a37InstitutionDepartmentBL.Save(c);
                if (v.rec_pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.rec_pid);
                    return View(v);
                }

            }

            
            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(a37Record v)
        {
            v.PageTitle = "Činnost školy";
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
        }
    }
}