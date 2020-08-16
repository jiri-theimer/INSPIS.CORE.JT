using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ij72TheGridTemplateBL
    {
        public BO.j72TheGridTemplate Load(int intJ72ID);
        public int Save(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73, List<int> j04ids, List<int> j11ids);
        public IEnumerable<BO.j72TheGridTemplate> GetList(string strEntity, int intJ03ID, string strMasterEntity);
        public IEnumerable<BO.j73TheGridQuery> GetList_j73(BO.j72TheGridTemplate rec);


    }

    class j72TheGridTemplateBL : BaseBL, Ij72TheGridTemplateBL
    {

        public j72TheGridTemplateBL(BL.Factory mother) : base(mother)
        {

        }

        public BO.j72TheGridTemplate Load(int intJ72ID)
        {
            return _db.Load<BO.j72TheGridTemplate>(string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72ID=@j72id", _db.GetSQL1_Ocas("j72")), new { j72id = intJ72ID });
        }





        public int Save(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73, List<int> j04ids, List<int> j11ids)
        {
            if (ValidateBeforeSave(rec, lisJ73) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j72ID);
            p.AddString("j72Name", rec.j72Name);
            p.AddBool("j72IsSystem", rec.j72IsSystem);
            p.AddInt("j03ID", rec.j03ID, true);

            p.AddString("j72Entity", rec.j72Entity);
            p.AddString("j72MasterEntity", rec.j72MasterEntity);
            p.AddString("j72Columns", rec.j72Columns);
            p.AddInt("j72SplitterFlag", rec.j72SplitterFlag);


            p.AddBool("j72IsNoWrap", rec.j72IsNoWrap);
            p.AddInt("j72SelectableFlag", rec.j72SelectableFlag);
            p.AddBool("j72IsPublic", rec.j72IsPublic);


            if (lisJ73 != null)
            {
                p.AddBool("j72HashJ73Query", false);
            }

            int intJ72ID = _db.SaveRecord("j72TheGridTemplate", p.getDynamicDapperPars(), rec);

            if (j04ids != null && j11ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("if EXISTS(select j74ID FROM j74TheGridReceiver WHERE j72ID=@pid) DELETE FROM j74TheGridReceiver WHERE j72ID=@pid", new { pid = intJ72ID });
                }
                if (j04ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO j74TheGridReceiver(j72ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intJ72ID });
                }
                if (j11ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO j74TheGridReceiver(j72ID,j04ID) SELECT @pid,j11ID FROM j11Team WHERE j11ID IN (" + string.Join(",", j11ids) + ")", new { pid = intJ72ID });
                }
            }
            if (lisJ73 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("if EXISTS(select j73ID FROM j73TheGridQuery WHERE j72ID=@pid) DELETE FROM j73TheGridQuery WHERE j72ID=@pid", new { pid = intJ72ID });
                }
                foreach (var c in lisJ73)
                {
                    if (c.IsTempDeleted == true && c.j73ID > 0)
                    {
                        _db.RunSql("DELETE FROM j73TheGridQuery WHERE j73ID=@pid", new { pid = c.j73ID });
                    }
                    else
                    {
                        p = new DL.Params4Dapper();
                        p.AddInt("pid", c.j73ID, true);
                        p.AddInt("j72ID", intJ72ID, true);
                        p.AddString("j73Column", c.j73Column);
                        p.AddString("j73Operator", c.j73Operator);
                        p.AddInt("j73ComboValue", c.j73ComboValue);
                        p.AddInt("j73DatePeriodFlag", c.j73DatePeriodFlag);
                        if (c.j73DatePeriodFlag > 0)
                        {
                            c.j73Date1 = null; c.j73Date2 = null;
                        }
                        p.AddDateTime("j73Date1", c.j73Date1);
                        p.AddDateTime("j73Date2", c.j73Date2);
                        p.AddDouble("j73Num1", c.j73Num1);
                        p.AddDouble("j73Num2", c.j73Num2);
                        p.AddString("j73Value", c.j73Value);
                        p.AddString("j73ValueAlias", c.j73ValueAlias);
                        p.AddInt("j73Ordinal", c.j73Ordinal);
                        p.AddString("j73Op", c.j73Op);
                        p.AddString("j73BracketLeft", c.j73BracketLeft);
                        p.AddString("j73BracketRight", c.j73BracketRight);
                        _db.SaveRecord("j73TheGridQuery", p.getDynamicDapperPars(), c, false, true);
                    }

                }
                if (GetList_j73(Load(intJ72ID)).Count() > 0)
                {
                    _db.RunSql("UPDATE j72TheGridTemplate set j72HashJ73Query=1 WHERE j72ID=@pid", new { pid = intJ72ID });
                }
            }

            return intJ72ID;
        }
        private bool ValidateBeforeSave(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73)
        {
            if (string.IsNullOrEmpty(rec.j72Columns) == true)
            {
                this.AddMessage("GRID musí obsahovat minimálně jeden sloupec."); return false;
            }
            if (lisJ73 != null)
            {
                int x = 0; string lb = ""; string rb = "";
                foreach (var c in lisJ73.Where(p => p.IsTempDeleted == false))
                {
                    x += 1;
                    if (c.j73BracketLeft != null)
                    {
                        lb += c.j73BracketLeft;
                    }
                    if (c.j73BracketRight != null)
                    {
                        rb += c.j73BracketRight;
                    }

                    switch (c.FieldType)
                    {
                        case "date":
                            if (c.j73Operator == "INTERVAL" && c.j73Date1 == null && c.j73Date2 == null && c.j73DatePeriodFlag == 0)
                            {
                                this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] musí mít alespoň jedno vyplněné datum nebo pojmenované období."), x)); return false;
                            }
                            break;
                        case "string":
                            if (string.IsNullOrEmpty(c.j73Value) == true && (c.j73Operator == "CONTAINS" || c.j73Operator == "STARTS" || c.j73Operator == "EQUAL" || c.j73Operator == "NOT-EQUAL"))
                            {
                                this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
                            }
                            break;
                        case "combo":
                            if (c.j73ComboValue == 0 && (c.j73Operator == "EQUAL" || c.j73Operator == "NOT-EQUAL"))
                            {
                                this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
                            }
                            break;
                        case "multi":
                            if (string.IsNullOrEmpty(c.j73Value) == true && (c.j73Operator == "EQUAL" || c.j73Operator == "NOT-EQUAL"))
                            {
                                this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
                            }
                            break;
                    }
                }
                if (lb.Length != rb.Length)
                {
                    this.AddMessage(string.Format("Ve filtrovací podmínce nejsou správně závorky.", x)); return false;
                }
            }


            return true;
        }


        public IEnumerable<BO.j72TheGridTemplate> GetList(string strEntity, int intJ03ID, string strMasterEntity)
        {
            string s;
            var p = new Dapper.DynamicParameters();
            p.Add("j03id", intJ03ID);
            p.Add("entity", strEntity);
            p.Add("j04id", _mother.CurrentUser.j04ID);
            if (string.IsNullOrEmpty(strMasterEntity) == true)
            {
                s = string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72Entity=@entity AND a.j72MasterEntity IS NULL", _db.GetSQL1_Ocas("j72"));
            }
            else
            {
                s = string.Format("SELECT a.*,{0} FROM j72TheGridTemplate a WHERE a.j72Entity=@entity AND a.j72MasterEntity = @masterentity", _db.GetSQL1_Ocas("j72"));
                p.Add("masterentity", strMasterEntity);
            }
            s += " AND (a.j03ID=@j03id OR a.j72IsPublic=1 OR a.j72ID IN (select j72ID FROM j74TheGridReceiver WHERE j04ID=@j04id))";


            return _db.GetList<BO.j72TheGridTemplate>(s + " ORDER BY a.j72IsSystem DESC", p);
        }



        public IEnumerable<BO.j73TheGridQuery> GetList_j73(BO.j72TheGridTemplate rec)
        {
            string s = "SELECT a.* FROM j73TheGridQuery a WHERE a.j72ID=@j72id ORDER BY a.j73Ordinal";

            var lis = _db.GetList<BO.j73TheGridQuery>(s, new { j72id = rec.j72ID });
            if (lis.Count() > 0)
            {
                var lisQueryFields = new BL.TheQueryFieldProvider(rec.j72Entity.Substring(0, 3)).getPallete();
                foreach (var c in lis.Where(p => p.j73Column != null))
                {
                    if (lisQueryFields.Where(p => p.Field == c.j73Column).Count() > 0)
                    {
                        var cc = lisQueryFields.Where(p => p.Field == c.j73Column).First();
                        c.FieldType = cc.FieldType;
                        c.FieldEntity = cc.SourceEntity;
                        c.FieldSqlSyntax = cc.FieldSqlSyntax;
                        c.SqlWrapper = cc.SqlWrapper;
                        c.MasterPrefix = cc.MasterPrefix;
                        c.MasterPid = cc.MasterPid;
                    }
                }
            }
            return lis;
        }


    }
}