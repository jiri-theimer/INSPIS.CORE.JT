using System;
using System.Collections.Generic;
using System.Linq;
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
        public int OFFSET_PageSize { get; set; }
        public int OFFSET_PageNum { get; set; }

        public IEnumerable<BO.TheGridColumn> explicit_columns { get; set; }
        public string explicit_orderby { get; set; }
        public string explicit_selectsql { get; set; }
        public string explicit_sqlwhere { get; set; }
        public BO.RunningUser CurrentUser;
        public List<BO.TheGridColumnFilter> TheGridFilter { get; set; }
        public bool MyRecordsDisponible { get; set; }
        public string param1 { get; set; }
        public DateTime? global_d1;
        public DateTime? global_d2;

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
                _searchstring = _searchstring.ToLower().Trim();
                _searchstring = _searchstring.Replace("--", "").Replace("drop", "").Replace("delete", "").Replace("truncate", "").Replace(";", " or ").Replace(",", " or ").Replace("  ", " ");
                _searchstring = _searchstring.Replace(" or ", "#or#").Replace(" and ", "#and#");
                _searchstring = _searchstring.Replace(" ", " and ");
                _searchstring = _searchstring.Replace("#or#", " or ").Replace("#and#", " and ");
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
            if (this.explicit_sqlwhere != null)
            {
                AQ(this.explicit_sqlwhere, "", null);
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
            _lis.Add(new QRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue, AndOrZleva = strAndOrZleva, BracketLeft = strBracketLeft, BracketRight = strBracketRight, Par2Name = strPar2Name, Par2Value = Par2Value });
        }

        private static object get_param_value(string colType, string colValue)
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


            string leva_zavorka(int i, int intEndIndex)
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
            string prava_zavorka(int i, int intEndIndex)
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



        }
    }
}
