using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j25Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j25Record() { rec_pid = pid, rec_entity = "j25" };
            v.Rec = new BO.j25NonPersonPlanReason();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j25NonPersonPlanReasonBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.j25Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j25NonPersonPlanReason c = new BO.j25NonPersonPlanReason();
                if (v.rec_pid > 0) c = Factory.j25NonPersonPlanReasonBL.Load(v.rec_pid);
                c.j25Name = v.Rec.j25Name;
                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j25NonPersonPlanReasonBL.Save(c);
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