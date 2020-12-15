using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA35:baseQuery
    {
        public int j02id { get; set; }
        public int a01id { get; set; }
        public int a05id { get; set; }
        public myQueryA35()
        {
            this.Prefix = "a35";
        }

        public override List<QRow> GetRows()
        {
            if (this.global_d1 != null)
            {
                AQ("a.a35PlanDate BETWEEN @gd1 AND @gd2", "gd1", this.global_d1, "AND", null, null, "gd2", this.global_d2);
            }
            if (this.a01id > 0)
            {
                AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
           
            if (this.j02id > 0)
            {
                AQ("a.j02ID=@j02id", "j02id", this.j02id);
            }
            if (this.a05id > 0)
            {
                AQ("a.j02ID IN (SELECT xa.j02ID FROM a02Inspector xa INNER JOIN a04Inspectorate xb ON xa.a04ID=xb.a04ID WHERE xb.a05ID=@a05id)", "a05id", this.a05id);
            }
            

            return this.InhaleRows();

        }

    }
}
