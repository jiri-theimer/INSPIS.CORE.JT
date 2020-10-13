using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class j23Controller : BaseController
    {
        public IActionResult InfoCapacity(int pid, int m, int y)
        {
            var v = new j23InfoCapacity() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.j23NonPersonBL.Load(v.pid);
                var tg = Factory.o51TagBL.GetTagging("j23", pid);
                v.TagHtml = tg.TagHtml;

            }
            var mq = new BO.myQuery("a38");
            mq.j23id = v.pid;
            mq.global_d1 = new DateTime(y, m, 1);
            mq.global_d2 = Convert.ToDateTime(mq.global_d1).AddMonths(1).AddDays(-1);
            v.lisA38 = Factory.a38NonPersonEventPlanBL.GetList(mq).OrderBy(p => p.a38PlanDate);
            if (v.lisA38.Count() > 0)
            {
                mq = new BO.myQuery("a01");
                mq.pids = v.lisA38.Select(p => p.a01ID).Distinct().ToList();
                mq.explicit_orderby = "a.a01DateFrom";
                v.lisA01 = Factory.a01EventBL.GetList(mq);
            }
            return View(v);
        }
        public IActionResult Info(int pid)
        {
            var v = new j23RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.j23NonPersonBL.Load(v.pid);
                var tg = Factory.o51TagBL.GetTagging("j23", pid);
                v.TagHtml = tg.TagHtml;

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j23Record() { rec_pid = pid, rec_entity = "j23" };
            v.Rec = new BO.j23NonPerson();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j23NonPersonBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.j23Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j23NonPerson c = new BO.j23NonPerson();
                if (v.rec_pid > 0) c = Factory.j23NonPersonBL.Load(v.rec_pid);
                c.j23Name = v.Rec.j23Name;
                c.a05ID = v.Rec.a05ID;
                c.j24ID = v.Rec.j24ID;
                c.j23Code = v.Rec.j23Code;
                c.j23Description = v.Rec.j23Description;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j23NonPersonBL.Save(c);
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