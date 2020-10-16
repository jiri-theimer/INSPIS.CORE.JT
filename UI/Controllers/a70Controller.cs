using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers
{
    public class a70Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {

            var v = new a70Record() { rec_pid = pid, rec_entity = "a70" };

            v.Rec = new BO.a70SIS();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a70SISBL.Load(v.rec_pid);

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
        public IActionResult Record(Models.Record.a70Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a70SIS c = new BO.a70SIS();
                if (v.rec_pid > 0) c = Factory.a70SISBL.Load(v.rec_pid);

                c.a70ScopFlag = v.Rec.a70ScopFlag;
                c.a70Name = v.Rec.a70Name;

                c.a70Code = v.Rec.a70Code;
                c.a70Description = v.Rec.a70Description;
                c.a70WsLogin = v.Rec.a70WsLogin;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a70SISBL.Save(c);
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
