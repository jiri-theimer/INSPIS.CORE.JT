using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class f44Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new f44Record() { rec_pid = pid, rec_entity = "f44" };
            v.Rec = new BO.f44QuestionCluster();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f44QuestionClusterBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.f44Record v)
        {

            if (ModelState.IsValid)
            {
                BO.f44QuestionCluster c = new BO.f44QuestionCluster();
                if (v.rec_pid > 0) c = Factory.f44QuestionClusterBL.Load(v.rec_pid);
                c.f44Name = v.Rec.f44Name;
                c.f44Ordinal = v.Rec.f44Ordinal;
                c.f44Code = v.Rec.f44Code;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.f44QuestionClusterBL.Save(c);
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