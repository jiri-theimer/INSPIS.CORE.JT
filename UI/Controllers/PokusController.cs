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


namespace UI.Controllers
{
    public class PokusController : BaseController
    {
        public IActionResult Evaluator()
        {
            var v = new UI.Models.Pokus();
            //var c = new EVAL.Evaluator(Factory, 1640734);
            //var s=c.TryEval("a03institution_s('a03Name')");
            //this.AddMessage(s.ToString());
            return View(v);
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

           



            return View(v);
        }
        public IActionResult Telerik()
        {
            var v = new UI.Models.Pokus();

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

            foreach (var rec in Factory.o13AttachmentTypeBL.GetList(new BO.myQuery("o13AttachmentType")))
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
                    Prefix = "o13"

                };
                v.treeNodes.Add(c);

            }
        }
    }

}