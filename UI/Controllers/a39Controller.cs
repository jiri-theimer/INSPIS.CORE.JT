using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a39Controller : BaseController
    {
        public IActionResult Record(int pid, int a03id, bool isclone)
        {
            if (pid == 0 && a03id == 0)
            {
                return this.StopPage(true, "a03id missing");
            }
            var v = new a39Record() { rec_pid = pid,rec_entity="a39",a03ID=a03id };            
            
            v.Rec = new BO.a39InstitutionPerson();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a39InstitutionPersonBL.Load(v.rec_pid);
                v.a03ID = v.Rec.a03ID;
                v.RoleName = v.Rec.RoleName;
                v.Person = v.Rec.Person;                
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
        public IActionResult Record(Models.Record.a39Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                var c = new BO.a39InstitutionPerson();
                if (v.rec_pid > 0) c = Factory.a39InstitutionPersonBL.Load(v.rec_pid);
                c.a03ID = v.a03ID;
                c.j02ID = v.Rec.j02ID;
                c.j04ID_Explicit = v.Rec.j04ID_Explicit;

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

        private void RefreshState(a39Record v)
        {                        
            v.PageTitle = "Kontaktní osoba";
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);

        }
    }
}