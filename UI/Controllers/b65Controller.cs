using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class b65Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new b65Record() { rec_pid = pid, rec_entity = "b65" };
            v.Rec = new BO.b65WorkflowMessage();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.b65WorkflowMessageBL.Load(v.rec_pid);
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
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.ostatni_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.b65Record v)
        {

            if (ModelState.IsValid)
            {
                BO.b65WorkflowMessage c = new BO.b65WorkflowMessage();
                if (v.rec_pid > 0) c = Factory.b65WorkflowMessageBL.Load(v.rec_pid);
                c.b65Name = v.Rec.b65Name;
                c.x29ID = v.Rec.x29ID;
                c.b65MessageSubject = v.Rec.b65MessageSubject;
                c.b65MessageBody = v.Rec.b65MessageBody;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.b65WorkflowMessageBL.Save(c);
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