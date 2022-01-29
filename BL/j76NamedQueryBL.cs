using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Ij76NamedQueryBL
    {
        public BO.j76NamedQuery Load(int j76id);

        
        public int Save(BO.j76NamedQuery rec, List<BO.j73TheGridQuery> lisJ73);
        
        public IEnumerable<BO.j76NamedQuery> GetList(string strEntity);
        public IEnumerable<BO.j73TheGridQuery> GetList_j73(int j76id,string prefix);
        

    }
    class j76NamedQueryBL : BaseBL, Ij76NamedQueryBL
    {

        public j76NamedQueryBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j76"));
            sb(" FROM j76NamedQuery a");
            sb(strAppend);
            return sbret();
        }
        


        
        
        public BO.j76NamedQuery Load(int j76id)
        {
            return _db.Load<BO.j76NamedQuery>(GetSQL1(" WHERE a.j76ID=@j76id"), new { j76id = j76id });
        }
       
        

        public int Save(BO.j76NamedQuery rec, List<BO.j73TheGridQuery> lisJ73)
        {
            if (ValidateBeforeSave(rec, lisJ73) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j76ID);
            p.AddString("j76Name", rec.j76Name);
            p.AddBool("j76IsPublic", rec.j76IsPublic);            
            p.AddInt("j03ID", rec.j03ID, true);

            p.AddString("j76Entity", rec.j76Entity);
            
           
            int intJ76ID = _db.SaveRecord("j76NamedQuery", p, rec);
            if (rec.pid > 0)
            {
                _db.RunSql("UPDATE x36UserParam set x36Value=@pid+'|'+@newvalue WHERE x36Key LIKE 'grid-filter-j76name%' AND x36Value LIKE @pid+'|%'", new { newvalue = rec.j76Name,pid=rec.pid.ToString() });
            }
                      
            if (lisJ73 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("if EXISTS(select j73ID FROM j73TheGridQuery WHERE j76ID=@pid) DELETE FROM j73TheGridQuery WHERE j76ID=@pid", new { pid = intJ76ID });
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
                        p.AddInt("j76ID", intJ76ID, true);
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
                        _db.SaveRecord("j73TheGridQuery", p, c, false, true);
                    }

                }
                
            }

            return intJ76ID;
        }
        private bool ValidateBeforeSave(BO.j76NamedQuery rec, List<BO.j73TheGridQuery> lisJ73)
        {
           if (string.IsNullOrEmpty(rec.j76Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (lisJ73 == null || lisJ73.Count()==0)
            {
                this.AddMessage("Filtr musí mít alespoň jeden řádek."); return false;
            }

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


            return true;
        }


        public IEnumerable<BO.j76NamedQuery> GetList(string strEntity)
        {            
            var p = new Dapper.DynamicParameters();            
            p.Add("entity", strEntity);

            var s = string.Format("SELECT a.*,{0} FROM j76NamedQuery a WHERE a.j76Entity=@entity", _db.GetSQL1_Ocas("j76"));
           

            return _db.GetList<BO.j76NamedQuery>(s, p);
        }



        public IEnumerable<BO.j73TheGridQuery> GetList_j73(int j76id,string prefix)
        {
            string s = "SELECT a.* FROM j73TheGridQuery a WHERE a.j76ID=@j76id ORDER BY a.j73Ordinal";

            var lis = _db.GetList<BO.j73TheGridQuery>(s, new { j76id = j76id });
            if (lis.Count() > 0)
            {
                var lisQueryFields = new BL.TheQueryFieldProvider(prefix).getPallete();
                foreach (var c in lis.Where(p => p.j73Column != null))
                {
                    if (lisQueryFields.Where(p => p.Field == c.j73Column).Count() > 0)
                    {
                        var cc = lisQueryFields.Where(p => p.Field == c.j73Column).First();
                        c.FieldType = cc.FieldType;
                        c.FieldEntity = cc.SourceEntity;
                        c.FieldSqlSyntax = cc.FieldSqlSyntax;
                        c.SqlWrapper = cc.SqlWrapper;
                        
                    }
                }
            }
            return lis;
        }

        


    }
}
