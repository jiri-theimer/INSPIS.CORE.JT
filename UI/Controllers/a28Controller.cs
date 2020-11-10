using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a28Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {

            var v = new a28Record() { rec_pid = pid, rec_entity = "a28" };

            v.Rec = new BO.a28SchoolType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a28SchoolTypeBL.Load(v.rec_pid);

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
        public IActionResult Record(Models.Record.a28Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a28SchoolType c = new BO.a28SchoolType();
                if (v.rec_pid > 0) c = Factory.a28SchoolTypeBL.Load(v.rec_pid);
                
                c.a28Name = v.Rec.a28Name;
                c.a28Code = v.Rec.a28Code;                
                c.a28Ordinary = v.Rec.a28Ordinary;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a28SchoolTypeBL.Save(c);
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