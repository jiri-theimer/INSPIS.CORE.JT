using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Ij76NamedQueryBL
    {
        public BO.j76NamedQuery Load(int j76id);

        
        public int Save(BO.j76NamedQuery rec, List<BO.j77NamedQueryRow> lisJ77);
        
        public IEnumerable<BO.j76NamedQuery> GetList(string strEntity);
        public IEnumerable<BO.j77NamedQueryRow> GetList_j77(int j76id,string prefix);
        public string getFiltrAlias(string prefix, BO.baseQuery mq);
        

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
       
        

        public int Save(BO.j76NamedQuery rec, List<BO.j77NamedQueryRow> lisJ77)
        {
            if (ValidateBeforeSave(rec, lisJ77) == false)
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
                      
            if (lisJ77 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("if EXISTS(select j77ID FROM j77NamedQueryRow WHERE j76ID=@pid) DELETE FROM j77NamedQueryRow WHERE j76ID=@pid", new { pid = intJ76ID });
                }
                foreach (var c in lisJ77)
                {
                    if (c.IsTempDeleted == true && c.j77ID > 0)
                    {
                        _db.RunSql("DELETE FROM j77NamedQueryRow WHERE j77ID=@pid", new { pid = c.j77ID });
                    }
                    else
                    {
                        p = new DL.Params4Dapper();
                        p.AddInt("pid", c.j77ID, true);
                        p.AddInt("j76ID", intJ76ID, true);
                        p.AddString("j77Column", c.j77Column);
                        p.AddString("j77Operator", c.j77Operator);
                        p.AddInt("j77ComboValue", c.j77ComboValue);
                        p.AddInt("j77DatePeriodFlag", c.j77DatePeriodFlag);
                        if (c.j77DatePeriodFlag > 0)
                        {
                            c.j77Date1 = null; c.j77Date2 = null;
                        }
                        p.AddDateTime("j77Date1", c.j77Date1);
                        p.AddDateTime("j77Date2", c.j77Date2);
                        p.AddDouble("j77Num1", c.j77Num1);
                        p.AddDouble("j77Num2", c.j77Num2);
                        p.AddString("j77Value", c.j77Value);
                        p.AddString("j77ValueAlias", c.j77ValueAlias);
                        p.AddInt("j77Ordinal", c.j77Ordinal);
                        p.AddString("j77Op", c.j77Op);
                        p.AddString("j77BracketLeft", c.j77BracketLeft);
                        p.AddString("j77BracketRight", c.j77BracketRight);
                        _db.SaveRecord("j77NamedQueryRow", p, c, false, true);
                    }

                }
                
            }

            return intJ76ID;
        }
        private bool ValidateBeforeSave(BO.j76NamedQuery rec, List<BO.j77NamedQueryRow> lisJ77)
        {
           if (string.IsNullOrEmpty(rec.j76Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (lisJ77 == null || lisJ77.Count()==0)
            {
                this.AddMessage("Filtr musí mít alespoň jeden řádek."); return false;
            }

            int x = 0; string lb = ""; string rb = "";
            foreach (var c in lisJ77.Where(p => p.IsTempDeleted == false))
            {
                x += 1;
                if (c.j77BracketLeft != null)
                {
                    lb += c.j77BracketLeft;
                }
                if (c.j77BracketRight != null)
                {
                    rb += c.j77BracketRight;
                }

                switch (c.FieldType)
                {
                    case "date":
                        if (c.j77Operator == "INTERVAL" && c.j77Date1 == null && c.j77Date2 == null && c.j77DatePeriodFlag == 0)
                        {
                            this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] musí mít alespoň jedno vyplněné datum nebo pojmenované období."), x)); return false;
                        }
                        break;
                    case "string":
                        if (string.IsNullOrEmpty(c.j77Value) == true && (c.j77Operator == "CONTAINS" || c.j77Operator == "STARTS" || c.j77Operator == "EQUAL" || c.j77Operator == "NOT-EQUAL"))
                        {
                            this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
                        }
                        break;
                    case "combo":
                        if (c.j77ComboValue == 0 && (c.j77Operator == "EQUAL" || c.j77Operator == "NOT-EQUAL"))
                        {
                            this.AddMessageTranslated(string.Format(_mother.tra("Filtr řádek [{0}] obsahuje nevyplněnou hodnotu."), x)); return false;
                        }
                        break;
                    case "multi":
                        if (string.IsNullOrEmpty(c.j77Value) == true && (c.j77Operator == "EQUAL" || c.j77Operator == "NOT-EQUAL"))
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



        public IEnumerable<BO.j77NamedQueryRow> GetList_j77(int j76id,string prefix)
        {
            string s = "SELECT a.* FROM j77NamedQueryRow a WHERE a.j76ID=@j76id ORDER BY a.j77Ordinal";

            var lis = _db.GetList<BO.j77NamedQueryRow>(s, new { j76id = j76id });
            if (lis.Count() > 0)
            {
                var lisQueryFields = new BL.TheQueryFieldProvider(prefix).getPallete();
                foreach (var c in lis.Where(p => p.j77Column != null))
                {
                    if (lisQueryFields.Where(p => p.Field == c.j77Column).Count() > 0)
                    {
                        var cc = lisQueryFields.Where(p => p.Field == c.j77Column).First();
                        c.FieldType = cc.FieldType;
                        c.FieldEntity = cc.SourceEntity;
                        c.FieldSqlSyntax = cc.FieldSqlSyntax;
                        c.SqlWrapper = cc.SqlWrapper;
                        
                    }
                }
            }
            return lis;
        }

        public string getFiltrAlias(string prefix, BO.baseQuery mq)
        {
            if (mq.lisJ77==null || mq.lisJ77.Count() == 0) return "";
            var lisFields = new BL.TheQueryFieldProvider(prefix).getPallete();

            var lis = new List<string>();

            foreach (var c in mq.lisJ77)
            {
                string ss = "";
                BO.TheQueryField cField = null;
                if (c.j77BracketLeft != null)
                {
                    ss += "(";
                }
                if (c.j77Op == "OR")
                {
                    ss += " OR ";
                }
                if (lisFields.Where(p => p.Field == c.j77Column).Count() > 0)
                {
                    cField = lisFields.Where(p => p.Field == c.j77Column).First();
                    string s = cField.Header;
                    if (_mother.CurrentUser.j03LangIndex > 0)
                    {
                        s = _mother.tra(s);
                    }
                    ss = "[" + s + "] ";
                }
                switch (c.j77Operator)
                {
                    case "EQUAL":
                        ss += "=";
                        break;
                    case "NOT-ISNULL":
                        ss += _mother.tra("Není prázdné");
                        break;
                    case "ISNULL":
                        ss += _mother.tra("Je prázdné");
                        break;
                    case "INTERVAL":
                        ss += _mother.tra("Je interval");
                        break;
                    case "GREATERZERO":
                        ss += _mother.tra("Je větší než nula");
                        break;
                    case "ISNULLORZERO":
                        ss += _mother.tra("Je nula nebo prázdné");
                        break;
                    case "NOT-EQUAL":
                        ss += _mother.tra("Není rovno");
                        break;
                    case "CONTAINS":
                        lis.Add(_mother.tra("Obsahuje"));
                        break;
                    case "STARTS":
                        ss += _mother.tra("Začíná na");
                        break;
                    default:
                        break;
                }
                if (c.j77ValueAlias != null)
                {
                    ss += c.j77ValueAlias;
                }
                else
                {
                    ss += c.j77Value;
                }
                if (c.j77DatePeriodFlag > 0)
                {
                    var cPeriods = new BO.CLS.ThePeriodProviderSupport();
                    var lisPeriods = cPeriods.GetPallete();

                    var d1 = lisPeriods.Where(p => p.pid == c.j77DatePeriodFlag).First().d1;
                    var d2 = Convert.ToDateTime(lisPeriods.Where(p => p.pid == c.j77DatePeriodFlag).First().d2).AddDays(1).AddMinutes(-1);
                    ss += ": " + BO.BAS.ObjectDate2String(d1, "dd.MM.yyyy") + " - " + BO.BAS.ObjectDate2String(d2, "dd.MM.yyyy");
                }

                if (c.j77BracketRight != null)
                {
                    ss += ")";
                }
                lis.Add(ss);
            }

            return string.Join("; ", lis);
        }


    }
}
