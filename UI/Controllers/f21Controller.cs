using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using ClosedXML.Excel;

namespace UI.Controllers
{
    public class f21Controller : BaseController
    {
        public IActionResult Import(string source)
        {
            var v = new f21ImportViewModel() { guid = BO.BAS.GetGuid(),source=source };

            return View(v);
        }
        [HttpPost]
        public IActionResult Import(f21ImportViewModel v,string oper)
        {
            
            if (oper == "import")
            {
                var lis = Factory.o27AttachmentBL.GetTempFiles(v.guid);
                if (lis.Count() == 0)
                {
                    this.AddMessage("Na vstupu chybí soubor XLS/XLSX soubor.");
                    return View(v);
                }
                
                var file=BO.BASFILE.GetFileInfo(lis.Last().FullPath);
                if (file.Extension.ToLower() !=".xls" &&file.Extension.ToLower() != ".xlsx")
                {
                    this.AddMessage("Vstupní soubor musí být ve formátu XLS/XLSX.");                   
                    return View(v);
                }
                v.lisPreview = ParseXlsData(v);
                if (v.lisPreview.Count() == 0)
                {
                    this.AddMessage("Vstupní soubor musí obsahovat alespoň jeden datový řádek.");
                    return View(v);
                }                
                v.istested = true;
                
                return View(v);
            }

            
            if (oper== "postback" && v.istested)
            {
                
                v.lisPreview = ParseXlsData(v);
            }
            
            if (oper == "save")
            {
                v.lisPreview = ParseXlsData(v);

                foreach(var rec in v.lisPreview)
                {
                    var intF21ID=Factory.f21ReplyUnitBL.Save(rec);
                    if (intF21ID > 0)
                    {
                        rec.f21ID = intF21ID;                        
                    }

                }

                if (v.lisPreview.Where(p => p.f21ID > 0).Count() > 0)
                {
                    v.saved_pids = string.Join(",", v.lisPreview.Where(p => p.f21ID > 0).Select(p=>p.f21ID));
                    v.SetJavascript_CallOnLoad(0, v.saved_pids, "window.parent.hardrefresh_afterimport");
                    return View(v);
                }
                
                
                
                
            }
            

            return View(v);
        }

        private List<BO.f21ReplyUnit> ParseXlsData(f21ImportViewModel v)
        {
            var lis = new List<BO.f21ReplyUnit>();

            var file = BO.BASFILE.GetFileInfo(Factory.o27AttachmentBL.GetTempFiles(v.guid).Last().FullPath);
            using (var workbook = new XLWorkbook(file.FullName))
            {
                var worksheet = workbook.Worksheets.First();

                for (int i = v.datafirstrowindex; i < 10000; i++)
                {
                    if (worksheet.Cell(i, 1).Value != null && !string.IsNullOrEmpty(worksheet.Cell(i, 1).Value.ToString()))
                    {
                        var rec = new BO.f21ReplyUnit();                       
                        rec.f21Name = worksheet.Cell(i, 1).Value.ToString();
                        if (worksheet.Cell(i, 2).Value != null)
                        {
                            rec.f21ExportValue = worksheet.Cell(i, 2).Value.ToString();
                        }
                        
                        if (worksheet.Cell(i, 3).Value != null)
                        {
                            rec.f21Ordinal = BO.BAS.InInt(worksheet.Cell(i, 3).Value.ToString());
                        }
                        if (worksheet.Cell(i, 4).Value != null)
                        {
                            rec.f21Description = worksheet.Cell(i, 4).Value.ToString();
                        }
                        lis.Add(rec);
                        
                    }

                }


            }

            return lis;
        }


        public IActionResult Record(int pid, bool isclone)
        {
            var v = new f21Record() { rec_pid = pid, rec_entity = "f21" };
            v.Rec = new BO.f21ReplyUnit();
           
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f21ReplyUnitBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.formular_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f21Record v)
        {
            RefreshState(v);
            if (ModelState.IsValid)
            {
                BO.f21ReplyUnit c = new BO.f21ReplyUnit();
                if (v.rec_pid > 0) c = Factory.f21ReplyUnitBL.Load(v.rec_pid);
                c.f21Name = v.Rec.f21Name;
                c.f21ExportValue = v.Rec.f21ExportValue;
                c.f21MinValue = v.Rec.f21MinValue;
                c.f21MaxValue = v.Rec.f21MaxValue;
                c.f21IsNegation = v.Rec.f21IsNegation;
                c.f21IsCommentAllowed = v.Rec.f21IsCommentAllowed;
                c.f21Description = v.Rec.f21Description;
                c.f21Ordinal = v.Rec.f21Ordinal;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.f21ReplyUnitBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(f21Record v)
        {
            
            v.lisF19 = Factory.f19QuestionBL.GetList(new BO.myQueryF19() { f21id = v.rec_pid });
            var mq = new BO.myQuery("f22");
            mq.f21id = v.rec_pid;
            v.lisF22 = Factory.f22ReplySetBL.GetList(mq);
        }
    }
}