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
    public class a08Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new a08RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a08ThemeBL.Load(v.pid);
                var tg = Factory.o51TagBL.GetTagging("a08", pid);
                v.TagHtml = tg.TagHtml;

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a08Record() { rec_pid = pid, rec_entity = "a08" };
            v.Rec = new BO.a08Theme();
            RefreshState(v);
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a08ThemeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                v.lisA12 = Factory.a08ThemeBL.GetListA12(v.rec_pid).ToList();
                foreach(var c in v.lisA12)
                {
                    c.TempGuid = BO.BAS.GetGuid();
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
        public IActionResult Record(Models.Record.a08Record v,string oper,int f06id,string guid,string f06ids)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add")
            {
                if (v.lisA12.Where(p => p.f06ID == f06id && p.IsTempDeleted==false).Count() > 0)
                {
                    this.AddMessage("Tento formulář již je v seznamu.");
                }
                else
                {
                    var recF06 = Factory.f06FormBL.Load(f06id);
                    var c = new BO.a12ThemeForm() { f06ID = recF06.pid, f06Name = recF06.f06Name,TempGuid=BO.BAS.GetGuid() };
                    v.lisA12.Add(c);
                }
                
                return View(v);
            }
            if (oper == "addmultiple" && f06ids !=null)
            {
                foreach(int intF06ID in BO.BAS.ConvertString2ListInt(f06ids))
                {
                    if (v.lisA12.Where(p => p.f06ID == intF06ID && p.IsTempDeleted == false).Count() == 0)
                    {
                        var recF06 = Factory.f06FormBL.Load(intF06ID);
                        var c = new BO.a12ThemeForm() { f06ID = recF06.pid, f06Name = recF06.f06Name, TempGuid = BO.BAS.GetGuid() };
                        v.lisA12.Add(c);
                    }
                }
                
                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA12.First(p => p.TempGuid == guid).IsTempDeleted = true;
                //var c = v.lisA12.First(p => p.TempGuid == guid);
                //v.lisA12.Remove(c);
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.a08Theme c = new BO.a08Theme();
                if (v.rec_pid > 0) c = Factory.a08ThemeBL.Load(v.rec_pid);
                c.a08Name = v.Rec.a08Name;
                c.a08Ident = v.Rec.a08Ident;
                c.a08Description = v.Rec.a08Description;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                c.pid = Factory.a08ThemeBL.Save(c, v.lisA12.Where(p => p.IsTempDeleted == false).ToList());
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(a08Record v)
        {
            if (v.lisA12 == null)
            {
                v.lisA12 = new List<BO.a12ThemeForm>();
            }
        }

    }
}