using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a41Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone, int a01id)
        {
            var v = new a41Record() { rec_pid = pid, rec_entity = "a41", a01ID = a01id };
            v.Rec = new BO.a41PersonToEvent();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a41PersonToEventBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.a01ID = v.Rec.a01ID;
                v.Person = v.Rec.PersonAsc;
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
        public IActionResult Record(Models.Record.a41Record v)
        {
            RefreshState(ref v);
            if (ModelState.IsValid)
            {
                BO.a41PersonToEvent c = new BO.a41PersonToEvent();
                if (v.rec_pid > 0) c = Factory.a41PersonToEventBL.Load(v.rec_pid);
                c.a01ID = v.a01ID;
                c.a45ID = v.Rec.a45ID;


                c.pid = Factory.a41PersonToEventBL.Save(c, true, null);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(ref Models.Record.a41Record v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);
        }



        //Přidat účastníky akce:
        public IActionResult Append(int a01id)
        {
            var v = new a41AppendViewModel() { a01ID = a01id };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppend(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }
            if (v.RecA01.isclosed)
            {
                return this.StopPage(true, "Tato akce je již uzavřena.");
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Append(Models.a41AppendViewModel v, string oper, string guid)
        {
            RefreshStateAppend(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add")
            {
                if (v.SelectedJ02ID > 0)
                {
                    var c = new BO.a41PersonToEvent() { TempGuid = BO.BAS.GetGuid() };

                    if (v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).Count() > 0)
                    {
                        if (v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).Count() > 0)
                        {
                            c = v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).First();
                            if (c.IsTempDeleted == true)
                            {
                                c.IsTempDeleted = false;
                                return View(v);
                            }
                            else
                            {
                                this.AddMessage("Tato osoba již je v seznamu.");
                                return View(v);
                            }
                        }
                    }
                    c.j02ID = v.SelectedJ02ID;
                    c.PersonCombo = Factory.j02PersonBL.Load(c.j02ID).FullNameDesc;
                    c.a45ID = BO.EventRoleENUM.Resitel;
                    c.a45Name = Factory.FBL.LoadA45((int)c.a45ID).a45Name;
                    c.a45IsManual = true;
                    v.lisA41.Add(c);
                    return View(v);
                }

                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA41.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {
                v.lisA41 = v.lisA41.Where(p => p.IsTempDeleted == false).ToList();
                foreach (var c in v.lisA41)
                {
                    c.a01ID = v.a01ID;                                       
                    if (Factory.a41PersonToEventBL.ValidateBeforeSave(c) == false)
                    {                        
                        return View(v);
                    }
                }
                foreach (var c in v.lisA41)
                {
                    var rec = new BO.a41PersonToEvent() { a01ID = v.a01ID };
                    rec.j02ID = c.j02ID;
                    rec.j11ID = c.j11ID;
                    rec.a45ID = c.a45ID;
                    Factory.a41PersonToEventBL.Save(rec, true, null);

                }
                v.SetJavascript_CallOnLoad(v.a01ID);
                return View(v);

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateAppend(a41AppendViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            if (v.lisA41 == null)
            {
                v.lisA41 = new List<BO.a41PersonToEvent>();
            }
            var mq = new BO.myQueryA41() { a01id = v.a01ID };            
            v.lisA41Saved = Factory.a41PersonToEventBL.GetList(mq);

        }
    }
}