using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryXX1: baseQuery
    {
        public int f06id { get; set; }
        public int f18id { get; set; }
        public int f19id { get; set; }
        public List<int> f19ids { get; set; }
        public myQueryXX1()
        {
            this.Prefix = "xx1";
        }

        public override List<QRow> GetRows()
        {
            if (this.f06id > 0)
            {
                AQ("f18.f06ID=@f06id", "f06id", this.f06id); //f21ReplyUnitJoinedF19: GetListJoinedF19
            }
            if (this.f18id > 0)
            {
                AQ("f19.f18ID=@f18id", "f18id", this.f18id); //f21ReplyUnitJoinedF19: GetListJoinedF19
            }
            if (this.f19id > 0)
            {
                
                AQ("f20.f19ID=@f19id", "f19id", this.f19id); //f21ReplyUnitJoinedF19: GetListJoinedF19
                
            }
            if (this.f19ids != null && this.f19ids.Count > 0)
            {
                if (this.Prefix == "xx1") AQ("f20.f19ID IN (" + string.Join(",", this.f19ids) + ")", "", null); //f21ReplyUnitJoinedF19: GetListJoinedF19

            }

            return this.InhaleRows();

        }
    }
}
