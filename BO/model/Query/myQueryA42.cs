using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA42:baseQuery
    {
        public int a03id { get; set; }
        
        public myQueryA42()
        {
            this.Prefix = "a42";
        }

        public override List<QRow> GetRows()
        {
            if (this.a03id > 0)
            {
                AQ("a.a42ID IN (select a42ID FROM a01Event WHERE a03ID=@a03id)", "a03id", this.a03id);
            }
           

            if (_searchstring != null && _searchstring.Length > 2)
            {
                AQ("(a.a42Name LIKE '%'+@expr+'%' OR a.a42Description LIKE '%'+@expr+'%')", "expr", this.SearchString);
            }

            return this.InhaleRows();

        }
    }
}
