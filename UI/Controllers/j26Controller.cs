using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j26Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j26Record() { rec_pid = pid, rec_entity = "j26" };
            v.Rec = new BO.j26Holiday();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j26HolidayBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.j26Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j26Holiday c = new BO.j26Holiday();
                if (v.rec_pid > 0) c = Factory.j26HolidayBL.Load(v.rec_pid);
                c.j26Name = v.Rec.j26Name;
                c.j26Date = v.Rec.j26Date;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j26HolidayBL.Save(c);
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