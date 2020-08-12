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
    public class a11Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new a11RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a11EventFormBL.Load(v.pid);
                v.RecA01 = Factory.a01EventBL.Load(v.Rec.a01ID);
                v.RecPermission = Factory.a01EventBL.InhalePermission(v.RecA01);

            }

            return View(v);
        }
        public IActionResult Record(int pid, bool isclone, int a01id)
        {
            var v = new a11Record() { rec_pid = pid, rec_entity = "a11", a01ID = a01id };
            v.Rec = new BO.a11EventForm();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a11EventFormBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.a01ID = v.Rec.a01ID;
                
            }
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing.");
            }
            RefreshState(ref v);
            v.Toolbar = new MyToolbarViewModel(v.Rec) { AllowArchive = false };
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a11Record v)
        {
            RefreshState(ref v);
            if (ModelState.IsValid)
            {                
                BO.a11EventForm c = Factory.a11EventFormBL.Load(v.rec_pid);               
                c.a11Description = v.Rec.a11Description;
                c.a37ID = v.Rec.a37ID;
                c.a11IsLocked = v.Rec.a11IsLocked;
                c.a25ID = v.Rec.a25ID;
                c.k01ID = v.Rec.k01ID;
                c.a11TeacherPID = v.Rec.a11TeacherPID;

                c.pid = Factory.a11EventFormBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(ref Models.Record.a11Record v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            var recA11 = Factory.a11EventFormBL.Load(v.rec_pid);
            v.RecF06 = Factory.f06FormBL.Load(recA11.f06ID);
        }



        //Přidat formuláře do akce:
        public IActionResult Append(int a01id)
        {
            var v = new a11AppendViewModel() { a01ID = a01id,SelectedKolikrat=1 };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppend(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Append(Models.a11AppendViewModel v, string oper, string guid)
        {
            RefreshStateAppend(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add")
            {
                if (v.SelectedF06ID > 0 && v.SelectedKolikrat>0)
                {
                    for(int i = 1; i <= v.SelectedKolikrat; i++)
                    {
                        var c = new BO.a11EventForm() { TempGuid = BO.BAS.GetGuid() };

                        c.f06ID = v.SelectedF06ID;
                        c.f06Name = v.SelectedForm;
                        c.a25ID = v.SelectedA25ID;
                        c.a25Name = v.SelectedA25Name;
                        c.a37ID = v.SelectedA37ID;
                        c.a37Name = v.SelectedA37Name;
                        c.a11Description = v.a11Description;

                        v.lisA11.Add(c);
                    }
                    
                    return View(v);
                }
                else
                {
                    this.AddMessage("Musíte vybrat formulář.");
                }

                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA11.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {
                v.lisA11 = v.lisA11.Where(p => p.IsTempDeleted == false).ToList();
                if (v.lisA11.Count() == 0)
                {
                    this.AddMessage("Pro uložení chybí výběr formulářů.");return View(v);
                }
                foreach (var c in v.lisA11)
                {
                    c.a01ID = v.a01ID;
                    if (Factory.a11EventFormBL.ValidateBeforeSave(c) == false)
                    {
                        return View(v);
                    }
                }
                foreach (var c in v.lisA11)
                {
                    var rec = new BO.a11EventForm() { a01ID = v.a01ID };
                    rec.f06ID = c.f06ID;
                    rec.a25ID = c.a25ID;
                    rec.a37ID = c.a37ID;
                    rec.a11Description = c.a11Description;
                    int intA11ID= Factory.a11EventFormBL.Save(rec);
                    if (intA11ID == 0)
                    {
                        return View(v);
                    }

                }
                v.SetJavascript_CallOnLoad(v.a01ID);
                return View(v);

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateAppend(a11AppendViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            if (v.lisA11 == null)
            {
                v.lisA11 = new List<BO.a11EventForm>();
            }
            var mq = new BO.myQuery("a11");
            mq.a01id = v.a01ID;
            v.lisA11Saved = Factory.a11EventFormBL.GetList(mq);

        }
    }
}