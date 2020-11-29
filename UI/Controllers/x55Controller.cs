using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x55Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x55Record() { rec_pid = pid, rec_entity = "x55" };
            v.Rec = new BO.x55Widget();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x55WidgetBL.Load(v.rec_pid);
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
            return ViewTup(v,BO.j05PermValuEnum.AdminGlobal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x55Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x55Widget c = new BO.x55Widget();
                if (v.rec_pid > 0) c = Factory.x55WidgetBL.Load(v.rec_pid);
                c.x55Name = v.Rec.x55Name;
                c.x55Code = v.Rec.x55Code;
                c.x55IsSystem = v.Rec.x55IsSystem;
                c.x55TypeFlag = v.Rec.x55TypeFlag;
                c.x55Sql = v.Rec.x55Sql;
                c.x55Content = v.Rec.x55Content;
                c.x55Ordinal = v.Rec.x55Ordinal;
                c.x55Image = v.Rec.x55Image;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.x55WidgetBL.Save(c);
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