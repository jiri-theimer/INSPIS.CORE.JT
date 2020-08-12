using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j24Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j24Record() { rec_pid = pid, rec_entity = "j24" };
            v.Rec = new BO.j24NonPersonType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j24NonPersonTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
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
        public IActionResult Record(Models.Record.j24Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j24NonPersonType c = new BO.j24NonPersonType();
                if (v.rec_pid > 0) c = Factory.j24NonPersonTypeBL.Load(v.rec_pid);
                c.j24Name = v.Rec.j24Name;
                c.j24IsDriver = v.Rec.j24IsDriver;
                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j24NonPersonTypeBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}