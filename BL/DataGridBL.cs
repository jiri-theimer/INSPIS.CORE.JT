using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace BL
{
    public interface IDataGridBL
    {
        public DataTable GetList(BO.myQuery mq, bool bolGetTotalsRow = false);
        //public BO.j72TheGridTemplate LoadTheGridState(int intJ72ID);
        //public BO.j72TheGridTemplate LoadTheGridState(string strEntity,int intJ03ID, string strMasterEntity);
        //public int SaveTheGridState(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73, List<int> j04ids, List<int> j11ids);
        //public IEnumerable<BO.j73TheGridQuery> GetList_j73(BO.j72TheGridTemplate rec);
        //public IEnumerable<BO.j72TheGridTemplate> GetList_j72(string strEntity, int intJ03ID, string strMasterEntity);

    }
    class DataGridBL:BaseBL,IDataGridBL
    {
       
        public DataGridBL(BL.Factory mother):base(mother)
        {
            
        }

        //public BO.j72TheGridTemplate LoadTheGridState(int intJ72ID)
        //{
        //    return _db.Load<BO.j72TheGridTemplate>(string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72ID=@j72id", _db.GetSQL1_Ocas("j72")), new { j72id = intJ72ID });
        //}
        //public BO.j72TheGridTemplate LoadTheGridState(string strEntity, int intJ03ID, string strMasterEntity)
        //{   //načtení systémového gridu: j72IsSystem=1
        //    if (String.IsNullOrEmpty(strMasterEntity))
        //    {
        //        return _db.Load<BO.j72TheGridTemplate>(string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72IsSystem=1 AND a.j72Entity=@entity AND a.j03ID=@j03id AND a.j72MasterEntity IS NULL", _db.GetSQL1_Ocas("j72")), new { entity = strEntity, j03id = intJ03ID });
        //    }
        //    else
        //    {
        //        return _db.Load<BO.j72TheGridTemplate>(string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72IsSystem=1 AND a.j72Entity=@entity AND a.j03ID=@j03id AND a.j72MasterEntity=@masterentity", _db.GetSQL1_Ocas("j72")), new { entity = strEntity, j03id = intJ03ID,masterentity=strMasterEntity });
        //    }
            
        //}


      
        public DataTable GetList(BO.myQuery mq,bool bolGetTotalsRow=false)
        {            
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ");
            if (mq.TopRecordsOnly > 0)
            {
                sb.Append("TOP "+mq.TopRecordsOnly.ToString()+" ");
            }
           
            if (mq.explicit_columns == null || mq.explicit_columns.Count()==0)
            {
                
                mq.explicit_columns = new BL.TheColumnsProvider(_mother.App,_mother.EProvider,_mother.Translator).getDefaultPallete(false,mq);    //na vstupu není přesný výčet sloupců -> pracovat s default sadou
            }
            if (bolGetTotalsRow)
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SUM())));   //součtová řádka gridu
            }
            else
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SELECT())));    //grid sloupce               
            }
            BO.TheEntity ce = _mother.EProvider.ByPrefix(mq.Prefix);
            
            if (bolGetTotalsRow == true)
            {
                sb.Append(string.Format(",COUNT(a.{0}ID) as RowsCount", mq.Prefix));     //sumační dotaz gridu
            }
            else
            {
                sb.Append(",");                
                sb.Append(_db.GetSQL1_Ocas(mq.Prefix, true, !ce.IsWithoutValidity));    //select dotaz gridu

            }
            
            if (mq.explicit_selectsql != null){
                sb.Append("," + mq.explicit_selectsql);
            }

            sb.Append(" FROM ");
            
            sb.Append(ce.SqlFromGrid);    //úvodní FROM klauzule s primární "a" tabulkou            
            
            List<string> relSqls = new List<string>();
            foreach (BO.TheGridColumn col in mq.explicit_columns.Where(x => x.RelName != null && x.RelName != "a"))
            {
                if (col.RelSqlDependOn != null && relSqls.Exists(p=>p==col.RelSqlDependOn)==false)
                {
                    relSqls.Add(col.RelSqlDependOn);
                    sb.Append(" ");
                    sb.Append(col.RelSqlDependOn);
                }
                if (relSqls.Exists(p => p == col.RelSql) == false)
                {
                    relSqls.Add(col.RelSql);
                    sb.Append(" ");
                    sb.Append(col.RelSql);
                }

            }
            
            
            //vždy musí být nějaké výchozí třídění v ce.SqlOrderBy!!
            if (bolGetTotalsRow == false && String.IsNullOrEmpty(mq.explicit_orderby)) mq.explicit_orderby = ce.SqlOrderBy;



            //parametrický dotaz s WHERE klauzulí

            DL.FinalSqlCommand q = DL.basQuery.ParseFinalSql(sb.ToString(),mq,_mother.CurrentUser, true);    //závěrečné vygenerování WHERE a ORDERBY klauzule

            if (bolGetTotalsRow == false && mq.OFFSET_PageSize > 0)
            {
                q.FinalSql += " OFFSET @pagesize*@pagenum ROWS FETCH NEXT @pagesize ROWS ONLY";
                if (q.Parameters4DT == null) q.Parameters4DT = new List<DL.Param4DT>();
                q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagesize", ParValue = mq.OFFSET_PageSize });
                q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagenum", ParValue = mq.OFFSET_PageNum });

            }
            
            return _db.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }


        //public int SaveTheGridState(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73, List<int> j04ids, List<int> j11ids)
        //{
        //    if (ValidateBeforeSave(rec,lisJ73) == false)
        //    {
        //        return 0;
        //    }
        //    var p = new DL.Params4Dapper();
        //    p.AddInt("pid", rec.j72ID);
        //    p.AddString("j72Name", rec.j72Name);
        //    p.AddBool("j72IsSystem", rec.j72IsSystem);
        //    p.AddInt("j03ID", rec.j03ID,true);
            
        //    p.AddString("j72Entity", rec.j72Entity);
        //    p.AddString("j72MasterEntity", rec.j72MasterEntity);
        //    p.AddString("j72Columns", rec.j72Columns);
        //    p.AddInt("j72SplitterFlag", rec.j72SplitterFlag);
        //    p.AddInt("j72HeightPanel1", rec.j72HeightPanel1);

        //    p.AddInt("j72PageSize", rec.j72PageSize);
        //    p.AddInt("j72CurrentPagerIndex", rec.j72CurrentPagerIndex);
        //    p.AddInt("j72CurrentRecordPid", rec.j72CurrentRecordPid);

        //    p.AddString("j72SortDataField", rec.j72SortDataField);            
        //    p.AddString("j72SortOrder", rec.j72SortOrder);
        //    p.AddString("j72Filter", rec.j72Filter);

        //    p.AddBool("j72IsNoWrap", rec.j72IsNoWrap);
        //    p.AddInt("j72SelectableFlag", rec.j72SelectableFlag);
        //    p.AddBool("j72IsPublic", rec.j72IsPublic);
            

        //    if (lisJ73 != null)
        //    {
        //        p.AddBool("j72HashJ73Query", false);
        //    }                

        //    int intJ72ID = _db.SaveRecord("j72TheGridTemplate", p.getDynamicDapperPars(), rec);
            
        //    if (j04ids != null && j11ids !=null)
        //    {
        //        if (rec.pid > 0)
        //        {
        //            _db.RunSql("if EXISTS(select j74ID FROM j74TheGridReceiver WHERE j72ID=@pid) DELETE FROM j74TheGridReceiver WHERE j72ID=@pid", new { pid = intJ72ID });
        //        }
        //        if (j04ids.Count > 0)
        //        {
        //            _db.RunSql("INSERT INTO j74TheGridReceiver(j72ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intJ72ID });
        //        }
        //        if (j11ids.Count > 0)
        //        {
        //            _db.RunSql("INSERT INTO j74TheGridReceiver(j72ID,j04ID) SELECT @pid,j11ID FROM j11Team WHERE j11ID IN (" + string.Join(",", j11ids) + ")", new { pid = intJ72ID });
        //        }
        //    }
        //    if (lisJ73 != null)
        //    {
        //        if (rec.pid > 0)
        //        {
        //            _db.RunSql("if EXISTS(select j73ID FROM j73TheGridQuery WHERE j72ID=@pid) DELETE FROM j73TheGridQuery WHERE j72ID=@pid", new { pid = intJ72ID });
        //        }
        //        foreach (var c in lisJ73)
        //        {
        //            if (c.IsTempDeleted==true && c.j73ID > 0)
        //            {
        //                _db.RunSql("DELETE FROM j73TheGridQuery WHERE j73ID=@pid", new { pid = c.j73ID });
        //            }
        //            else
        //            {
        //                p = new DL.Params4Dapper();
        //                p.AddInt("pid", c.j73ID, true);
        //                p.AddInt("j72ID", intJ72ID, true);
        //                p.AddString("j73Column", c.j73Column);
        //                p.AddString("j73Operator", c.j73Operator);
        //                p.AddInt("j73ComboValue", c.j73ComboValue);
        //                p.AddInt("j73DatePeriodFlag", c.j73DatePeriodFlag);
        //                if (c.j73DatePeriodFlag > 0)
        //                {
        //                    c.j73Date1 = null; c.j73Date2 = null;
        //                }
        //                p.AddDateTime("j73Date1", c.j73Date1);
        //                p.AddDateTime("j73Date2", c.j73Date2);
        //                p.AddDouble("j73Num1", c.j73Num1);
        //                p.AddDouble("j73Num2", c.j73Num2);
        //                p.AddString("j73Value", c.j73Value);
        //                p.AddString("j73ValueAlias", c.j73ValueAlias);
        //                p.AddInt("j73Ordinal", c.j73Ordinal);
        //                p.AddString("j73Op", c.j73Op);
        //                p.AddString("j73BracketLeft", c.j73BracketLeft);
        //                p.AddString("j73BracketRight", c.j73BracketRight);
        //                _db.SaveRecord("j73TheGridQuery", p.getDynamicDapperPars(), c, false, true);
        //            }
                    
        //        }
        //        if (GetList_j73(LoadTheGridState(intJ72ID)).Count() > 0)
        //        {
        //            _db.RunSql("UPDATE j72TheGridTemplate set j72HashJ73Query=1 WHERE j72ID=@pid", new { pid = intJ72ID });
        //        }
        //    }

        //    return intJ72ID;
        //}
        //private bool ValidateBeforeSave(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73)
        //{
        //    if (string.IsNullOrEmpty(rec.j72Columns)==true)
        //    {
        //        this.AddMessage("GRID musí obsahovat minimálně jeden sloupec."); return false;
        //    }
        //    if (lisJ73 != null)
        //    {
        //        int x = 0;string lb = "";string rb = "";
        //        foreach(var c in lisJ73.Where(p => p.IsTempDeleted == false))
        //        {
        //            x += 1;
        //            if (c.j73BracketLeft != null)
        //            {
        //                lb += c.j73BracketLeft;
        //            }
        //            if (c.j73BracketRight != null)
        //            {
        //                rb += c.j73BracketRight;
        //            }

        //            switch (c.FieldType)
        //            {
        //                case "date":
        //                    if (c.j73Operator == "INTERVAL" && c.j73Date1 == null && c.j73Date2 == null && c.j73DatePeriodFlag == 0)
        //                    {
        //                        this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] musí mít alespoň jedno vyplněné datum nebo pojmenované období."), x)); return false;
        //                    }
        //                    break;
        //                case "string":
        //                    if (string.IsNullOrEmpty(c.j73Value)==true && (c.j73Operator == "CONTAINS" || c.j73Operator == "STARTS" || c.j73Operator == "EQUAL" || c.j73Operator == "NOT-EQUAL"))
        //                    {
        //                        this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
        //                    }
        //                    break;
        //                case "combo":
        //                    if (c.j73ComboValue==0 && (c.j73Operator == "EQUAL" || c.j73Operator == "NOT-EQUAL"))
        //                    {
        //                        this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
        //                    }
        //                    break;
        //                case "multi":
        //                    if (string.IsNullOrEmpty(c.j73Value) == true && (c.j73Operator == "EQUAL" || c.j73Operator == "NOT-EQUAL"))
        //                    {
        //                        this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
        //                    }
        //                    break;
        //            }
        //        }
        //        if (lb.Length != rb.Length)
        //        {
        //            this.AddMessage(string.Format("Ve filtrovací podmínce nejsou správně závorky.", x)); return false;
        //        }
        //    }


        //    return true;
        //}


        //public IEnumerable<BO.j72TheGridTemplate> GetList_j72(string strEntity, int intJ03ID, string strMasterEntity)
        //{
        //    string s;
        //    var p = new Dapper.DynamicParameters();
        //    p.Add("j03id", intJ03ID);
        //    p.Add("entity", strEntity);
        //    p.Add("j04id", _mother.CurrentUser.j04ID);
        //    if (string.IsNullOrEmpty(strMasterEntity) == true)
        //    {
        //        s = string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72Entity=@entity AND a.j72MasterEntity IS NULL", _db.GetSQL1_Ocas("j72"));
        //    }
        //    else
        //    {
        //        s = string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72Entity=@entity AND a.j72MasterEntity = @masterentity", _db.GetSQL1_Ocas("j72"));
        //        p.Add("masterentity", strMasterEntity);
        //    }
        //    s += " AND (a.j03ID=@j03id OR a.j72IsPublic=1 OR a.j72ID IN (select j72ID FROM j74TheGridReceiver WHERE j04ID=@j04id))";


        //    return _db.GetList<BO.j72TheGridTemplate>(s + " ORDER BY a.j72IsSystem DESC", p);
        //}
        //public IEnumerable<BO.j73TheGridQuery> GetList_j73(BO.j72TheGridTemplate rec)
        //{
        //    string s = "SELECT a.* FROM j73TheGridQuery a WHERE a.j72ID=@j72id ORDER BY a.j73Ordinal";

        //    var lis= _db.GetList<BO.j73TheGridQuery>(s,new { j72id = rec.j72ID });
        //    if (lis.Count() > 0)
        //    {
        //        var lisQueryFields = new BL.TheQueryFieldProvider(rec.j72Entity.Substring(0, 3)).getPallete();
        //        foreach (var c in lis.Where(p => p.j73Column != null))
        //        {
        //            if (lisQueryFields.Where(p => p.Field == c.j73Column).Count() > 0)
        //            {
        //                var cc = lisQueryFields.Where(p => p.Field == c.j73Column).First();
        //                c.FieldType = cc.FieldType;
        //                c.FieldEntity = cc.SourceEntity;
        //                c.FieldSqlSyntax = cc.FieldSqlSyntax;
        //                c.SqlWrapper = cc.SqlWrapper;
        //                c.MasterPrefix = cc.MasterPrefix;
        //                c.MasterPid = cc.MasterPid;
        //            }                    
        //        }
        //    }
        //    return lis;
        //}
        
    }
}
