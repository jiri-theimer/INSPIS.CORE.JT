using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO
{
    public class peQueryRow
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
    
    public class peQuery
    {
        private string _pkfield;
        private string _prefix;
        private List<peQueryRow> _lis;

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
        public string explicit_orderby { get; set; }
        public string explicit_selectsql { get; set; }
        public string explicit_sqlwhere { get; set; }
        public BO.RunningUser CurrentUser;
        public bool MyRecordsDisponible;


        public virtual string GetSqlWhere()
        {
            if (this.pids != null && this.pids.Any())
            {
                AQ(_pkfield + " IN (" + String.Join(",", this.pids) + ")", "", null);
            }

            return "";
        }

        protected void AQ(string strWhere, string strParName, object ParValue, string strAndOrZleva = "AND", string strBracketLeft = null, string strBracketRight = null, string strPar2Name = null, object Par2Value = null)
        {
            if (_lis == null)
            {
                _lis = new List<peQueryRow>();
            }
            if (_lis.Count == 0)
            {
                strAndOrZleva = ""; //první podmínka zleva
            }
            
            if (String.IsNullOrEmpty(strParName) == false && _lis.Where(p => p.ParName == strParName).Count() > 0)
            {
                return; //parametr strParName již byl dříve přidán
            }
            _lis.Add(new peQueryRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue, AndOrZleva = strAndOrZleva, BracketLeft = strBracketLeft, BracketRight = strBracketRight, Par2Name = strPar2Name, Par2Value = Par2Value });
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

        

    }
}
