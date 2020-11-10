using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a02Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a02Record() { rec_pid = pid, rec_entity = "a02" };
            v.Rec = new BO.a02Inspector();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a02InspectorBL.Load(v.rec_pid);
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
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.inspektorat_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a02Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a02Inspector c = new BO.a02Inspector();
                if (v.rec_pid > 0) c = Factory.a02InspectorBL.Load(v.rec_pid);
                c.a04ID = v.Rec.a04ID;
                c.j02ID = v.Rec.j02ID;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a02InspectorBL.Save(c);
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