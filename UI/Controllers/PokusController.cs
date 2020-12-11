using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

using System.IO;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


using System.Text.Json;

namespace UI.Controllers
{
    public class PokusController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public PokusController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        public IActionResult Evaluator()
        {
            var v = new UI.Models.Pokus();
            //var c = new EVAL.Evaluator(Factory, 1640734);
            //var s=c.TryEval("a03institution_s('a03Name')");
            //this.AddMessage(s.ToString());
            return View(v);
        }

        private BO.myQuery GetGridQuery()
        {
            var mq = new BO.myQuery("a01");
            mq.a10id = 35;
            return mq;
        }

        public IActionResult Grid()
        {
            var v = new UI.Models.Pokus();
            v.myQueryGrid = GetGridQuery();
            v.ExtendPagerHtml = "<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='tg_switchflag(\"a01\",1)'>Zapnout spodní panel</button>";

            return View(v);

        }
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.TheGridColumnFilter> filter)
        {            
            var c = new UI.TheGridSupport(Factory, _colsProvider);
            c.extendpagerhtml= "HandleTheGridFilter<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='tg_switchflag(\"a01\",1)'>Zapnout spodní panel</button>";

            return c.Event_HandleTheGridFilter(tgi, filter, GetGridQuery());
        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi)
        {
            
            var c = new UI.TheGridSupport(Factory, _colsProvider);
            c.extendpagerhtml = "HandleTheGridOper<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='tg_switchflag(\"a01\",1)'>Zapnout spodní panel</button>";

            return c.Event_HandleTheGridOper(tgi, GetGridQuery());

        }
        public string HandleTheGridMenu(int j72id)
        {
            var c = new UI.TheGridSupport(Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(j72id);
        }


        public IActionResult PokusWord()
        {
            System.IO.File.Copy("c:\\temp\\hovado2.docx", "c:\\temp\\hovado3.docx", true);
            Package wordPackage = Package.Open("c:\\temp\\hovado3.docx", FileMode.Open, FileAccess.ReadWrite);

            var fields = new List<string>() { "a04city", "a04Name","a03name","a04street","a04phone","a04email" };

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(wordPackage))
            {                
                var body = wordDocument.MainDocumentPart.Document.Body;
                var allParas = wordDocument.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();
                foreach (var item in allParas)
                {
                    foreach(var fld in fields)
                    {
                        if (item.Text.Contains("«" + fld + "»",StringComparison.OrdinalIgnoreCase) || item.Text.Contains("<" + fld + ">", StringComparison.OrdinalIgnoreCase))
                        {
                            item.Text = item.Text.Replace("<" + fld + ">", "##"+fld,StringComparison.OrdinalIgnoreCase).Replace("«" + fld + "»", "##"+fld, StringComparison.OrdinalIgnoreCase);
                            
                        }
                    }
                    
                }
                foreach (HeaderPart headerPart in wordDocument.MainDocumentPart.HeaderParts)
                {
                    Header header = headerPart.Header;
                    var allHeaderParas = header.Descendants<Text>();
                    foreach (var item in allHeaderParas)
                    {
                        foreach (var fld in fields)
                        {
                            if (item.Text.Contains("«" + fld + "»", StringComparison.OrdinalIgnoreCase) || item.Text.Contains("<" + fld + ">", StringComparison.OrdinalIgnoreCase))
                            {
                                item.Text = item.Text.Replace("<" + fld + ">", "##" + fld).Replace("«" + fld + "»", "##" + fld);

                            }
                        }

                    }

                }

                foreach (FooterPart footerPart in wordDocument.MainDocumentPart.FooterParts)
                {
                    Footer footer = footerPart.Footer;
                    var allFooterParas = footer.Descendants<Text>();
                    foreach (var item in allFooterParas)
                    {
                        foreach (var fld in fields)
                        {
                            if (item.Text.Contains("«" + fld + "»", StringComparison.OrdinalIgnoreCase) || item.Text.Contains("<" + fld + ">", StringComparison.OrdinalIgnoreCase))
                            {
                                item.Text = item.Text.Replace("<" + fld + ">", "##" + fld).Replace("«" + fld + "»", "##" + fld);

                            }
                        }

                    }

                }

                  
                    wordDocument.MainDocumentPart.Document.Save();
            }

            return View();
        }
        public IActionResult Strom()
        {
            var v = new UI.Models.Pokus();
            RefreshState(v);

            var cc = new BO.myQueryA11() { a01id = 1, f06id = 2 };
           


            return View(v);
        }
        public IActionResult Telerik()
        {
            var v = new UI.Models.Pokus();
            v.kendoItems = new List<kendoTreeItem>();
            for (int i = 1; i <= 5; i++)
            {
                
                var c = new kendoTreeItem() { id = i.ToString(), text = "položka " + i.ToString(),expanded=false,imageUrl="/images/form.png" };
                c.prefix = "j02";
                if (i == 2) c.expanded = true;
                
                c.items = new List<kendoTreeItem>();
                
                for (int x = 1; x <= 9; x++)
                {
                    var cc = new kendoTreeItem() { id =i.ToString()+x.ToString(), text = "pod-položka " + i.ToString() + "-" + x.ToString() };
                    c.items.Add(cc);

                    cc.items = new List<kendoTreeItem>();
                    for (int y = 1; y <= 2; y++)
                    {
                        var dd = new kendoTreeItem() { id = i.ToString() + x.ToString()+y.ToString(), text = "super-položka " + i.ToString() + "-" + x.ToString()+"-"+y.ToString() };
                        cc.items.Add(dd);
                    }
                }

                v.kendoItems.Add(c);

            }
            v.JsonTreeDatasource=JsonSerializer.Serialize(v.kendoItems, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false,Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,IgnoreNullValues=true
            });

            //---------------------------------------------------
            v.treeNodes = new List<myTreeNode>();
            var lis = Factory.x32ReportTypeBL.GetList(new BO.myQuery("x32Report"));
            foreach (var rec in lis)
            {
                var c = new myTreeNode()
                {
                    TreeIndex = rec.x32TreeIndex,
                    TreeLevel = rec.x32TreeLevel,
                    Text = rec.x32Name,
                    TreeIndexFrom = rec.x32TreeIndexFrom,
                    TreeIndexTo = rec.x32TreeIndexTo,
                    Pid = rec.pid,
                    ParentPid = rec.x32ParentID,
                    Prefix = "x32",
                    Expanded = false

                };
                v.treeNodes.Add(c);

            }

            return View(v);
        }

        [HttpPost]
        public IActionResult Strom(Models.Pokus v)
        {
            
            RefreshState(v);
            return View(v);
        }

        public int test1()
        {
            var c = Factory.a11EventFormBL.LoadPoll(646278, "7076");
            


            return c.pid;
        }


        private void RefreshState(UI.Models.Pokus v)
        {
            v.treeNodes = new List<myTreeNode>();

            foreach (var rec in Factory.o13AttachmentTypeBL.GetList(new BO.myQuery("o13")))
            {
                var c = new myTreeNode()
                {
                    TreeIndex = rec.o13TreeIndex,
                    TreeLevel = rec.o13TreeLevel,
                    Text = rec.o13Name,
                    TreeIndexFrom = rec.o13TreeIndexFrom,
                    TreeIndexTo = rec.o13TreeIndexTo,
                    Pid = rec.pid,
                    ParentPid = rec.o13ParentID,
                    Expanded=true,
                    Prefix="o13"


                };
                if (c.TreeIndexTo > c.TreeIndexFrom)
                {
                    c.CssClass = "cervena";
                }
                else
                {
                    c.ImgUrl = "/images/type_text.png";
                    c.Prefix = "";
                }
                v.treeNodes.Add(c);

            }
        }
    }

}