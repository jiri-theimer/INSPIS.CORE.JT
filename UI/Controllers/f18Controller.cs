using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class f18Controller : BaseController
    {
        public IActionResult Record(int pid,int f06id, bool isclone)
        {
            var v = new f18Record() { rec_pid = pid, rec_entity = "f18",f06ID=f06id };
            v.Rec = new BO.f18FormSegment();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f18FormSegmentBL.Load(v.rec_pid);
                v.f06ID = v.Rec.f06ID;
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            if (v.f06ID == 0)
            {
                return this.StopPage(true, "f06id missing");
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.RecF06 = Factory.f06FormBL.Load(v.f06ID);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f18Record v)
        {
            v.RecF06 = Factory.f06FormBL.Load(v.f06ID);
            if (ModelState.IsValid)
            {
                BO.f18FormSegment c = new BO.f18FormSegment();
                if (v.rec_pid > 0) c = Factory.f18FormSegmentBL.Load(v.rec_pid);
                c.f06ID = v.f06ID;
                c.f18ParentID = v.Rec.f18ParentID;
                c.f18Name = v.Rec.f18Name;
                c.f18RazorTemplate = v.Rec.f18RazorTemplate;
                c.f18Ordinal = v.Rec.f18Ordinal;
                c.f18ParentID = v.Rec.f18ParentID;
                c.f18Description = v.Rec.f18Description;
                c.f18Text = v.Rec.f18Text;
                c.f18SupportingText = v.Rec.f18SupportingText;
                c.f18ReadonlyExpression = v.Rec.f18ReadonlyExpression;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.f18FormSegmentBL.Save(c);
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