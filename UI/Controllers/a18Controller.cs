using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a18Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {

            var v = new a18Record() { rec_pid = pid, rec_entity = "a18" };

            v.Rec = new BO.a18DepartmentDomain();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a18DepartmentDomainBL.Load(v.rec_pid);

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
        public IActionResult Record(Models.Record.a18Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a18DepartmentDomain c = new BO.a18DepartmentDomain();
                if (v.rec_pid > 0) c = Factory.a18DepartmentDomainBL.Load(v.rec_pid);

                c.a18Code = v.Rec.a18Code;
                c.a18Name = v.Rec.a18Name;

                c.a18SubCode1 = v.Rec.a18SubCode1;
                c.a18SubCode2 = v.Rec.a18SubCode2;
                c.a18SubCode3 = v.Rec.a18SubCode3;
                c.a18SubCode4 = v.Rec.a18SubCode4;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a18DepartmentDomainBL.Save(c);
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