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
    public class h11Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new h11RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.h11NoticeBoardBL.Load(v.pid);
                var recHtml = Factory.h11NoticeBoardBL.LoadHtmlRecord(v.pid);
                if (recHtml != null)
                {
                    v.HtmlContent = recHtml.o11Html;
                }
               
                var tg = Factory.o51TagBL.GetTagging("h11", pid);
                v.TagHtml = tg.TagHtml;

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.H11Admin))
            {
                return RedirectToAction("Info", new { pid = pid });
            }
            var v = new h11Record() { rec_pid = pid, rec_entity = "h11" };            
            v.Rec = new BO.h11NoticeBoard();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.h11NoticeBoardBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var recHtml = Factory.h11NoticeBoardBL.LoadHtmlRecord(v.rec_pid);
                if (recHtml != null)
                {
                    v.HtmlContent = recHtml.o11Html;
                }
                
                var mq = new BO.myQueryJ04() { h11id = v.rec_pid };                
                v.j04IDs = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.pid));
                v.j04Names = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.j04Name));
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.H11Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.h11Record v)
        {

            if (ModelState.IsValid)
            {
                BO.h11NoticeBoard c = new BO.h11NoticeBoard();
                if (v.rec_pid > 0) c = Factory.h11NoticeBoardBL.Load(v.rec_pid);
                c.h11Name = v.Rec.h11Name;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                c.pid = Factory.h11NoticeBoardBL.Save(c,j04ids, v.HtmlContent,v.PlainText);
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