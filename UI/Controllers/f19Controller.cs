using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class f19Controller : BaseController
    {
        public IActionResult Record(int pid, int f18id,int f06id, bool isclone)
        {
            var v = new f19Record() { rec_pid = pid, rec_entity = "f19", f18ID = f18id, UploadGuid = BO.BAS.GetGuid() };
            v.Rec = new BO.f19Question();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f19QuestionBL.Load(v.rec_pid);
                v.f18ID = v.Rec.f18ID;
                v.f06ID = v.Rec.f06ID;
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                
                if (v.Rec.ReplyControl == BO.ReplyKeyEnum.DropdownList || v.Rec.ReplyControl == BO.ReplyKeyEnum.RadiobuttonList || v.Rec.ReplyControl == BO.ReplyKeyEnum.Listbox || (v.Rec.ReplyControl == BO.ReplyKeyEnum.Checkbox && v.Rec.f19IsMultiselect))
                {
                    var lisF21 = Factory.f21ReplyUnitBL.GetList(new BO.myQueryF21() { f19id = v.rec_pid });
                    v.f21IDs = string.Join(",", lisF21.Select(p => p.pid));
                }
                //if (v.Rec.ReplyControl == BO.ReplyKeyEnum.TextBox && lisF21.Count()>0)
                //{                    
                //    v.Rec.TextBox_MinValue = lisF21.First().f21MinValue;
                //    v.Rec.TextBox_MaxValue = lisF21.First().f21MaxValue;
                //    v.Rec.TextBox_ExportValue = lisF21.First().f21ExportValue;

                //}
            }
            else
            {
                if (v.f18ID==0 && f06id > 0)
                {
                    var mq = new BO.myQuery("f18");
                    mq.IsRecordValid = true;
                    mq.f06id = f06id;
                    var lisF18 = Factory.f18FormSegmentBL.GetList(mq);
                    if (lisF18.Count() == 0)
                    {
                        return this.StopPage(true, "Formulář musí obsahovat minimálně jeden segment.",true);
                    }
                    v.f06ID = f06id;
                    v.f18ID = lisF18.First().f18ID;
                }
                v.Rec.f23ID = 1;
                v.Rec.f23Name = "Textbox";
                v.Rec.x24ID = 2;
                v.Rec.x24Name = "String";
            }            
            if (v.f18ID == 0)
            {
                return this.StopPage(true, "f18id missing");
            }

            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.FormDesigner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f19Record v,string oper,int f23id)
        {
            if(v.Rec.f18ID == 0)
            {
                v.Rec.f18ID = v.f18ID;
            }
            else
            {
                v.f18ID = v.Rec.f18ID;
            }
            RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.f19Question c = new BO.f19Question();
                if (v.rec_pid > 0) c = Factory.f19QuestionBL.Load(v.rec_pid);
                c.f18ID = v.Rec.f18ID;
                c.f23ID = v.Rec.f23ID;
                c.x24ID = v.Rec.x24ID;
                c.f25ID = v.Rec.f25ID;
                c.f26ID = v.Rec.f26ID;
                c.f44ID = v.Rec.f44ID;

                c.f19Ordinal = v.Rec.f19Ordinal;
                c.f19TemplateID = v.Rec.f19TemplateID;

                c.f19Name = v.Rec.f19Name;
                c.f19Ident = v.Rec.f19Ident;
                c.f19ChessRow = v.Rec.f19ChessRow;                
                c.f19ChessColumn = v.Rec.f19ChessColumn;

                c.f19SupportingText = v.Rec.f19SupportingText;
                c.f19Hint = v.Rec.f19Hint;

                c.f19IsDefaultValueHTML = v.Rec.f19IsDefaultValueHTML;
                c.f19ReadonlyExpression = v.Rec.f19ReadonlyExpression;

                c.f19DefaultValue = v.Rec.f19DefaultValue;

                c.f19StatID = v.Rec.f19StatID;
                c.f19IsMultiselect = v.Rec.f19IsMultiselect;
                c.f19IsHorizontalDisplay = v.Rec.f19IsHorizontalDisplay;
                c.f19IsRequired = v.Rec.f19IsRequired;
                c.f19ReadonlyExpression = v.Rec.f19ReadonlyExpression;
                c.f19SkipExpression = v.Rec.f19SkipExpression;
                c.f19RequiredExpression = v.Rec.f19RequiredExpression;
                c.f19CancelValidateExpression = v.Rec.f19CancelValidateExpression;
                c.f19CancelValidateExpression_Message = v.Rec.f19CancelValidateExpression_Message;

                c.x24ID = 0;
                switch (c.ReplyControl)
                {
                    case BO.ReplyKeyEnum.TextBox:
                        c.x24ID = v.Rec.x24ID;
                        c.f19Regex = v.Rec.f19Regex;
                        c.TextBox_MinValue = v.Rec.TextBox_MinValue;
                        c.TextBox_MaxValue = v.Rec.TextBox_MaxValue;
                        c.TextBox_ExportValue = v.Rec.TextBox_ExportValue;
                        break;
                    case BO.ReplyKeyEnum.Button:
                        c.f27ID = v.Rec.f27ID;
                        c.f19LinkURL = v.Rec.f19LinkURL;
                        break;
                    case BO.ReplyKeyEnum.FileUpload:
                        c.f19AllowedFileUploadExtensions = v.Rec.f19AllowedFileUploadExtensions;
                        c.f19MaxUploadFiles = v.Rec.f19MaxUploadFiles;
                        break;
                }

                c.f19PortalPublishFlag = v.Rec.f19PortalPublishFlag;
                c.f19IsSearchablePortalField = v.Rec.f19IsSearchablePortalField;
                c.f29ID = v.Rec.f29ID;
                c.f19CHLMaxAnswers = v.Rec.f19CHLMaxAnswers;
                c.f19EvalListSource = v.Rec.f19EvalListSource;
                
                c.f19EntityField = v.Rec.f19EntityField;
                c.f19LinkerValue = v.Rec.f19LinkerValue;
                c.f19IsEncrypted = v.Rec.f19IsEncrypted;
                c.f19MaxAllowedSize = v.Rec.f19MaxAllowedSize;
                c.f19IsTextboxMultiline = v.Rec.f19IsTextboxMultiline;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                List<int> f21ids = BO.BAS.ConvertString2ListInt(v.f21IDs);
                c.pid = Factory.f19QuestionBL.Save(c,f21ids);

                if (c.pid > 0)
                {
                    if (Factory.o27AttachmentBL.SaveChangesAndUpload(v.UploadGuid, 419, c.pid))
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                        return View(v);
                    }

                    
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(f19Record v)
        {
            v.RecF18 = Factory.f18FormSegmentBL.Load(v.f18ID);
            if (v.Rec.f18ID == 0)
            {
                v.Rec.f18ID = v.f18ID;
            }
            if (v.Rec.f18Name == null)
            {
                v.Rec.f18Name = v.RecF18.f18Name;
            }
            v.RecF06 = Factory.f06FormBL.Load(v.RecF18.f06ID);
            var mq = new BO.myQuery("f25");
            mq.f18id = v.f18ID;
            v.lisF25 = Factory.f25ChessBoardBL.GetList(mq);
            mq = new BO.myQuery("f26");
            mq.f18id = v.f18ID;
            v.lisF26 = Factory.f26BatteryBoardBL.GetList(mq);
        }
    }
}