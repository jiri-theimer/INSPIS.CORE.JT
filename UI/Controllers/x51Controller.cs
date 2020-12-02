using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;
using Microsoft.AspNetCore.Hosting;


namespace UI.Controllers
{
    public class x51Controller : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public x51Controller(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(string viewurl, string pagetitle, int listflag)
        {
            
            var v = new x51RecPage() { InputViewUrl = viewurl,PageTitle=pagetitle,NearListFlag= listflag };
          
            if (string.IsNullOrEmpty(v.InputViewUrl) ==false)
            {
                                
                v.Rec = Factory.x51HelpCoreBL.LoadByViewUrl(v.InputViewUrl);
                if (v.Rec == null && v.InputViewUrl.Contains("="))
                {
                    v.InputViewUrl = v.InputViewUrl.Split("=").First();
                    v.Rec = Factory.x51HelpCoreBL.LoadByViewUrl(v.InputViewUrl);
                }
                if (v.Rec == null && v.InputViewUrl.Contains("?"))
                {
                    v.InputViewUrl = v.InputViewUrl.Split("?").First();
                    v.Rec = Factory.x51HelpCoreBL.LoadByViewUrl(v.InputViewUrl);
                }
                if (v.Rec != null)
                {
                    v.HtmlContent = Factory.x51HelpCoreBL.LoadHtmlContent(v.Rec.pid);

                    var tg = Factory.o51TagBL.GetTagging("x51", v.Rec.pid);
                    v.TagHtml = tg.TagHtml;
                }

                string strNear = v.InputViewUrl;
                if (v.InputViewUrl.Contains("/"))
                {
                    if (strNear.Contains("?"))
                    {
                        strNear = strNear.Split("?")[0];
                    }
                    var strRemove = strNear.Split("/").Last();
                    strNear = strNear.Replace("/" + strRemove,"");
                }
                v.lisNear = Factory.x51HelpCoreBL.GetList(new BO.myQuery("x51") { IsRecordValid = true }).Where(p => p.x51ViewUrl != null);
                switch (v.NearListFlag)
                {
                    case 1:
                        v.lisNear = v.lisNear.OrderBy(p => p.x51Name);
                        break;
                    default:
                        //v.lisNear = v.lisNear.Where(p => p.x51ViewUrl.ToUpper().StartsWith(strNear.ToUpper())).OrderBy(p => p.x51ViewUrl);
                        var hodnoty = new List<string>() { strNear };
                        if (v.Rec !=null && v.Rec.x51NearUrls != null)
                        {
                            hodnoty.InsertRange(1,BO.BAS.ConvertString2List(v.Rec.x51NearUrls, ","));
                        }
                        v.lisNear = v.lisNear.Where(p => hodnoty.Contains(p.x51ViewUrl));
                        break;
                }

            }
            else
            {
                v.NearListFlag = 1;   //pokud se necílí konkrétní url, pak automaticky rejstřík
                v.lisNear = Factory.x51HelpCoreBL.GetList(new BO.myQuery("x51") { IsRecordValid = true });

                v.HtmlContent += "<hr><img src='/images/splash_help.jpg'/>";




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
                c.x51NearUrls = v.Rec.x51NearUrls;
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