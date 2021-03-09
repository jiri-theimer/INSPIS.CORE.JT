using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class f26Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone,int f18id)
        {
            var v = new f26Record() { rec_pid = pid, rec_entity = "f26" };
            v.Rec = new BO.f26BatteryBoard();
            if (v.rec_pid==0 && f18id == 0)
            {
                return this.StopPage(true, "Na vstupu chybí segment formuláře.",true);
            }            
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f26BatteryBoardBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.f06ID = v.Rec.f06ID;
            }
            else
            {
                var recF18 = Factory.f18FormSegmentBL.Load(f18id);
                v.f06ID = recF18.f06ID;
                v.Rec.f18ID = recF18.pid;
                v.Rec.f18Name = recF18.f18Name;
            }
            v.RecF06 = Factory.f06FormBL.Load(v.f06ID);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f26Record v)
        {
            v.RecF06 = Factory.f06FormBL.Load(v.f06ID);

            if (ModelState.IsValid)
            {
                BO.f26BatteryBoard c = new BO.f26BatteryBoard();
                if (v.rec_pid > 0) c = Factory.f26BatteryBoardBL.Load(v.rec_pid);
                c.f26Name = v.Rec.f26Name;
                c.f18ID = v.Rec.f18ID;
                c.f26ColumnHeaders = v.Rec.f26ColumnHeaders;
                c.f26SupportingText = v.Rec.f26SupportingText;
                c.f26Hint = v.Rec.f26Hint;
                c.f26Description = v.Rec.f26Description;
                c.f26Ordinal = v.Rec.f26Ordinal;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                c.pid = Factory.f26BatteryBoardBL.Save(c);
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