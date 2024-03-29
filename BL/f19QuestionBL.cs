﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface If19QuestionBL : BaseInterface
    {
        public BO.f19Question Load(int pid);
        public BO.f19Question Load_Merged(int pid, BO.a11EventForm recA11); //načte otázku vč. sloučení s externími SQL zdroji dat
        public IEnumerable<BO.f19Question> GetList(BO.myQueryF19 mq, bool bolLoadTextboxMinMax=false);
        public IEnumerable<BO.f19Question> GetList_Merged(BO.myQueryF19 mq, BO.a11EventForm recA11);    //načte seznam otázek vč. sloučení s externími zdroji dat
        public int Save(BO.f19Question rec, List<int> f21ids);
        public IEnumerable<BO.f27LinkUrl> GetList_AllF27();
        public IEnumerable<BO.f21ReplyUnitJoinedF19> GetListJoinedF19_Merged(BO.myQueryXX1 mq, BO.a11EventForm recA11);

    }
    class f19QuestionBL : BaseBL, If19QuestionBL
    {
        public f19QuestionBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,f18.f18Name,f23.f23Name,x24.x24Name,f26.f26Name,f25.f25Name,f44.f44Name,f29.f29Name,f06.f06Name,f18.f06ID,");
            sb(_db.GetSQL1_Ocas("f19"));
            sb(" FROM f19Question a INNER JOIN f23ReplyType f23 ON a.f23ID=f23.f23ID INNER JOIN f18FormSegment f18 ON a.f18ID=f18.f18ID LEFT OUTER JOIN x24DataType x24 ON a.x24ID=x24.x24ID");
            sb(" LEFT OUTER JOIN f06Form f06 ON f18.f06ID=f06.f06ID LEFT OUTER JOIN f25ChessBoard f25 ON a.f25ID=f25.f25ID");
            sb(" LEFT OUTER JOIN f26BatteryBoard f26 ON a.f26ID=f26.f26ID LEFT OUTER JOIN f44QuestionCluster f44 ON a.f44ID=f44.f44ID LEFT OUTER JOIN f29PortalQuestionTab f29 ON a.f29ID=f29.f29ID");
            sb(strAppend);

            return sbret();
        }
        public BO.f19Question Load(int pid)
        {
            var rec = _db.Load<BO.f19Question>(GetSQL1(" WHERE a.f19ID=@pid"), new { pid = pid });
            if (rec.f23ID == 1)
            {
                //textbox                
                var lisF21 = _mother.f21ReplyUnitBL.GetList(new BO.myQueryF21() { f19id = rec.pid });
                if (lisF21.Count() > 0)
                {
                    var c = lisF21.First();
                    rec.TextBox_MinValue = c.f21MinValue;
                    rec.TextBox_MaxValue = c.f21MaxValue;
                    rec.TextBox_ExportValue = c.f21ExportValue;
                }
            }
            return rec;
        }

        public BO.f19Question Load_Merged(int pid, BO.a11EventForm recA11)
        {
            var rec = Load(pid);
            if (rec != null)
            {
                var keys = getMergeKeys(recA11);
                var lisX39 = _mother.x39ConnectStringBL.GetList(new BO.myQuery("x39")); //seznam všech connect stringů

                rec.f19Name = MergeOneExternalSqlValue(rec.f19Name, keys, lisX39);
                rec.f19SupportingText = MergeOneExternalSqlValue(rec.f19SupportingText, keys, lisX39);
                rec.TextBox_MinValue = MergeOneExternalSqlValue(rec.TextBox_MinValue, keys, lisX39);
                rec.TextBox_MaxValue = MergeOneExternalSqlValue(rec.TextBox_MaxValue, keys, lisX39);
                rec.f19EvalListSource = MergeOneExternalSqlValue(rec.f19EvalListSource, keys, lisX39);
                rec.f19ReadonlyExpression = MergeOneExternalSqlValue(rec.f19ReadonlyExpression, keys, lisX39);
                rec.f19SkipExpression = MergeOneExternalSqlValue(rec.f19SkipExpression, keys, lisX39);
                rec.f19RequiredExpression = MergeOneExternalSqlValue(rec.f19RequiredExpression, keys, lisX39);
                rec.f19CancelValidateExpression = MergeOneExternalSqlValue(rec.f19CancelValidateExpression, keys, lisX39);
            }

            return rec;
        }

        private List<BO.StringPair> getMergeKeys(BO.a11EventForm recA11)
        {
            var keys = new List<BO.StringPair>();
            keys.Add(new BO.StringPair() { Key = "@j03login", Value = _mother.CurrentUser.j03Login });
            keys.Add(new BO.StringPair() { Key = "@j03id", Value = _mother.CurrentUser.pid.ToString() });
            keys.Add(new BO.StringPair() { Key = "@a01id", Value = recA11.a01ID.ToString() });
            keys.Add(new BO.StringPair() { Key = "@a03id", Value = recA11.a03ID.ToString() });
            if (recA11.a03ID > 0)
            {
                var recA03 = _mother.a03InstitutionBL.Load(recA11.a03ID);
                if (recA03.a03REDIZO != null) keys.Add(new BO.StringPair() { Key = "@a03redizo", Value = recA03.a03REDIZO });
                if (recA03.a03ICO != null) keys.Add(new BO.StringPair() { Key = "@a03ico", Value = recA03.a03ICO });
            }

            return keys;
        }
        

        public IEnumerable<BO.f19Question> GetList(BO.myQueryF19 mq,bool bolLoadTextboxMinMax=false)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.f19Ordinal"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            if (!bolLoadTextboxMinMax)
            {
                return _db.GetList<BO.f19Question>(fq.FinalSql, fq.Parameters);
            }
            else
            {
                //načítat i atributy TextBox_MinValue, TextBox_MaxValue
                var lis =_db.GetList<BO.f19Question>(fq.FinalSql, fq.Parameters);
                var lisF21 = _mother.f21ReplyUnitBL.GetListJoinedF19(new BO.myQueryXX1() {f23id=1, f19ids = lis.Select(p=>p.pid).ToList() });
                foreach(var recF19 in lis.Where(p=>p.f23ID==1))
                {
                    if (lisF21.Where(p => p.f19ID == recF19.pid).Count() > 0)
                    {
                        var c = lisF21.Where(p => p.f19ID == recF19.pid).First();
                        recF19.TextBox_MinValue = c.f21MinValue;
                        recF19.TextBox_MaxValue = c.f21MaxValue;
                        recF19.TextBox_ExportValue = c.f21ExportValue;
                       
                    }
                }

                return lis;
            }
            

           
            
        }

        public IEnumerable<BO.f19Question> GetList_Merged(BO.myQueryF19 mq, BO.a11EventForm recA11)
        {
            //vrátí seznam otázek doplněný o sloučené výrazy z externích sql dotazů
            var lis = GetList(mq,true);
            var lisX39 = _mother.x39ConnectStringBL.GetList(new BO.myQuery("x39")); //seznam všech connect stringů
            var keys = getMergeKeys(recA11);
            

            foreach (var rec in lis)
            {
                rec.f19Name = MergeOneExternalSqlValue(rec.f19Name, keys, lisX39);
                rec.f19SupportingText = MergeOneExternalSqlValue(rec.f19SupportingText, keys, lisX39);
                rec.TextBox_MinValue = MergeOneExternalSqlValue(rec.TextBox_MinValue, keys, lisX39);
                rec.TextBox_MaxValue = MergeOneExternalSqlValue(rec.TextBox_MaxValue, keys, lisX39);
                rec.f19DefaultValue = MergeOneExternalSqlValue(rec.f19DefaultValue, keys, lisX39);
                rec.f19EvalListSource = MergeOneExternalSqlValue(rec.f19EvalListSource, keys, lisX39);
                rec.f19ReadonlyExpression = MergeOneExternalSqlValue(rec.f19ReadonlyExpression, keys, lisX39);
                rec.f19SkipExpression = MergeOneExternalSqlValue(rec.f19SkipExpression, keys, lisX39);
                rec.f19RequiredExpression = MergeOneExternalSqlValue(rec.f19RequiredExpression, keys, lisX39);
                rec.f19CancelValidateExpression = MergeOneExternalSqlValue(rec.f19CancelValidateExpression, keys, lisX39);
            }

            return lis;

        }

        private string MergeOneExternalSqlValue(string strValue, List<BO.StringPair> keys, IEnumerable<BO.x39ConnectString> lisX39)
        {
            if (string.IsNullOrEmpty(strValue))
            {
                return strValue;
            }
            if (!strValue.Contains("{") || !strValue.Contains("}"))
            {
                return strValue;
            }

            var cMerge = new BO.CLS.MergeContent();

            var sqls = cMerge.GetAllMergeExternalSqlsInContent(strValue, keys, lisX39);  //vrátí všechny SQL externí dotazy
            foreach (BO.MergeExternalSql sql in sqls.Where(p => p.ConnectString != null))
            {
                var dbExternal = new DL.DbHandler(sql.ConnectString, _mother.CurrentUser, _mother.App.LogFolder); //db connection na databázi                    
                string ret = null;
                if (sql.MergedSql.Contains("select ", StringComparison.OrdinalIgnoreCase))
                {
                    var dt = dbExternal.GetDataTable(sql.MergedSql);  //výsledek pro select syntaxy se natahuje přes datatable
                    System.Data.DataRow dbRow = dt.Rows[0];
                    if (dbRow[0] != DBNull.Value)
                    {
                        ret = dbRow[0].ToString();
                    }


                }
                else
                {
                    sql.MergedSql = "SELECT " + sql.MergedSql + " as Value";
                    try
                    {
                        ret = dbExternal.Load<BO.GetString>(sql.MergedSql).Value;
                    }
                    catch(Exception e)
                    {
                        ret = e.Message;
                    }
                    
                    
                }
                if (ret != null)
                {

                }
                strValue = strValue.Replace(sql.OrigExpr, ret);



            }

            return strValue;

        }

        public IEnumerable<BO.f21ReplyUnitJoinedF19> GetListJoinedF19_Merged(BO.myQueryXX1 mq, BO.a11EventForm recA11)
        {
            var lis = _mother.f21ReplyUnitBL.GetListJoinedF19(mq);
            var lisX39 = _mother.x39ConnectStringBL.GetList(new BO.myQuery("x39")); //seznam všech connect stringů
            var keys = getMergeKeys(recA11);

            foreach (var rec in lis)
            {
                rec.f19Name = MergeOneExternalSqlValue(rec.f19Name, keys, lisX39);
                rec.f19SupportingText = MergeOneExternalSqlValue(rec.f19SupportingText, keys, lisX39);
                rec.f21MinValue = MergeOneExternalSqlValue(rec.f21MinValue, keys, lisX39);
                rec.f21MaxValue = MergeOneExternalSqlValue(rec.f21MaxValue, keys, lisX39);                
            }

            return lis;

        }

        public int Save(BO.f19Question rec, List<int> f21ids)
        {
            if (!ValidateBeforeSave(ref rec, f21ids))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.f19ID);
                p.AddInt("f18ID", rec.f18ID, true);
                p.AddInt("f23ID", rec.f23ID, true);
                p.AddInt("x24ID", rec.x24ID, true);
                p.AddInt("f25ID", rec.f25ID, true);
                p.AddInt("f26ID", rec.f26ID, true);
                p.AddInt("f27ID", rec.f27ID, true);
                p.AddInt("f29ID", rec.f29ID, true);
                p.AddInt("f44ID", rec.f44ID, true);
                p.AddInt("f19TemplateID", rec.f19TemplateID, true);
                p.AddEnumInt("f19PortalPublishFlag", rec.f19PortalPublishFlag);
                p.AddBool("f19IsSearchablePortalField", rec.f19IsSearchablePortalField);
                p.AddBool("f19IsRequired", rec.f19IsRequired);
                p.AddBool("f19IsMultiselect", rec.f19IsMultiselect);
                p.AddBool("f19IsHorizontalDisplay", rec.f19IsHorizontalDisplay);

                p.AddInt("f19Ordinal", rec.f19Ordinal);
                p.AddString("f19Name", rec.f19Name);
                p.AddString("f19Ident", rec.f19Ident);
                p.AddString("f19SupportingText", rec.f19SupportingText);
                p.AddString("f19Hint", rec.f19Hint);
                p.AddString("f19StatID", rec.f19StatID);
                p.AddString("f19ReadonlyExpression", rec.f19ReadonlyExpression);
                p.AddString("f19SkipExpression", rec.f19SkipExpression);
                p.AddString("f19RequiredExpression", rec.f19RequiredExpression);
                p.AddString("f19DefaultValue", rec.f19DefaultValue);
                p.AddString("f19CancelValidateExpression", rec.f19CancelValidateExpression);
                p.AddString("f19CancelValidateExpression_Message", rec.f19CancelValidateExpression_Message);
                p.AddString("f19Regex", rec.f19Regex);
                p.AddBool("f19IsDefaultValueHTML", rec.f19IsDefaultValueHTML);
                p.AddString("f19LinkURL", rec.f19LinkURL);
                p.AddString("f19AllowedFileUploadExtensions", rec.f19AllowedFileUploadExtensions);
                p.AddInt("f19MaxUploadFiles", rec.f19MaxUploadFiles);

                p.AddString("f19EvalListSource", rec.f19EvalListSource);

                p.AddInt("f19ChessRow", rec.f19ChessRow);
                p.AddInt("f19ChessColumn", rec.f19ChessColumn);
                p.AddInt("f19CHLMaxAnswers", rec.f19CHLMaxAnswers);
                p.AddInt("f19MaxAllowedSize", rec.f19MaxAllowedSize);

                p.AddBool("f19IsTextboxMultiline", rec.f19IsTextboxMultiline);
                p.AddString("f19LinkerValue", rec.f19LinkerValue);
                p.AddString("f19EntityField", rec.f19EntityField);
                p.AddBool("f19IsEncrypted", rec.f19IsEncrypted);

                if (rec.f19UC == null) { rec.f19UC = BO.BAS.GetGuid(); }
                p.AddString("f19UC", rec.f19UC);

                int intPID = _db.SaveRecord("f19Question", p, rec);

                if (rec.ReplyControl == BO.ReplyKeyEnum.TextBox || (rec.ReplyControl == BO.ReplyKeyEnum.Checkbox && rec.f19IsMultiselect == false) || rec.ReplyControl == BO.ReplyKeyEnum.FileUpload || rec.ReplyControl == BO.ReplyKeyEnum.HtmlEditor || rec.ReplyControl == BO.ReplyKeyEnum.SummaryOverview || rec.ReplyControl == BO.ReplyKeyEnum.EvalList)
                {
                    var recF21 = new BO.f21ReplyUnit();
                    if (rec.pid > 0)
                    {
                        var lisF21 = _mother.f21ReplyUnitBL.GetList(new BO.myQueryF21() { f19id_onlyin = rec.pid });
                        if (lisF21.Count() > 0)
                        {
                            recF21 = lisF21.First();
                        }
                    }
                    recF21.f21MinValue = rec.TextBox_MinValue;
                    recF21.f21MaxValue = rec.TextBox_MaxValue;
                    recF21.f21ExportValue = rec.TextBox_ExportValue;
                    recF21.f21Name = rec.f21Name;
                    var intF21ID = _mother.f21ReplyUnitBL.Save(recF21);
                    if (rec.pid > 0 && intF21ID > 0)
                    {
                        _db.RunSql("DELETE FROM f20ReplyUnitToQuestion WHERE f21ID<>@f21id AND f19ID=@f19id", new { f21id = intF21ID, f19id = intPID });
                    }
                    if (intPID > 0 && intF21ID > 0)
                    {
                        _db.RunSql("INSERT INTO f20ReplyUnitToQuestion(f19ID,f21ID) SELECT @pid,f21ID FROM f21ReplyUnit WHERE f21ID=@f21id AND f21ID NOT IN (select f21ID FROM f20ReplyUnitToQuestion WHERE f19ID=@pid AND f21ID=@f21id)", new { pid = intPID, f21id = intF21ID });
                    }

                    f21ids = null;
                }
                if (f21ids != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM f20ReplyUnitToQuestion WHERE f19ID=@pid", new { pid = intPID });
                    }
                    if (f21ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO f20ReplyUnitToQuestion(f19ID,f21ID) SELECT @pid,f21ID FROM f21ReplyUnit WHERE f21ID IN (" + string.Join(",", f21ids) + ")", new { pid = intPID });
                    }
                }

                sc.Complete();
                return intPID;
            }




        }

        public bool ValidateBeforeSave(ref BO.f19Question rec, List<int> f21ids)
        {
            if (string.IsNullOrEmpty(rec.f19Name))
            {
                this.AddMessage("Chybí vyplnit [Název]: Hlavní text otázky."); return false;
            }
            if (rec.f25ID > 0 && rec.f26ID > 0)
            {
                this.AddMessage("Otázka nemůže mít vazbu současně na baterii i šachovnici."); return false;
            }
            if (rec.f25ID > 0 && rec.ReplyControl != BO.ReplyKeyEnum.TextBox)
            {
                this.AddMessage("K šachovnici se mohou vázat pouze TEXTBOX otázky."); return false;
            }
            if (rec.f25ID > 0 && (rec.f19ChessColumn <= 0 || rec.f19ChessRow <= 0))
            {
                this.AddMessage("Upravte index řádku a sloupce v šachovnici."); return false;
            }
            if (rec.f19PortalPublishFlag == BO.f19PortalPublishFlagENUM.None)
            {
                rec.f29ID = 0;
            }
            else
            {
                if (rec.f29ID == 0)
                {
                    this.AddMessage("Otázka musí být zařazena do konkrétního PORTAL boxu."); return false;
                }
            }
            if (rec.ReplyControl != BO.ReplyKeyEnum.EvalList)
            {
                rec.f19EvalListSource = "";
            }
            if (rec.f19TemplateID == 0) //u zděděné otázky neprobíhá vstupní kontrola
            {
                if (rec.x24ID == 0 && rec.ReplyControl == BO.ReplyKeyEnum.TextBox)
                {
                    this.AddMessage("Datový typ (formát) otázky je povinná pole."); return false;
                }
                if (rec.f18ID == 0)
                {
                    this.AddMessage("Segment formuláře je povinná pole."); return false;
                }
                if (rec.f23ID == 0)
                {
                    this.AddMessage("Typ odpovědi je povinná pole."); return false;
                }
                if (rec.ReplyControl == BO.ReplyKeyEnum.TextBox && rec.ReplyType == BO.x24DataTypeEnum.tString && rec.f19Regex != null)
                {
                    rec.f19Regex = rec.f19Regex.Trim();
                }
                else
                {
                    rec.f19Regex = "";
                }
                if (rec.ReplyControl == BO.ReplyKeyEnum.Button)
                {
                    if (string.IsNullOrEmpty(rec.f19LinkURL) == true && rec.f27ID == 0)
                    {
                        this.AddMessage("URL tlačítka není vyplněno."); return false;
                    }
                    if (rec.f27ID > 0)
                    {
                        rec.f19LinkURL = "";
                    }
                    if ((rec.ReplyControl == BO.ReplyKeyEnum.Checkbox && rec.f19IsMultiselect == true) || rec.ReplyControl == BO.ReplyKeyEnum.RadiobuttonList)
                    {
                        if (rec.f26ID > 0 && rec.f19IsHorizontalDisplay == true)
                        {
                            this.AddMessage("Položky odpovědi musí mít horizontální směr."); return false;
                        }
                    }
                }
                rec.f19ReadonlyExpression = OcistiEvalFunct(rec.f19ReadonlyExpression);
                rec.f19SkipExpression = OcistiEvalFunct(rec.f19SkipExpression);
                rec.f19DefaultValue = OcistiEvalFunct(rec.f19DefaultValue);
                rec.f19RequiredExpression = OcistiEvalFunct(rec.f19RequiredExpression);

                if (f21ids != null)
                {
                    if ((rec.ReplyControl == BO.ReplyKeyEnum.Checkbox && rec.f19IsMultiselect) || (rec.ReplyControl == BO.ReplyKeyEnum.DropdownList || rec.ReplyControl == BO.ReplyKeyEnum.RadiobuttonList || rec.ReplyControl == BO.ReplyKeyEnum.Listbox))
                    {
                        if (f21ids.Count <= 1)
                        {
                            this.AddMessage("V definičním oboru hodnot odpovědi musí být minimálně 2 položky."); return false;
                        }
                    }


                }

                if (rec.ReplyControl != BO.ReplyKeyEnum.TextBox)
                {
                    rec.x24ID = 0; //x24id je pouze pro textbox
                }
                if (rec.ReplyControl != BO.ReplyKeyEnum.Button)
                {
                    rec.f19LinkURL = "";
                }
                else
                {
                    rec.f19IsRequired = true;
                }
                if (rec.ReplyControl != BO.ReplyKeyEnum.FileUpload)
                {
                    rec.f19AllowedFileUploadExtensions = "";
                }
            }

            return true;
        }


        private string OcistiEvalFunct(string s)
        {
            return BO.BAS.Uvozovky2Apostrofy(s);
        }


        //private void SaveF12(int intPID,List<BO.f21ReplyUnit> lisF21)
        //{
        //    var recF19 = Load(intPID);
        //    bool bolVyctova = false;
        //    if (recF19.ReplyControl == BO.ReplyKeyEnum.DropdownList || recF19.ReplyControl == BO.ReplyKeyEnum.RadiobuttonList || recF19.ReplyControl == BO.ReplyKeyEnum.Listbox || (recF19.ReplyControl == BO.ReplyKeyEnum.Checkbox && recF19.f19IsMultiselect))
        //    {
        //        bolVyctova = true;
        //    }
        //    if (!bolVyctova)
        //    {

        //    }
        //}

        public IEnumerable<BO.f27LinkUrl> GetList_AllF27()
        {

            return _db.GetList<BO.f27LinkUrl>("select a.*," + _db.GetSQL1_Ocas("f27") + " FROM f27LinkUrl a");
        }

    }
}
