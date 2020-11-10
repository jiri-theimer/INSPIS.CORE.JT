using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
namespace UI.Controllers
{
    public class f12Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new f12Record() { rec_pid = pid, rec_entity = "f12" };
            v.Rec = new BO.f12FormType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f12FormTypeBL.Load(v.rec_pid);
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
            return ViewTupCiselnik(v,BO.j03AdminRoleValueFlagEnum.formular_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f12Record v)
        {

            if (ModelState.IsValid)
            {
                BO.f12FormType c = new BO.f12FormType();
                if (v.rec_pid > 0) c = Factory.f12FormTypeBL.Load(v.rec_pid);
                c.f12ParentID = v.Rec.f12ParentID;                
                c.f12Name = v.Rec.f12Name;
                c.f12ParentID = v.Rec.f12ParentID;               
                c.f12Description = v.Rec.f12Description;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.f12FormTypeBL.Save(c);
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