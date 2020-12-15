using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DL
{
    public class basQuery
    {
        public static DL.FinalSqlCommand GetFinalSql(string strPrimarySql,BO.baseQuery mq, BO.RunningUser ru, bool bolPrepareParam4DT = false)
        {           
            if (mq.CurrentUser == null)
            {
                mq.CurrentUser = ru;
            }
            var ret = new DL.FinalSqlCommand();
            var lis = mq.GetRows();
            if (lis !=null && lis.Count > 0)
            {
                ret.Parameters = new Dapper.DynamicParameters();
                if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                foreach (var c in lis.Where(p => String.IsNullOrEmpty(p.ParName) == false))
                {
                    ret.Parameters.Add(c.ParName, c.ParValue);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.ParName, ParValue = c.ParValue });
                }
                foreach (var c in lis.Where(p => String.IsNullOrEmpty(p.Par2Name) == false))
                {
                    ret.Parameters.Add(c.Par2Name, c.Par2Value);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.Par2Name, ParValue = c.Par2Value });
                }


                ret.SqlWhere = String.Join(" ", lis.Select(p => p.AndOrZleva + " " + p.BracketLeft + p.StringWhere + p.BracketRight)).Trim();    //složit závěrčnou podmínku
                //System.IO.File.WriteAllText("c:\\temp\\hovado"+mq.Prefix+".txt", ret.SqlWhere);
            }

            if (!string.IsNullOrEmpty(ret.SqlWhere))
            {
                strPrimarySql += " WHERE " + ret.SqlWhere;
            }
            if (!string.IsNullOrEmpty(mq.explicit_orderby))
            {
                strPrimarySql += " ORDER BY " + mq.explicit_orderby;
            }

            if (strPrimarySql.Contains("@gd1"))
            {   //view s napevno navrženou podmínkou časového filtru                            
                if (mq.global_d1 == null) mq.global_d1 = new DateTime(2000, 1, 1);
                if (mq.global_d2 == null)
                {
                    mq.global_d2 = new DateTime(3000, 1, 1);
                }

                
            }


            //System.IO.File.WriteAllText("c:\\temp\\hovado"+mq.Prefix+".txt", strPrimarySql);
            ret.FinalSql = strPrimarySql;
            return ret;
        }
    }
}
