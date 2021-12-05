using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace BO.CLS
{
    public class MergeContent
    {
        public List<BO.MergeExternalSql> GetAllMergeExternalSqlsInContent(string strContent,List<BO.StringPair> vars,IEnumerable<BO.x39ConnectString> lisX39)
        {
            //vrátí seznam sql výrazů, které se mají volat vůči externímu datovému zdroji uvnitř závorek {}
            //ve vars jsou proměnné uvedené v SQL, @a03redizo, @j03login, @a01id,@a03id + jejich hodnoty
            //v lisX39 je seznam externích sql zdrojů

            var lis = new List<BO.MergeExternalSql>();
            if (string.IsNullOrEmpty(strContent)) return lis;

            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(strContent, @"\{.*?\}");
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                var c = new BO.MergeExternalSql() { OrigExpr = m.Value };  //do Key se uloží hledaný výraz vč. {}
                string strExpr = m.Value.Replace("{", "").Replace("}", "");

                var arr = BO.BAS.ConvertString2List(strExpr, "==");
                if (arr.Count > 0)
                {
                    c.x39Code = arr[0];
                    if (lisX39.Where(p => p.x39Code == c.x39Code).Count() > 0)
                    {                        
                        c.ConnectString = lisX39.Where(p => p.x39Code == c.x39Code).First().x39Value;   //connect string záznam
                    }
                    
                    c.OrigSql = arr[1];
                    c.MergedSql = arr[1];
                }
                

                if (vars != null && vars.Count >0)
                {
                    foreach (BO.StringPair v in vars)
                    {
                        if (c.MergedSql.Contains("@" + v.Key,StringComparison.OrdinalIgnoreCase))
                        {
                            c.MergedSql = c.MergedSql.Replace("@" + v.Key, v.Value,StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }

                c.MergedSql = strExpr;  //do Value se uloží výsledek
               

                lis.Add(c);
            }

            return lis;

            
            

        }
        public List<string> GetAllMergeFieldsInContent(string strContent)
        {
            //vrátí seznam slučovacích polí uvozrených [% %], které se vyskytují v strContent
            var lis = new List<string>();
            if (string.IsNullOrEmpty(strContent)) return lis;

            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(strContent, @"\[%.*?\%]");
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                string strField = m.Value.Replace("[%", "").Replace("%]","");
                lis.Add(strField.ToLower());
            }

            return lis;

        }
        public string GetMergedContent(string strContent,DataTable dt)
        {
            var fields = GetAllMergeFieldsInContent(strContent);

            foreach (DataRow dr in dt.Rows)
            {
                string strVal = "";
                foreach (DataColumn col in dt.Columns)
                {
                    if (fields.Contains(col.ColumnName.ToLower()))
                    {
                        if (dr[col] == null)
                        {
                            strVal = "";
                        }
                        else
                        {
                            switch (col.DataType.Name.ToString())
                            {
                                case "DateTime":
                                    strVal = BO.BAS.ObjectDate2String(dr[col]);
                                    break;
                                case "Decimal":
                                case "Double":
                                    strVal = BO.BAS.Number2String(Convert.ToDouble(dr[col]));
                                    break;
                                default:
                                    strVal = dr[col].ToString();
                                    break;
                            }

                        }
                        strContent = strContent.Replace("[%" + col.ColumnName + "%]", strVal, StringComparison.OrdinalIgnoreCase);

                    }
                    
                }

            }
            return strContent;
        }
    }
}
