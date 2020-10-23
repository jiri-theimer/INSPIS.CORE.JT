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
    public class importController : BaseController
    {
        public IActionResult a03()
        {
            var v = new a03Import() { HeadersRow = 1 };

            v.FileFullPath = "c:\\temp\\import_test.xlsx";


            RefreshState(v);



            return View(v);
        }

        private void RefreshState(a03Import v)
        {
            v.Sheets = new List<BO.StringPair>();
            
            using (var workbook = new XLWorkbook(v.FileFullPath))
            {
                foreach (var c in workbook.Worksheets)
                {
                    var sp = new BO.StringPair() { Key = c.Name, Value = c.Name };
                    v.Sheets.Add(sp);
                }
                if (v.SelectedSheet == null)
                {
                    v.SelectedSheet = v.Sheets[0].Value;
                }
                var sheet = workbook.Worksheets.First(p => p.Name == v.SelectedSheet);
                if (v.MapCols == null)
                {
                    v.MapCols = new List<ImportMappingColumn>();
                    for (int col = 1; col <= 100; col++)
                    {
                        if (sheet.Cell(v.HeadersRow, col).Value !=null && string.IsNullOrEmpty(sheet.Cell(v.HeadersRow, col).Value.ToString())==false)
                        {
                            var c = new ImportMappingColumn() { Index = col, Name = sheet.Cell(v.HeadersRow, col).Value.ToString(), IsChecked = false };
                            v.MapCols.Add(c);
                        }

                    }
                }
                
                
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult a03(a03Import v,string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}
