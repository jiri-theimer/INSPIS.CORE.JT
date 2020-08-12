using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Parameters;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a25Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone,int a01id)
        {
            var v = new a25Record() { rec_pid = pid, rec_entity = "a25",a01ID=a01id };
            
            v.Rec = new BO.a25EventFormGroup();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a25EventFormGroupBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            if (v.Rec.pid==0 && v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing.");

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec) { AllowClone = false };
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a25Record v)
        {
            RefreshState(v);
            if (ModelState.IsValid)
            {
                BO.a25EventFormGroup c = new BO.a25EventFormGroup();
                if (v.rec_pid > 0) c = Factory.a25EventFormGroupBL.Load(v.rec_pid);
                c.a25Name = v.Rec.a25Name;
                c.a25Color = v.Rec.a25Color;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                if (v.rec_pid == 0)
                {
                    v.SelectedA11IDs = v.SelectedA11IDs.Where(p => p > 0).ToList();
                }               
                if (v.rec_pid == 0 && v.SelectedA11IDs.Count() < 2)
                {
                    this.AddMessage("Musíte zaškrtnout minimálně 2 formuláře."); return View(v);
                }                
                c.pid = Factory.a25EventFormGroupBL.Save(c);
                if (c.pid > 0)
                {
                    if (v.rec_pid == 0 && v.SelectedA11IDs.Count() > 0)
                    {
                        foreach(int a11id in v.SelectedA11IDs)
                        {
                            var recA11 = Factory.a11EventFormBL.Load(a11id);
                            recA11.a25ID = c.pid;
                            Factory.a11EventFormBL.Save(recA11);
                        }
                    }
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(Models.Record.a25Record v)
        {
            if (v.a01ID > 0)
            {
                v.RecA01 = Factory.a01EventBL.Load(v.a01ID);
                var mq = new BO.myQuery("a11");
                mq.a01id = v.a01ID;
                v.lisA11 = Factory.a11EventFormBL.GetList(mq).Where(p => p.a11IsPoll == false);
            }
        }
    }

}