using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryF19:baseQuery
    {
        public int f21id { get; set; }
        public int f25id { get; set; }
        public int f26id { get; set; }
        public int f06id { get; set; }
        public int f18id { get; set; }
        public List<int> f06ids { get; set; }
        public myQueryF19()
        {
            this.Prefix = "f19";
        }

        public override List<QRow> GetRows()
        {
            if (this.f21id > 0)
            {

                AQ("a.f19ID IN (SELECT f19ID FROM f20ReplyUnitToQuestion WHERE f21ID=@f21id)", "f21id", this.f21id);
            }
            if (this.f25id > 0)
            {

                AQ("a.f25ID=@f25id", "f25id", this.f25id);
            }
            if (this.f26id > 0)
            {
                AQ( "a.f26ID=@f26id", "f26id", this.f26id);
            }
            if (this.f06id > 0)
            {
                AQ("a.f18ID IN (SELECT f18ID FROM f18FormSegment WHERE f06ID=@f06id)", "f06id", this.f06id);
            }
            if (this.f18id > 0)
            {
                AQ( "a.f18ID=@f18id", "f18id", this.f18id);
            }
            if (this.f06ids != null && this.f06ids.Count > 0)
            {
                AQ("a.f18ID IN (SELECT f18ID FROM f18FormSegment WHERE f06ID IN (" + string.Join(",", this.f06ids) + "))", "", null);
            }

            return this.InhaleRows();

        }
    }
}
