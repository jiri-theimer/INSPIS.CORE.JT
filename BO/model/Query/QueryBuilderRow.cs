using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public abstract class QueryBuilderRow:BaseBO
    {
        public string FieldType { get; set; }
        public string FieldEntity { get; set; }
        public string FieldSqlSyntax { get; set; }
        
        

        public string SqlWrapper { get; set; }

        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:table-row;";
                }
            }
        }

        public string WrapFilter(string strWhere)
        {
            if (this.SqlWrapper == null)
            {
                return strWhere;
            }
            else
            {
                return this.SqlWrapper.Replace("#filter#", strWhere);


            }
        }
    }
}
