using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BO.CLS
{
    public class MergeContent
    {
        public List<string> GetAllMergeFieldsInContent(string strContent)
        {
            //vrátí seznam slučovacích polí, které se vyskytují v strContent
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
