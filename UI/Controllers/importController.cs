﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Primitives;
using DocumentFormat.OpenXml.Wordprocessing;
using BO;

namespace UI.Controllers
{
    public class importController : BaseController
    {
        

        public IActionResult a03run(string guid)
        {
            var lisMapping = Factory.p85TempboxBL.GetList(guid).Where(p => p.p85Prefix == "mapping");
            var headerMapping = Factory.p85TempboxBL.GetList(guid).Where(p => p.p85Prefix == "header").First();

            var v = new a03ImportRun() { Guid=guid,StartRow= headerMapping.p85OtherKey2,EndRow= headerMapping.p85OtherKey3};
            return View(v);
        }
        public IActionResult a03(int j75id,string sheetname,int startrow,int endrow,string guid,string filename)
        {
            if (startrow == 0) startrow = 2;
            if (endrow == 0) endrow = 50000;
            if (string.IsNullOrEmpty(guid)) guid = BO.BAS.GetGuid();
            var v = new a03Import() { HeadersRow = startrow-1,StartRow=startrow,EndRow=endrow,FileName=filename,Guid=guid,GuidMapping=BO.BAS.GetGuid() };

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
            

            RefreshState(v,true);

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

        private void RefreshState(a03Import v,bool bolTestRowsCount)
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
                if (bolTestRowsCount)
                {                    
                    for (int row = v.StartRow; row <= v.EndRow; row++)
                    {                        
                        var strVal = GV(sheet.Cell(row, 1).Value.ToString());
                        if (strVal=="" && GV(sheet.Cell(row+1, 1).Value.ToString()) == "")
                        {
                            v.EndRow = row-1;
                            break;
                        }                        
                    }
                }
                
                
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult a03(a03Import v,string oper,string j75name)
        {
            RefreshState(v,false);
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
                RefreshState(v,false);
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
                RefreshState(v,false);
                return View(v);
            }
            if (oper== "runimport")
            {
                prepare_import(v);
                return Redirect("/import/a03run?guid=" + v.GuidMapping);
                
                
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

       private string GV(object val,int intMaxLength=0)
        {
            if (val == null)
            {
                return "";
            }
            if (intMaxLength > 0 && val.ToString().Length>intMaxLength)
            {
                return val.ToString().Substring(0, intMaxLength);
            }
            return val.ToString();
        }
        private void prepare_import(a03Import v)
        {//uložit namapování sloupců do db tempu
            if (!validate_mapping(v))
            {
                return;
            }

            if (Factory.p85TempboxBL.GetList(v.GuidMapping).Count() > 0)
            {
                v.GuidMapping = BO.BAS.GetGuid();
            }
            var rec = new BO.p85Tempbox() { p85Prefix = "header", p85GUID = v.GuidMapping, p85FreeText01 = v.FileName, p85FreeText02 = v.SelectedSheet,p85OtherKey1=v.SelectedA06ID,p85OtherKey2=v.StartRow,p85OtherKey3=v.EndRow };
            Factory.p85TempboxBL.Save(rec);

            foreach (var c in v.MapCols.Where(p => p.IsChecked == true && string.IsNullOrEmpty(p.TargetField) == false))
            {
                rec = new BO.p85Tempbox() {p85Prefix="mapping", p85GUID=v.GuidMapping, p85FreeText01 = c.TargetField,p85DataPID=c.Index };
                Factory.p85TempboxBL.Save(rec);
            }
        }
        public BO.Result handle_import(string strGuid)
        {
            var ret = new BO.Result(false);
            var lisMapping = Factory.p85TempboxBL.GetList(strGuid).Where(p=>p.p85Prefix== "mapping");
            var headerMapping = Factory.p85TempboxBL.GetList(strGuid).Where(p => p.p85Prefix == "header").First();
            var lisA21 = Factory.FBL.GetListA21();
            var lisA05 = Factory.a05RegionBL.GetList(new BO.myQuery("a05"));
            var lisA28 = Factory.a28SchoolTypeBL.GetList(new BO.myQuery("a28"));
            var lisA09=Factory.a09FounderTypeBL.GetList(new BO.myQuery("a09"));
            var lisA03 = Factory.a03InstitutionBL.GetList(new BO.myQueryA03());
            var errs = new List<string>();
            var recs = new List<BO.a03Institution>();
            int intRedIzoIndex = -1;
            if (lisMapping.Where(p => p.p85FreeText01 == "a03REDIZO").Count() > 0)
            {
                intRedIzoIndex = lisMapping.Where(p => p.p85FreeText01 == "a03REDIZO").First().p85DataPID;
            }
            int intStartRow = headerMapping.p85OtherKey2;
            int intEndRow = headerMapping.p85OtherKey3;
            int intBatchStartRow = headerMapping.p85OtherKey4;
            if (intBatchStartRow < intStartRow)
            {
                intBatchStartRow = intStartRow;
            }
            int intBatchEndRow = intBatchStartRow + 50;
            if (intBatchEndRow > intEndRow)
            {
                intBatchEndRow = intEndRow;
            }
           


            using (var workbook = new XLWorkbook(Factory.App.TempFolder+"\\"+headerMapping.p85FreeText01))
            {
                
                var sheet = workbook.Worksheets.First(p => p.Name == headerMapping.p85FreeText02);
                for (int row = intBatchStartRow; row <= intBatchEndRow; row++)
                {
                    var rec = new BO.a03Institution() { a06ID = headerMapping.p85OtherKey1 };
                    if (intRedIzoIndex > -1)
                    {
                        rec.a03REDIZO = GV(sheet.Cell(row, intRedIzoIndex).Value);  //najít již existující instituci podle REDIZO
                        if (rec.a03REDIZO != "" && lisA03.Where(p=>p.a03REDIZO==rec.a03REDIZO).Count()>0)
                        {
                            rec = lisA03.Where(p => p.a03REDIZO == rec.a03REDIZO).First();                           
                        }                        
                    }
                    foreach(var c in lisMapping)
                    {
                        var strVal = GV(sheet.Cell(row, c.p85DataPID).Value);
                        switch (c.p85FreeText01)
                        {
                            case "a03REDIZO":
                                rec.a03REDIZO = strVal;
                                break;
                            case "a03REDIZO_Parent":
                                if (strVal != "" && lisA03.Where(p => p.a03REDIZO == strVal).Count() > 0)
                                {
                                    rec.a03ID_Parent = lisA03.Where(p => p.a03REDIZO == strVal).First().pid;
                                    rec.a03ParentFlag = BO.a03ParentFlagEnum.Slave;
                                }
                                break;
                            case "a03ParentFlag":
                                switch (strVal)
                                {
                                    case "1":
                                        rec.a03ParentFlag = a03ParentFlagEnum.Master;
                                        break;
                                    case "2":
                                    case "Філія":
                                        rec.a03ParentFlag = a03ParentFlagEnum.Slave;
                                        break;
                                    default:
                                        rec.a03ParentFlag = a03ParentFlagEnum.None;
                                        break;
                                }

                                break;
                            case "a03LocationFlag":
                                switch (strVal)
                                {
                                    case "1":
                                        rec.a03LocationFlag = BO.a03LocationFlagEnum.Village;break;
                                    default:
                                        rec.a03LocationFlag = BO.a03LocationFlagEnum.City;break;
                                }
                                break;
                            case "a03REDIZO_Supervisory":
                                if (strVal != "" && lisA03.Where(p => p.a03REDIZO == strVal).Count() > 0)
                                {
                                    rec.a03ID_Supervisory = lisA03.Where(p => p.a03REDIZO == strVal).First().pid;                                   
                                }
                                break;
                            case "a03Name_Supervisory":
                                if (strVal != "" && lisA03.Where(p => p.a03Name == strVal || p.a03ShortName==strVal).Count() > 0)
                                {
                                    rec.a03ID_Supervisory = lisA03.Where(p => p.a03REDIZO == strVal || p.a03Name==strVal).First().pid;
                                }
                                break;
                            case "a03ICO":
                                rec.a03ICO = strVal;
                                break;
                            case "a03Name":
                                rec.a03Name = strVal;
                                break;
                            case "a03City":
                                rec.a03City = BO.BAS.LeftString(strVal,80);
                                break;
                            case "a03Street":
                                rec.a03Street = BO.BAS.LeftString(strVal, 255);                                
                                break;
                            case "a03PostCode":
                                rec.a03PostCode = BO.BAS.LeftString(strVal,50);
                                break;
                            case "a03Phone":
                                rec.a03Phone = BO.BAS.LeftString(strVal,50);
                                break;                           
                            case "a03Mobile":
                                rec.a03Mobile =BO.BAS.LeftString( strVal,50);
                                break;
                            case "a03Fax":
                                rec.a03Fax = BO.BAS.LeftString(strVal, 50);
                                break;
                            case "a03Web":
                                rec.a03Web = BO.BAS.LeftString(strVal, 200);
                                break;
                            case "a03Email":
                                rec.a03Email = BO.BAS.LeftString(strVal, 100);
                                break;
                            case "a03DirectorFullName":
                                rec.a03DirectorFullName = GV(sheet.Cell(row, c.p85DataPID).Value.ToString(),100);
                                break;
                            case "a03ShortName":
                                rec.a03ShortName = GV(sheet.Cell(row, c.p85DataPID).Value.ToString(),200);
                                break;
                            case "a21Code":                                
                                if (strVal!= "" && lisA21.Where(p => p.a21Code == strVal).Count() > 0)
                                {
                                    rec.a21ID = lisA21.Where(p => p.a21Code == strVal).First().pid;
                                }
                                break;
                            case "a28Code":
                                if (strVal != "" && lisA28.Where(p => p.a28Code == strVal).Count() > 0)
                                {
                                    rec.a28ID = lisA28.Where(p => p.a28Code == strVal).First().pid;
                                }
                                break;
                            case "a28Name":
                                if (strVal != "" && lisA28.Where(p => p.a28Name.ToLower() == strVal.ToLower()).Count() > 0)
                                {
                                    rec.a28ID = lisA28.Where(p => p.a28Name.ToLower() == strVal.ToLower()).First().pid;
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
                            case "a05Name":
                                if (strVal != "" && lisA05.Where(p => p.a05Name.ToLower() == strVal.ToLower()).Count() > 0)
                                {
                                    rec.a05ID = lisA05.Where(p => p.a05Name.ToLower() == strVal.ToLower()).First().pid;
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
                            case "a09Name":
                                if (strVal != "" && lisA09.Where(p => p.a09Name.ToUpper() == strVal.ToUpper()).Count() > 0)
                                {
                                    rec.a09ID = lisA09.Where(p => p.a09Name.ToUpper() == strVal.ToUpper()).First().pid;
                                }
                                break;
                            case "a03ValidFrom":
                                try
                                {
                                    rec.ValidFrom = sheet.Cell(row, c.p85DataPID).GetDateTime();
                                }
                                catch
                                {

                                }
                                
                                break;
                            case "a03ValidUntil":
                                try
                                {
                                    rec.ValidUntil = sheet.Cell(row, c.p85DataPID).GetDateTime();
                                }
                                catch
                                {

                                }

                                break;
                            
                        }
                        
                    }

                    if (Factory.a03InstitutionBL.ValidateBeforeSave(ref rec))
                    {
                        recs.Add(rec);
                    }
                    else
                    {
                        var cRecErr = new BO.p85Tempbox() { p85GUID = strGuid, p85Prefix = "error", p85FreeText01 = "#" + row.ToString(), p85Message = Factory.CurrentUser.Messages4Notify.Last().Key + ": " + Factory.CurrentUser.Messages4Notify.Last().Value };
                        Factory.p85TempboxBL.Save(cRecErr);
                        
                        errs.Add(row.ToString());
                        ret.Message += "<hr>#" + row.ToString()+" | "+Factory.CurrentUser.Messages4Notify.Last().Key + ": " + Factory.CurrentUser.Messages4Notify.Last().Value;
                        if (errs.Count() >= 10)
                        {
                            break;
                        }
                    }
                   

                }
                

                if (errs.Count() >= 10)
                {                   
                    ret.Message += "<hr>"+"Import pozastaven kvůli chybám.";                   
                    return ret;
                }

                foreach(var rec in recs)
                {
                    
                    Factory.a03InstitutionBL.Save(rec);
                }
                ret.Message = "Import #" + intBatchStartRow.ToString() + " - #" + intBatchEndRow.ToString();

                headerMapping.p85OtherKey4 = intBatchEndRow + 1;
                if (intBatchEndRow >= intEndRow)
                {
                    ret.pid = 1;    //import celkově dokončen
                    var lisErrs = Factory.p85TempboxBL.GetList(strGuid).Where(p => p.p85Prefix == "error");
                    foreach(var cc in lisErrs)
                    {
                        ret.Message += "<hr>" + cc.p85FreeText01 + " | " + cc.p85Message;
                    }
                }
                Factory.p85TempboxBL.Save(headerMapping);

                return ret;
            }
        }
    }
}
