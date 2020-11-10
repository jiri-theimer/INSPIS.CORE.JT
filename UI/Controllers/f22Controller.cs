using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class f22Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new f22Record() { rec_pid = pid, rec_entity = "f22" };
            v.Rec = new BO.f22ReplySet();
            
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f22ReplySetBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQuery("f21ReplyUnit");
                mq.f22id = v.rec_pid;
                v.f21IDs = string.Join(",", Factory.f21ReplyUnitBL.GetList(mq).Select(p => p.pid));
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.formular_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f22Record v)
        {

            if (ModelState.IsValid)
            {
                BO.f22ReplySet c = new BO.f22ReplySet();
                if (v.rec_pid > 0) c = Factory.f22ReplySetBL.Load(v.rec_pid);
                c.f22Name = v.Rec.f22Name;
                c.f22Description = v.Rec.f22Description;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> f21ids = BO.BAS.ConvertString2ListInt(v.f21IDs);
                c.pid = Factory.f22ReplySetBL.Save(c, f21ids);
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