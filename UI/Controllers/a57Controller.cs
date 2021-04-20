using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a57Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {

            var v = new a57Record() { rec_pid = pid, rec_entity = "a57" };

            v.Rec = new BO.a57AutoEvaluation();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a57AutoEvaluationBL.Load(v.rec_pid);
                v.SelectedA08Name = v.Rec.a08Name;
                v.SelectedA10Name = v.Rec.a10Name;
                
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
        public IActionResult Record(Models.Record.a57Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a57AutoEvaluation c = new BO.a57AutoEvaluation();
                if (v.rec_pid > 0) c = Factory.a57AutoEvaluationBL.Load(v.rec_pid);

                c.a10ID = v.Rec.a10ID;
                c.a08ID = v.Rec.a08ID;
                c.a57Name = v.Rec.a57Name;                
                c.a57Description = v.Rec.a57Description;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a57AutoEvaluationBL.Save(c);
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
