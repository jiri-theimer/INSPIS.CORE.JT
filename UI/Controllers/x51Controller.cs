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
    public class x51Controller : BaseController
    {
        public IActionResult Index(string viewurl, string pagetitle)
        {
            var v = new x51RecPage() { InputViewUrl = viewurl,PageTitle=pagetitle };
            if (string.IsNullOrEmpty(v.InputViewUrl) ==false)
            {
                if (v.InputViewUrl.Contains("?"))
                {
                    v.InputViewUrl = v.InputViewUrl.Split("?")[0];
                }
                v.Rec = Factory.x51HelpCoreBL.LoadByViewUrl(v.InputViewUrl);
                if (v.Rec != null)
                {
                    v.HtmlContent = Factory.x51HelpCoreBL.LoadHtmlContent(v.Rec.pid);

                    var tg = Factory.o51TagBL.GetTagging("x51", v.Rec.pid);
                    v.TagHtml = tg.TagHtml;
                }
                

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone,string viewurl,string source,string pagetitle)
        {
            var v = new x51Record() { rec_pid = pid, rec_entity = "x51",Source=source };
            v.Rec = new BO.x51HelpCore();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x51HelpCoreBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.HtmlContent = Factory.x51HelpCoreBL.LoadHtmlContent(v.rec_pid);
            }
            else
            {
                v.Rec.x51ViewUrl = viewurl;
                v.Rec.x51Name = pagetitle;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.HelpEditor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x51Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x51HelpCore c = new BO.x51HelpCore();
                if (v.rec_pid > 0) c = Factory.x51HelpCoreBL.Load(v.rec_pid);
                c.x51Name = v.Rec.x51Name;
                c.x51ExternalUrl = v.Rec.x51ExternalUrl;
                c.x51ViewUrl = v.Rec.x51ViewUrl;
                c.x51Html = v.HtmlContent;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.x51HelpCoreBL.Save(c);
                if (c.pid > 0)
                {
                    if (v.Source == "helppage")
                    {
                        return RedirectToAction("Index",new { viewurl = c.x51ViewUrl }) ;
                    }
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}