using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace BO
{
    public class QRow
    {
        public string StringWhere { get; set; }
        public string ParName { get; set; }
        public object ParValue { get; set; }

        public string AndOrZleva { get; set; } = "AND";

        public string BracketLeft { get; set; }
        public string BracketRight { get; set; }

        public string Par2Name { get; set; }
        public object Par2Value { get; set; }

    }

    public abstract class baseQuery
    {
        private string _pkfield;
        private string _prefix;
        private List<QRow> _lis;

        public string Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
                _pkfield = "a." + _prefix + "ID";
            }
        }
        public string PkField
        {
            get
            {
                return _pkfield;
            }
        }
        public List<int> pids;
        public void SetPids(string strPids)
        {
            this.pids = BO.BAS.ConvertString2ListInt(strPids);

        }
        public int TopRecordsOnly { get; set; }
        public int OFFSET_PageSize { get; set; }
        public int OFFSET_PageNum { get; set; }

        public IEnumerable<BO.TheGridColumn> explicit_columns { get; set; }
        public string explicit_orderby { get; set; }
        public string explicit_selectsql { get; set; }
        public string explicit_sqlwhere { get; set; }
        public BO.RunningUser CurrentUser;
        public List<BO.TheGridColumnFilter> TheGridFilter { get; set; }     //sloupcový filtr
        public IEnumerable<BO.j73TheGridQuery> lisJ73 { get; set; }         //uložený filtr z návrháře sloupců
        public bool MyRecordsDisponible { get; set; }
        public bool? IsRecordValid { get; set; }
        public List<int> o51ids { get; set; }

        public string param1 { get; set; }
        public DateTime? global_d1;
        private DateTime? _global_d2;
        public DateTime? global_d2
        {
            get
            {
                return _global_d2;
            }
            set
            {
                if (value != null)
                {
                    _global_d2 = BO.BAS.ConvertDateTo235959(Convert.ToDateTime(value));     //převést datum-do na čas 23:59:59
                }
                else
                {
                    _global_d2 = value;
                }
            }
        }

        protected string _searchstring;
        public string SearchString
        {
            get
            {
                return _searchstring;
            }
            set
            {
                _searchstring = value;
                if (!string.IsNullOrEmpty(_searchstring))
                {
                    _searchstring = _searchstring.ToLower().Trim();
                    _searchstring = _searchstring.Replace("--", "").Replace("drop", "").Replace("delete", "").Replace("truncate", "").Replace(";", " or ").Replace(",", " or ").Replace("  ", " ");
                    _searchstring = _searchstring.Replace(" or ", "#or#").Replace(" and ", "#and#");
                    _searchstring = _searchstring.Replace(" ", " and ");
                    _searchstring = _searchstring.Replace("#or#", " or ").Replace("#and#", " and ");
                }
                
            }
        }

        public virtual List<QRow> GetRows()
        {
            return InhaleRows();

        }
        protected List<QRow> InhaleRows()
        {
           
            if (this.pids != null && this.pids.Any())
            {
                AQ(_pkfield + " IN (" + String.Join(",", this.pids) + ")", "", null);
            }
            if (this.IsRecordValid != null)
            {
                if (this.IsRecordValid==true)
                {
                    AQ("a." + _prefix + "ValidUntil>GETDATE()", "", null);

                }
                if (this.IsRecordValid==false)
                {
                    AQ("GETDATE() NOT BETWEEN a." + _prefix + "ValidFrom AND a." + _prefix + "ValidUntil", "", null);
                }
            }
            if (this.o51ids != null && this.o51ids.Count > 0)
            {
                AQ("a." + _prefix + "ID IN (select o52RecordPid FROM o52TagBinding WHERE o51ID IN (" + string.Join(",", this.o51ids) + "))", null, null);
            }
            if (this.explicit_sqlwhere != null)
            {
                AQ(this.explicit_sqlwhere, "", null);
            }
            if (this.lisJ73 != null)
            {
                ParseJ73Query();
            }
            if (this.TheGridFilter != null)
            {
                ParseSqlFromTheGridFilter();  //složit filtrovací podmínku ze sloupcového filtru gridu
            }

            return _lis;
        }

        protected void AQ(string strWhere, string strParName, object ParValue, string strAndOrZleva = "AND", string strBracketLeft = null, string strBracketRight = null, string strPar2Name = null, object Par2Value = null)
        {
            if (_lis == null)
            {
                _lis = new List<QRow>();
            }
            if (_lis.Count == 0)
            {
                strAndOrZleva = ""; //první podmínka zleva
            }

            if (String.IsNullOrEmpty(strParName) == false && _lis.Where(p => p.ParName == strParName).Count() > 0)
            {
                return; //parametr strParName již byl dříve přidán
            }
            if (strWhere.Contains( " OR ",StringComparison.OrdinalIgnoreCase))
            {
                strWhere = "(" + strWhere + ")";    //v podmínce je operátor OR
            }
            _lis.Add(new QRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue, AndOrZleva = strAndOrZleva, BracketLeft = strBracketLeft, BracketRight = strBracketRight, Par2Name = strPar2Name, Par2Value = Par2Value });
        }

        



        private void ParseSqlFromTheGridFilter()
        {

            int x = 0;
            foreach (var filterrow in this.TheGridFilter)
            {
                var col = filterrow.BoundColumn;
                var strF = col.getFinalSqlSyntax_WHERE();

                x += 1;
                string parName = "par" + x.ToString();

                int endIndex = 0;
                string[] arr = new string[] { filterrow.value };
                if (filterrow.value.IndexOf(";") > -1)  //v podmnínce sloupcového filtru může být středníkem odděleno více hodnot!
                {
                    arr = filterrow.value.Split(";");
                    endIndex = arr.Count() - 1;
                }

                switch (filterrow.oper)
                {
                    case "1":   //IS NULL
                        AQ(strF + " IS NULL", "", null);
                        break;
                    case "2":   //IS NOT NULL
                        AQ(strF + " IS NOT NULL", "", null);
                        break;
                    case "10":   //větší než nula
                        AQ(strF + " > 0", "", null);
                        break;
                    case "11":   //je nula nebo prázdné
                        AQ("ISNULL(" + strF + ",0)=0", "", null);
                        break;
                    case "8":   //ANO
                        AQ(strF + " = 1", "", null);
                        break;
                    case "9":   //NE
                        AQ(strF + " = 0", "", null);
                        break;
                    case "3":   //obsahuje                
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(leva_zavorka(i, endIndex) + string.Format(strF + " LIKE '%'+@{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR"); ;
                            }

                        }

                        break;
                    case "5":   //začíná na 
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(leva_zavorka(i, endIndex) + string.Format(strF + " LIKE @{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR");
                            }

                        }

                        break;
                    case "6":   //je rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(leva_zavorka(i, endIndex) + string.Format(strF + " = @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }

                        }

                        break;
                    case "4":   //interval
                        AQ(string.Format(strF + " >= @{0}", parName + "c1"), parName + "c1", get_param_value(col.NormalizedTypeName, filterrow.c1value));
                        AQ(string.Format(strF + " <= @{0}", parName + "c2"), parName + "c2", get_param_value(col.NormalizedTypeName, filterrow.c2value));
                        break;
                    case "7":   //není rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(leva_zavorka(i, endIndex) + string.Format(strF + " <> @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }
                        }

                        break;
                }

            }



        }

        private string leva_zavorka(int i, int intEndIndex)
        {
            if (intEndIndex > 0 && i == 0)
            {
                return "(";
            }
            else
            {
                return "";
            }
        }
        private string prava_zavorka(int i, int intEndIndex)
        {
            if (intEndIndex > 0 && i == intEndIndex)
            {
                return ")";
            }
            else
            {
                return "";
            }
        }


        private void ParseJ73Query()    //zpracování vnitřní filtrovací podmínky z návrháře sloupců
        {
            int x = 0; string ss = ""; string strField = ""; string strAndOrZleva = "";
            if (this.lisJ73.Count() > 0)
            {
                this.lisJ73.First().j73BracketLeft += "(";
                this.lisJ73.Last().j73BracketRight += ")";
            }
            foreach (var c in this.lisJ73)
            {
                x += 1;
                ss = x.ToString();
                strField = c.j73Column;
                if (c.FieldSqlSyntax != null)
                {
                    strField = c.FieldSqlSyntax;
                }
                strAndOrZleva = c.j73Op;

                switch (c.j73Operator)
                {
                    case "ISNULL":
                        AQ( strField + " IS NULL", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "NOT-ISNULL":
                        AQ( strField + " IS NOT NULL", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "GREATERZERO":
                        AQ( "ISNULL(" + strField + ",0)>0", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "ISNULLORZERO":
                        AQ( "ISNULL(" + strField + ",0)=0", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "CONTAINS":                        
                        AQ( strField + " LIKE '%" + BO.BAS.GSS(c.j73Value) + "+%'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);

                        break;
                    case "STARTS":                        
                        AQ( strField + " LIKE '" + BO.BAS.GSS(c.j73Value) + "+%'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        
                        break;
                    case "INTERVAL":
                        if (c.FieldType == "date")
                        {
                            if (c.j73DatePeriodFlag > 0)
                            {
                                var cPeriods = new BO.CLS.ThePeriodProviderSupport();
                                var lisPeriods = cPeriods.GetPallete();                                
                                c.j73Date1 = lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d1;
                                c.j73Date2 = Convert.ToDateTime(lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d2).AddDays(1).AddMinutes(-1);
                            }
                            if (c.j73Date1 != null && c.j73Date2 != null)
                            {
                                
                                AQ( c.WrapFilter(strField + " BETWEEN " + BO.BAS.GD(c.j73Date1) + " AND " + BO.BAS.GD(c.j73Date2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                            }
                            else
                            {
                                if (c.j73Date1 != null)
                                {                                    
                                    AQ( c.WrapFilter(strField + ">=" + BO.BAS.GD(c.j73Date1)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                }
                                if (c.j73Date2 != null)
                                {                                    
                                    AQ( c.WrapFilter(strField + "<=" + BO.BAS.GD(c.j73Date2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                }
                            }

                        }
                        if (c.FieldType == "number")
                        {
                            
                            AQ( c.WrapFilter(strField + " BETWEEN " + BO.BAS.GN(c.j73Num1) + " AND " + BO.BAS.GN(c.j73Num2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        break;
                    case "EQUAL":
                    case "NOT-EQUAL":
                        string strOper = "=";
                        if (c.j73Operator == "NOT-EQUAL")
                        {
                            strOper = "<>";
                        }
                        if (c.FieldType == "bool" || c.FieldType == "bool1")
                        {
                            AQ( c.WrapFilter(strField + " " + strOper + " " + c.j73Value), "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        if (c.FieldType == "string")
                        {                            
                            AQ( strField + " " + strOper + " '" + BO.BAS.GSS(c.j73Value) + "'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        if (c.FieldType == "combo")
                        {
                            
                            AQ( c.WrapFilter(strField + " " + strOper + " " + c.j73ComboValue.ToString()), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);

                        }
                        if (c.FieldType == "multi")
                        {
                            strOper = "IN";
                            if (c.j73Operator == "NOT-EQUAL")
                            {
                                strOper = "NOT IN";
                            }
                            AQ( c.WrapFilter(strField + " " + strOper + " (" + c.j73Value + ")"), "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        break;
                }

            }
        }


        private object get_param_value(string colType, string colValue)
        {
            if (String.IsNullOrEmpty(colValue) == true)
            {
                return null;
            }
            if (colType == "num")
            {
                return BO.BAS.InDouble(colValue);
            }
            if (colType == "date")
            {
                return Convert.ToDateTime(colValue);
            }
            if (colType == "bool")
            {
                if (colValue == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return colValue;
        }


        

    }
}
