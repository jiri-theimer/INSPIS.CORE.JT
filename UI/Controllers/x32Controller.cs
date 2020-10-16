using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x32Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x32Record() { rec_pid = pid, rec_entity = "x32" };
            v.Rec = new BO.x32ReportType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x32ReportTypeBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.x32Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x32ReportType c = new BO.x32ReportType();
                if (v.rec_pid > 0) c = Factory.x32ReportTypeBL.Load(v.rec_pid);
                c.x32ParentID = v.Rec.x32ParentID;
                c.x32Name = v.Rec.x32Name;
                c.x32ParentID = v.Rec.x32ParentID;
                c.x32Description = v.Rec.x32Description;
                c.x32Ordinal = v.Rec.x32Ordinal;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.x32ReportTypeBL.Save(c);
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