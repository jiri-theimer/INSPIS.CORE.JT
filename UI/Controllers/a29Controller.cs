using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a29Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {            
            var v = new a29Record() { rec_pid = pid, rec_entity = "a29" };
            
            v.Rec = new BO.a29InstitutionList();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a29InstitutionListBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQueryA03() { a29id = v.rec_pid };
                
                v.a03IDs = string.Join(",", Factory.a03InstitutionBL.GetList(mq).Select(p => p.pid));
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
        public IActionResult Record(Models.Record.a29Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a29InstitutionList c = new BO.a29InstitutionList();
                if (v.rec_pid > 0) c = Factory.a29InstitutionListBL.Load(v.rec_pid);
                c.a29Name = v.Rec.a29Name;
                c.a29Description = v.Rec.a29Description;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> a03ids = BO.BAS.ConvertString2ListInt(v.a03IDs);
                c.pid = Factory.a29InstitutionListBL.Save(c, a03ids);
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