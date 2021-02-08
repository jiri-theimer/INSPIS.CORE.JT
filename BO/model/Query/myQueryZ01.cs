using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryZ01 : baseQuery
    {
        public int x32id { get; set; }
        public myQueryZ01()
        {
            this.Prefix = "z01";
        }

        public override List<QRow> GetRows()
        {
            if (this.x32id > 0)
            {
                AQ("a.x31ID IN (select x31ID FROM x34Report_Category WHERE x32ID=@x32id)", "x32id", this.x32id);
            }


            return this.InhaleRows();

        }

    }
}
