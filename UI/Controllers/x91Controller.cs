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
    public class x91Controller : BaseController
    {
       
        public IActionResult Record(int pid, bool isclone, string viewurl, string source, string pagetitle)
        {
            var v = new x91Record() { rec_pid = pid, rec_entity = "x91" };
            v.Rec = new BO.x91Translate();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x91TranslateBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            
            v.Toolbar = new MyToolbarViewModel(v.Rec,false);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.AdminGlobal_Ciselniky);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x91Record v,string oper)
        {
            if (oper == "recovery")
            {
                Factory.Translator.Recovery();
                
                this.AddMessage("Překlad načten.","info");
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.x91Translate c = new BO.x91Translate();
                if (v.rec_pid > 0) c = Factory.x91TranslateBL.Load(v.rec_pid);
                c.x91Code = v.Rec.x91Code;
                c.x91Orig = v.Rec.x91Code;
                c.x91Lang1 = v.Rec.x91Lang1;
                c.x91Lang2= v.Rec.x91Lang2;
                c.x91Lang3 = v.Rec.x91Lang3;
                c.x91Lang4 = v.Rec.x91Lang4;


                c.pid = Factory.x91TranslateBL.Save(c);
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