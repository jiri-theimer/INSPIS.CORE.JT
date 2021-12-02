using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x39Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x39Record() { rec_pid = pid, rec_entity = "x39" };
            v.Rec = new BO.x39ConnectString();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x39ConnectStringBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.x39Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x39ConnectString c = new BO.x39ConnectString();
                if (v.rec_pid > 0) c = Factory.x39ConnectStringBL.Load(v.rec_pid);
                c.x39Code = v.Rec.x39Code;
                c.x39Name = v.Rec.x39Name;
                c.x39Value = v.Rec.x39Value;
                c.x39Description = v.Rec.x39Description;               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.x39ConnectStringBL.Save(c);
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
