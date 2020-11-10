using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a17Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
           
            var v = new a17Record() { rec_pid = pid, rec_entity = "a17" };

            v.Rec = new BO.a17DepartmentType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a17DepartmentTypeBL.Load(v.rec_pid);
               
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
        public IActionResult Record(Models.Record.a17Record v)
        {
            
            if (ModelState.IsValid)
            {
                BO.a17DepartmentType c = new BO.a17DepartmentType();
                if (v.rec_pid > 0) c = Factory.a17DepartmentTypeBL.Load(v.rec_pid);
               
                c.a17IsDefault = v.Rec.a17IsDefault;
                c.a17Name = v.Rec.a17Name;

                c.a17UIVCode = v.Rec.a17UIVCode;
                c.a17Description = v.Rec.a17Description;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a17DepartmentTypeBL.Save(c);
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