using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a04Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a04Record() { rec_pid = pid, rec_entity = "a04" };
            v.Rec = new BO.a04Inspectorate();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a04InspectorateBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.a04Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a04Inspectorate c = new BO.a04Inspectorate();
                if (v.rec_pid > 0) c = Factory.a04InspectorateBL.Load(v.rec_pid);
                c.a04Name = v.Rec.a04Name;               
                c.a05ID = v.Rec.a05ID;                               
                c.a04Street = v.Rec.a04Street;
                c.a04City = v.Rec.a04City;
                c.a04PostCode = v.Rec.a04PostCode;
                c.a04Email = v.Rec.a04Email;                
                c.a04Mobile = v.Rec.a04Mobile;
                c.a04Phone = v.Rec.a04Phone;
                c.a04Fax = v.Rec.a04Fax;
                c.a04IsRegional = v.Rec.a04IsRegional;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a04InspectorateBL.Save(c);
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