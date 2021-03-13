using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ij72TheGridTemplateBL
    {
        public BO.j72TheGridTemplate Load(int j72id);

        public BO.TheGridState LoadState(int j72id, int j03id);
        public BO.TheGridState LoadState(string strEntity, int j03id, string strMasterEntity);
        public int Save(BO.j72TheGridTemplate rec, List<BO.j73TheGridQuery> lisJ73, List<int> j04ids, List<int> j11ids);
        public int SaveState(BO.TheGridState rec, int j03id);
        public IEnumerable<BO.j72TheGridTemplate> GetList(string strEntity, int intJ03ID, string strMasterEntity);
        public IEnumerable<BO.j73TheGridQuery> GetList_j73(int j72id,string prefix);
        public string getFiltrAlias(string prefix, BO.baseQuery mq);        
        public BO.j72TheGridTemplate LoadTemplateGrid(string strEntity, string strMasterEntity);
        public bool DeleteSystemDefaultGrids(string strEntity, string strMasterEntity);

    }

    class j72TheGridTemplateBL : BaseBL, Ij72TheGridTemplateBL
    {

        public j72TheGridTemplateBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j72"));
            sb(" FROM j72TheGridTemplate a LEFT OUTER JOIN j75TheGridState j75 ON a.j72ID=j75.j72ID");
            sb(strAppend);
            return sbret();
        }
        private string GetSQL2(int j72id,string strAppend = null)
        {
            sb("SELECT a.*,j75.j75ID,");
            sb("j75.j75SortDataField,j75.j75SortOrder,j75.j75PageSize,j75.j75CurrentPagerIndex,j75.j75Filter,j75.j75HeightPanel1,j75.j75ColumnsGridWidth,j75.j75ColumnsReportWidth,");

            sb(_db.GetSQL1_Ocas("j72"));
            if (j72id > 0)
            {
                sb(" FROM j72TheGridTemplate a LEFT OUTER JOIN (select * from j75TheGridState WHERE j03ID=@j03id AND j72ID=@j72id) j75 ON a.j72ID=j75.j72ID");
            }
            else
            {
                sb(" FROM j72TheGridTemplate a LEFT OUTER JOIN (select * from j75TheGridState WHERE j03ID=@j03id) j75 ON a.j72ID=j75.j72ID");                
            }
            
            sb(strAppend);
            return sbret();
        }

        
        public bool DeleteSystemDefaultGrids(string strEntity, string strMasterEntity)
        {
            var p = new DL.Params4Dapper();
            p.AddString("entity", strEntity);
            p.AddString("masterentity", strMasterEntity);

            using (var sc = new System.Transactions.TransactionScope())
            {
                _db.RunSql("DELETE FROM j73TheGridQuery WHERE j72ID IN (SELECT j72ID FROM j72TheGridTemplate WHERE j72IsTemplate4SystemGrid=0 AND j72IsSystem=1 AND j72Entity=@entity AND ISNULL(j72MasterEntity,'') = ISNULL(@masterentity,''))", p.getDynamicDapperPars());

                _db.RunSql("DELETE FROM j74TheGridReceiver WHERE j72ID IN (SELECT j72ID FROM j72TheGridTemplate WHERE j72IsTemplate4SystemGrid=0 AND j72IsSystem=1 AND j72Entity=@entity AND ISNULL(j72MasterEntity,'') = ISNULL(@masterentity,''))", p.getDynamicDapperPars());

                _db.RunSql("DELETE FROM j75TheGridState WHERE j72ID IN (SELECT j72ID FROM j72TheGridTemplate WHERE j72IsTemplate4SystemGrid=0 AND j72IsSystem=1 AND j72Entity=@entity AND ISNULL(j72MasterEntity,'') = ISNULL(@masterentity,''))", p.getDynamicDapperPars());                                

                _db.RunSql("DELETE FROM j72TheGridTemplate WHERE j72IsTemplate4SystemGrid=0 AND j72IsSystem=1 AND j72Entity=@entity AND ISNULL(j72MasterEntity,'') = ISNULL(@masterentity,'')", p.getDynamicDapperPars());

                if (string.IsNullOrEmpty(strMasterEntity))
                {
                    _db.RunSql("DELETE FROM x36UserParam WHERE x36Key like '%flatview-j72id-" + strEntity.Substring(0, 3) + "%'");
                    _db.RunSql("DELETE FROM x36UserParam WHERE x36Key like '%admin-j72id-" + strEntity.Substring(0, 3) + "%'");
                }
                else
                {
                    _db.RunSql("DELETE FROM x36UserParam WHERE x36Key like '%slaveview-j72id-" + strEntity.Substring(0, 3) + "-"+strMasterEntity.Substring(0,3)+"%'");
                }
                

                sc.Complete();

                return true;
            }
        }
        public BO.j72TheGridTemplate LoadTemplateGrid(string strEntity,string strMasterEntity)
        {
            if (String.IsNullOrEmpty(strMasterEntity))
            {
                return _db.Load<BO.j72TheGridTemplate>(GetSQL1(" WHERE a.j72IsTemplate4SystemGrid=1 AND a.j72Entity=@entity"), new { entity = strEntity });
            }
            else
            {
                return _db.Load<BO.j72TheGridTemplate>(GetSQL1(" WHERE a.j72IsTemplate4SystemGrid=1 AND a.j72Entity=@entity AND a.j72MasterEntity=@masterentity"), new { entity = strEntity, masterentity = strMasterEntity });
            }
                
        }
        public BO.j72TheGridTemplate Load(int j72id)
        {
            return _db.Load<BO.j72TheGridTemplate>(GetSQL1(" WHERE a.j72ID=@j72id"), new { j72id = j72id });
        }
        public BO.TheGridState LoadState(int j72id,int j03id)
        {
            return _db.Load<BO.TheGridState>(GetSQL2(j72id," WHERE a.j72ID=@j72id"), new { j72id = j72id,j03id=j03id });
        }
        public BO.TheGridState LoadState(string strEntity, int j03id, string strMasterEntity)
        {   //načtení systémového gridu: j72IsSystem=1
            if (String.IsNullOrEmpty(strMasterEntity))
            {
                return _db.Load<BO.TheGridState>(GetSQL2(0," WHERE a.j72IsSystem=1 AND a.j72Entity=@entity AND a.j03ID=@j03id AND a.j72MasterEntity IS NULL"), new { entity = strEntity, j03id = j03id });
            }
            else
            {
                return _db.Load<BO.TheGridState>(GetSQL2(0," WHERE a.j72IsSystem=1 AND a.j72Entity=@entity AND a.j03ID=@j03id AND a.j72MasterEntity=@masterentity"), new { entity = strEntity, j03id = j03id, masterentity = strMasterEntity });
            }

        }

        public int SaveState(BO.TheGridState rec,int j03id)
        {
            rec.pid = rec.j75ID;
            if (rec.j75PageSize < 0) rec.j75PageSize = 100;

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j75ID);
            p.AddInt("j72ID", rec.j72ID, true);
            p.AddInt("j03ID",j03id , true);
            p.AddInt("j75PageSize", rec.j75PageSize);
            p.AddInt("j75CurrentPagerIndex", rec.j75CurrentPagerIndex);
            p.AddInt("j75CurrentRecordPid", rec.j75CurrentRecordPid);
            p.AddString("j75SortDataField", rec.j75SortDataField);
            p.AddString("j75SortOrder", rec.j75SortOrder);
            p.AddString("j75Filter", rec.j75Filter);
            p.AddInt("j75HeightPanel1", rec.j75HeightPanel1);

            int intJ75ID = _db.SaveRecord("j75TheGridState", p, rec,false,true);

            return intJ75ID;
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
            p.AddBool("j72IsTemplate4SystemGrid", rec.j72IsTemplate4SystemGrid);
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

            int intJ72ID = _db.SaveRecord("j72TheGridTemplate", p, rec);

            if (rec.j72IsTemplate4SystemGrid)
            {
                _db.RunSql("UPDATE j72TheGridTemplate set j72IsTemplate4SystemGrid=0 WHERE j72Entity=@entity AND j72ID<>@pid AND ISNULL(j72MasterEntity,'')=ISNULL(@masterentity,'')", new { entity = rec.j72Entity, pid = intJ72ID, masterentity=rec.j72MasterEntity });
            }

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
                        _db.SaveRecord("j73TheGridQuery", p, c, false, true);
                    }

                }
                if (GetList_j73(intJ72ID,rec.j72Entity.Substring(0,3)).Count() > 0)
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



        public IEnumerable<BO.j73TheGridQuery> GetList_j73(int j72id,string prefix)
        {
            string s = "SELECT a.* FROM j73TheGridQuery a WHERE a.j72ID=@j72id ORDER BY a.j73Ordinal";

            var lis = _db.GetList<BO.j73TheGridQuery>(s, new { j72id =j72id });
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
                        c.MasterPrefix = cc.MasterPrefix;
                        c.MasterPid = cc.MasterPid;
                    }
                }
            }
            return lis;
        }

        public string getFiltrAlias(string prefix, BO.baseQuery mq)
        {
            if (mq.lisJ73.Count() == 0) return "";
            var lisFields = new BL.TheQueryFieldProvider(prefix).getPallete();

            var lis = new List<string>();

            foreach (var c in mq.lisJ73)
            {
                string ss = "";
                BO.TheQueryField cField = null;
                if (c.j73BracketLeft != null)
                {
                    ss += "(";
                }
                if (c.j73Op == "OR")
                {
                    ss += " OR ";
                }
                if (lisFields.Where(p => p.Field == c.j73Column).Count() > 0)
                {
                    cField = lisFields.Where(p => p.Field == c.j73Column).First();
                    string s = cField.Header;
                    if (_mother.CurrentUser.j03LangIndex > 0)
                    {
                        s = _mother.tra(s);
                    }
                    ss = "[" + s + "] ";
                }
                switch (c.j73Operator)
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
                if (c.j73ValueAlias != null)
                {
                    ss += c.j73ValueAlias;
                }
                else
                {
                    ss += c.j73Value;
                }
                if (c.j73DatePeriodFlag > 0)
                {
                    var cPeriods = new BO.CLS.ThePeriodProviderSupport();
                    var lisPeriods = cPeriods.GetPallete();

                    var d1 = lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d1;
                    var d2 = Convert.ToDateTime(lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d2).AddDays(1).AddMinutes(-1);
                    ss += ": " + BO.BAS.ObjectDate2String(d1, "dd.MM.yyyy") + " - " + BO.BAS.ObjectDate2String(d2, "dd.MM.yyyy");
                }

                if (c.j73BracketRight != null)
                {
                    ss += ")";
                }
                lis.Add(ss);
            }

            return string.Join("; ", lis);
        }


    }
}