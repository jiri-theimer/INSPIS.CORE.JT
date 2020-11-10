using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a09Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a09Record() { rec_pid = pid, rec_entity = "a09" };
            v.Rec = new BO.a09FounderType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a09FounderTypeBL.Load(v.rec_pid);
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
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.instituce_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a09Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a09FounderType c = new BO.a09FounderType();
                if (v.rec_pid > 0) c = Factory.a09FounderTypeBL.Load(v.rec_pid);
                c.a09Name = v.Rec.a09Name;
                c.a09UIVCode = v.Rec.a09UIVCode;
                c.a09Description = v.Rec.a09Description;                
                c.a09Ordinal = v.Rec.a09Ordinal;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a09FounderTypeBL.Save(c);
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