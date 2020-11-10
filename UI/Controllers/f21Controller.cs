using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers
{
    public class f21Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new f21Record() { rec_pid = pid, rec_entity = "f21" };
            v.Rec = new BO.f21ReplyUnit();
           
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f21ReplyUnitBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.formular_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f21Record v)
        {
            RefreshState(v);
            if (ModelState.IsValid)
            {
                BO.f21ReplyUnit c = new BO.f21ReplyUnit();
                if (v.rec_pid > 0) c = Factory.f21ReplyUnitBL.Load(v.rec_pid);
                c.f21Name = v.Rec.f21Name;
                c.f21ExportValue = v.Rec.f21ExportValue;
                c.f21MinValue = v.Rec.f21MinValue;
                c.f21MaxValue = v.Rec.f21MaxValue;
                c.f21IsNegation = v.Rec.f21IsNegation;
                c.f21IsCommentAllowed = v.Rec.f21IsCommentAllowed;
                c.f21Description = v.Rec.f21Description;
                c.f21Ordinal = v.Rec.f21Ordinal;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.f21ReplyUnitBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(f21Record v)
        {
            var mq = new BO.myQuery("f19Question");
            mq.f21id = v.rec_pid;
            v.lisF19 = Factory.f19QuestionBL.GetList(mq);
            mq = new BO.myQuery("f22ReplySet");
            mq.f21id = v.rec_pid;
            v.lisF22 = Factory.f22ReplySetBL.GetList(mq);
        }
    }
}