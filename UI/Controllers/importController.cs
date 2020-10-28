using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace UI.Controllers
{
    public class importController : BaseController
    {
        public IActionResult a03(int j75id,string sheetname,int startrow,int endrow)
        {
            if (startrow == 0) startrow = 2;
            if (endrow == 0) endrow = 50000;

            var v = new a03Import() { HeadersRow = 1,StartRow=startrow,EndRow=endrow };

            v.FileFullPath = "c:\\temp\\import_test.xlsx";
            if (j75id > 0)
            {
                v.SelectedJ75ID = j75id.ToString();
            }
           if (string.IsNullOrEmpty(sheetname) == false)
            {
                v.SelectedSheet = sheetname;
            }
            

            RefreshState(v);

            if (v.Rec != null)
            {
                var lis = BO.BAS.ConvertString2List(v.Rec.j75Pairs, "|");
                foreach(string s in lis)
                {
                    var arr = BO.BAS.ConvertString2List(s, ";");
                    var intIndex = BO.BAS.InInt(arr[0].Replace("#",""));
                    v.MapCols[intIndex-1].IsChecked = true;
                    v.MapCols[intIndex - 1].TargetField = arr[1];
                    v.MapCols[intIndex - 1].CssTrStyle = "background-color:lightyellow;";
                }
                v.SelectedA06ID = v.Rec.a06ID;
            }
            else
            {
                v.SelectedJ75ID = null;
            }

            return View(v);
        }

        private void RefreshState(a03Import v)
        {
            v.Sheets = new List<BO.StringPair>();
            v.lisJ75 = Factory.j75ImportTemplateBL.GetList(new BO.myQuery("j75")).Where(p => p.x29ID == 103);
            if (BO.BAS.InInt(v.SelectedJ75ID) > 0)
            {
                v.Rec = Factory.j75ImportTemplateBL.Load(BO.BAS.InInt(v.SelectedJ75ID));
            }
            else
            {
                v.SelectedJ75ID = null;
            }
            

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
        public IActionResult a03(a03Import v,string oper,string j75name)
        {
            RefreshState(v);
            if (oper== "createtemplate" && string.IsNullOrEmpty(j75name)==false)
            {
                //uložit jako novou šablonu
                var rec = new BO.j75ImportTemplate() { j75Name = j75name };
                var pid=handle_save_j75(v, rec);
                if (pid > 0)
                {
                    return Redirect("/import/a03?j75id=" + pid.ToString()+"&sheetname="+v.SelectedSheet+"&startrow="+v.StartRow.ToString()+"&endrow="+v.EndRow.ToString());
                }
                
                return View(v);
            }
            if (oper == "savechanges")
            {
                //uložit změny               
                var pid=handle_save_j75(v, v.Rec);
                if (pid > 0)
                {
                    return Redirect("/import/a03?j75id=" + v.SelectedJ75ID+"&sheetname="+v.SelectedSheet + "&startrow=" + v.StartRow.ToString() + "&endrow=" + v.EndRow.ToString());
                }
                return View(v);
                
            }

            if (oper == "changesheet")
            {
                v.MapCols = null;
                RefreshState(v);
                return View(v);
            }
            if (oper== "deletetemplate")
            {
                if (Factory.j75ImportTemplateBL.Delete(BO.BAS.InInt(v.SelectedJ75ID))){
                    return Redirect("/import/a03?sheetname=" + v.SelectedSheet + "&startrow=" + v.StartRow.ToString() + "&endrow=" + v.EndRow.ToString());
                }
                return View(v);
            }
            
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

        private int handle_save_j75(a03Import v,BO.j75ImportTemplate rec)
        {
            if (v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == true).Count() > 0)
            {
                this.AddMessageTranslated(v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == true).First().Name);
                this.AddMessage("U minimálně jednoho zaškrtlého sloupce chybí mapování na cílové pole."); return 0;
            }
            var lis = v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == false);
            if (lis.Count() < 3)
            {
                this.AddMessage("Pro import šablonu musíte zaškrtnout a namapovat minimálně 2 sloupce.");return 0;
            }
            if (lis.Select(p=>p.TargetField).Count()>lis.Select(p=>p.TargetField).Distinct().Count())
            {                
                this.AddMessage("V mapování je duplicitně nastavený sloupec."); return 0;
            }
            
            rec.x29ID = 103;
            rec.j03ID = Factory.CurrentUser.pid;
            rec.j75Pairs = string.Join("|", lis.Select(p => "#" + p.Index.ToString() + ";" + p.TargetField));
            rec.a06ID = v.SelectedA06ID;
           return Factory.j75ImportTemplateBL.Save(rec);
            

        }
    }
}
