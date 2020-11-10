using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class f29Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new f29Record() { rec_pid = pid, rec_entity = "f29" };
            v.Rec = new BO.f29PortalQuestionTab();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f29PortalQuestionTabBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQuery("a17DepartmentType");
                mq.f29id = v.rec_pid;
                var lis = Factory.a17DepartmentTypeBL.GetList(mq);
                v.a17IDs = string.Join(",", lis.Select(p => p.pid));
                v.a17Names = string.Join(",", lis.Select(p => p.a17Name));
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
        public IActionResult Record(Models.Record.f29Record v)
        {
            
            if (ModelState.IsValid)
            {
                BO.f29PortalQuestionTab c = new BO.f29PortalQuestionTab();
                if (v.rec_pid > 0) c = Factory.f29PortalQuestionTabBL.Load(v.rec_pid);
                c.f29Name = v.Rec.f29Name;
                c.f29Description = v.Rec.f29Description;
                c.f29Ordinal = v.Rec.f29Ordinal;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> a17ids = BO.BAS.ConvertString2ListInt(v.a17IDs);
                c.pid = Factory.f29PortalQuestionTabBL.Save(c, a17ids);
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