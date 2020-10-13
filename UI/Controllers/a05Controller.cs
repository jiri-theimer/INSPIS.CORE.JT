using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a05Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a05Record() { rec_pid = pid, rec_entity = "a05" };
            v.Rec = new BO.a05Region();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a05RegionBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.a05Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a05Region c = new BO.a05Region();
                if (v.rec_pid > 0) c = Factory.a05RegionBL.Load(v.rec_pid);
                c.a05Name = v.Rec.a05Name;                
                c.a05UIVCode = v.Rec.a05UIVCode;
                c.a05VUSCCode = v.Rec.a05VUSCCode;
                c.a05RZCode = v.Rec.a05RZCode;
                c.a05Ordinal = v.Rec.a05Ordinal;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a05RegionBL.Save(c);
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