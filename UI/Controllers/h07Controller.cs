using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class h07Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new h07Record() { rec_pid = pid, rec_entity = "h07" };
            v.Rec = new BO.h07ToDoType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.h07ToDoTypeBL.Load(v.rec_pid);
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
            return ViewTup(v, BO.j05PermValuEnum.AdminGlobal_Ciselniky);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.h07Record v)
        {

            if (ModelState.IsValid)
            {
                BO.h07ToDoType c = new BO.h07ToDoType();
                if (v.rec_pid > 0) c = Factory.h07ToDoTypeBL.Load(v.rec_pid);
                c.h07Name = v.Rec.h07Name;
                c.h07Description = v.Rec.h07Description;
                c.h07IsCapacityPlan = v.Rec.h07IsCapacityPlan;
                c.h07IsDefault = v.Rec.h07IsDefault;
                c.h07IsToDo = v.Rec.h07IsToDo;
                c.b65ID = v.Rec.b65ID;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.h07ToDoTypeBL.Save(c);
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