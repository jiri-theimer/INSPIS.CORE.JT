using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x27Controller : BaseController
    {
        public IActionResult SelectFunction(string elementid)
        {
            var v = new SelectFunction();
            var mq = new BO.myQuery("x27");
            mq.IsRecordValid = true;
            v.lisX27 = Factory.x27EvalFunctionBL.GetList(mq).OrderBy(p => p.x27Name);
            v.ElementID = elementid;
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x27Record() { rec_pid = pid, rec_entity = "x27" };
            v.Rec = new BO.x27EvalFunction();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x27EvalFunctionBL.Load(v.rec_pid);
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
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.formular_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x27Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x27EvalFunction c = new BO.x27EvalFunction();
                if (v.rec_pid > 0) c = Factory.x27EvalFunctionBL.Load(v.rec_pid);
                c.x27Name = v.Rec.x27Name;
                c.x27Description = v.Rec.x27Description;
                c.x27Returns = v.Rec.x27Returns;
                c.x27Ordinal = v.Rec.x27Ordinal;
                c.x27Parameters = v.Rec.x27Parameters;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
             
                c.pid = Factory.x27EvalFunctionBL.Save(c);
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