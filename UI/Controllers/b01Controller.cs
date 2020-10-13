using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class b01Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new b01Record() { rec_pid = pid, rec_entity = "b01" };
            v.Rec = new BO.b01WorkflowTemplate();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.b01WorkflowTemplateBL.Load(v.rec_pid);
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
            return ViewTup(v, BO.j05PermValuEnum.WorkflowDesigner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.b01Record v)
        {

            if (ModelState.IsValid)
            {
                BO.b01WorkflowTemplate c = new BO.b01WorkflowTemplate();
                if (v.rec_pid > 0) c = Factory.b01WorkflowTemplateBL.Load(v.rec_pid);
                c.b01Name = v.Rec.b01Name;
               

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.b01WorkflowTemplateBL.Save(c);
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