using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Primitives;

namespace UI.Controllers
{
    public class importController : BaseController
    {
        //public IActionResult file()
        //{

        //}
        public IActionResult a03(int j75id,string sheetname,int startrow,int endrow,string guid,string filename)
        {
            if (startrow == 0) startrow = 2;
            if (endrow == 0) endrow = 50000;
            if (string.IsNullOrEmpty(guid)) guid = BO.BAS.GetGuid();
            var v = new a03Import() { HeadersRow = startrow-1,StartRow=startrow,EndRow=endrow,FileName=filename,Guid=guid };

            if (string.IsNullOrEmpty(v.FileName))
            {
                if (Factory.o27AttachmentBL.GetTempFiles(v.Guid).Count() > 0)
                {
                    v.FileName = Factory.o27AttachmentBL.GetTempFiles(v.Guid).First().o27ArchiveFileName;
                }
            }
            if (string.IsNullOrEmpty(v.FileName))
            {
                return Redirect("/fileupload/singleupload?targetflag=a03import&guid=" + v.Guid);
            }
            if (string.IsNullOrEmpty(v.FileNameOrig))
            {
                v.FileNameOrig = Factory.o27AttachmentBL.GetTempFiles(v.Guid).Where(p=>p.o27ArchiveFileName==v.FileName).First().o27OriginalFileName;
            }

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
            

            using (var workbook = new XLWorkbook(Factory.App.TempFolder+"\\"+v.FileName))
            {
                v.HeadersRow = v.StartRow - 1;
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
                    return Redirect("/import/a03?j75id=" + pid.ToString()+"&sheetname="+v.SelectedSheet+"&startrow="+v.StartRow.ToString()+"&endrow="+v.EndRow.ToString()+"&guid="+v.Guid+"&filename="+v.FileName);
                }
                
                return View(v);
            }
            if (oper == "savechanges")
            {
                //uložit změny               
                var pid=handle_save_j75(v, v.Rec);
                if (pid > 0)
                {
                    return Redirect("/import/a03?j75id=" + v.SelectedJ75ID+"&sheetname="+v.SelectedSheet + "&startrow=" + v.StartRow.ToString() + "&endrow=" + v.EndRow.ToString() + "&guid=" + v.Guid + "&filename=" + v.FileName);
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
                    return Redirect("/import/a03?sheetname=" + v.SelectedSheet + "&startrow=" + v.StartRow.ToString() + "&endrow=" + v.EndRow.ToString() + "&guid=" + v.Guid + "&filename=" + v.FileName);
                }
                return View(v);
            }
            if (oper == "startrow")
            {
                
                v.MapCols = null;
                RefreshState(v);
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
        private bool validate_mapping(a03Import v)
        {
            if (v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == true).Count() > 0)
            {
                this.AddMessageTranslated(v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == true).First().Name);
                this.AddMessage("U minimálně jednoho zaškrtlého sloupce chybí mapování na cílové pole."); return false;
            }
            var lis = v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == false);
            if (lis.Count() < 3)
            {
                this.AddMessage("Musíte zaškrtnout a namapovat minimálně 2 sloupce."); return false;
            }
            if (lis.Select(p => p.TargetField).Count() > lis.Select(p => p.TargetField).Distinct().Count())
            {
                this.AddMessage("V mapování je duplicitně nastavený sloupec."); return false;
            }
            return true;
        }
        private int handle_save_j75(a03Import v,BO.j75ImportTemplate rec)
        {
            if (!validate_mapping(v))
            {
                return 0;
            }

            var lis = v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == false);

            rec.x29ID = 103;
            rec.j03ID = Factory.CurrentUser.pid;
            rec.j75Pairs = string.Join("|", lis.Select(p => "#" + p.Index.ToString() + ";" + p.TargetField));
            rec.a06ID = v.SelectedA06ID;
           return Factory.j75ImportTemplateBL.Save(rec);
            

        }

       private string GV(object val)
        {
            if (val == null)
            {
                return "";
            }
            return val.ToString();
        }
        private void handle_import(a03Import v)
        {
            if (!validate_mapping(v))
            {
                return;
            }
            var lisA21 = Factory.FBL.GetListA21();
            var lisA05 = Factory.a05RegionBL.GetList(new BO.myQuery("a05"));
            var lisA09=Factory.a09FounderTypeBL.GetList(new BO.myQuery("a09"));

            using (var workbook = new XLWorkbook(Factory.App.TempFolder+"\\"+v.FileName))
            {
                
                var sheet = workbook.Worksheets.First(p => p.Name == v.SelectedSheet);
                for (int row = v.StartRow; row <= v.EndRow; row++)
                {
                    var rec = new BO.a03Institution() { a06ID = v.SelectedA06ID };
                    foreach(var c in v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField)==false))
                    {
                        var strVal = GV(sheet.Cell(row, c.Index).Value.ToString());
                        switch (c.TargetField)
                        {
                            case "a03REDIZO":
                                rec.a03REDIZO = strVal;
                                break;
                            case "a03ICO":
                                rec.a03ICO = strVal;
                                break;
                            case "a03Name":
                                rec.a03Name = strVal;
                                break;
                            case "a03City":
                                rec.a03Name = strVal;
                                break;
                            case "a03Street":
                                rec.a03Street = strVal;
                                break;
                            case "a03PostCode":
                                rec.a03PostCode = strVal;
                                break;
                            case "a03Phone":
                                rec.a03Phone = strVal;
                                break;                           
                            case "a03Mobile":
                                rec.a03Mobile = strVal;
                                break;
                            case "a03Fax":
                                rec.a03Fax = strVal;
                                break;
                            case "a03Web":
                                rec.a03Web = strVal;
                                break;
                            case "a03Email":
                                rec.a03Email = strVal;
                                break;
                            case "a03DirectorFullName":
                                rec.a03DirectorFullName = GV(sheet.Cell(row, c.Index).Value.ToString());
                                break;
                            case "a03ShortName":
                                rec.a03ShortName = GV(sheet.Cell(row, c.Index).Value.ToString());
                                break;
                            case "a21Code":                                
                                if (strVal!= "" && lisA21.Where(p => p.a21Code == strVal).Count() > 0)
                                {
                                    rec.a21ID = lisA21.Where(p => p.a21Code == strVal).First().pid;
                                }
                                break;
                            case "bind_a03FounderCode":
                                if (strVal !="" && Factory.a03InstitutionBL.LoadByFounderCode(strVal, 0) != null)
                                {
                                    rec.a03ID_Founder = Factory.a03InstitutionBL.LoadByFounderCode(strVal, 0).pid;
                                }                                
                                break;
                            case "a05RZCode":
                                if (strVal !="" && lisA05.Where(p => p.a05RZCode == strVal).Count() > 0)
                                {
                                    rec.a05ID = lisA05.Where(p => p.a05RZCode == strVal).First().pid;
                                }
                                break;
                            case "a09ID":
                                if (BO.BAS.InInt(strVal)>0 && lisA09.Where(p => p.pid == BO.BAS.InInt(strVal)).Count() > 0)
                                {
                                    rec.a09ID = lisA09.Where(p => p.pid == BO.BAS.InInt(strVal)).First().pid;
                                }
                                break;
                            case "a09UIVCode":
                                if (strVal !="" && lisA09.Where(p => p.a09UIVCode == strVal).Count() > 0)
                                {
                                    rec.a09ID = lisA09.Where(p => p.a09UIVCode == strVal).First().pid;
                                }
                                break;
                            case "a03ValidFrom":
                                try
                                {
                                    rec.ValidFrom = sheet.Cell(row, c.Index).GetDateTime();
                                }
                                catch
                                {

                                }
                                
                                break;
                            case "a03ValidUntil":
                                try
                                {
                                    rec.ValidUntil = sheet.Cell(row, c.Index).GetDateTime();
                                }
                                catch
                                {

                                }

                                break;
                        }
                    }
                    

                }


            }
        }
    }
}
