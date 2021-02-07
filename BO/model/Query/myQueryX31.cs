using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryX31 : baseQuery
    {
        public int x32id { get; set; }
        public int f06id { get; set; }
        public int x29id { get; set; }
        public myQueryX31()
        {
            this.Prefix = "x31";
        }

        public override List<QRow> GetRows()
        {
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }

            if (this.f06id > 0)
            {
                AQ("a.x31ID IN (select x31ID FROM f08Form_Report WHERE f06ID=@f06id)", "f06id", this.f06id);


            }


            if (this.x32id > 0)
            {
                AQ("a.x31ID IN (select x31ID FROM x34Report_Category WHERE x32ID=@x32id)", "x32id", this.x32id);
            }

            if (this.param1 == "x31Is4SingleRecord=1")
            {
                AQ("a.x31Is4SingleRecord=1", "", null);    //pouze kontextové sestavy
            }

            return this.InhaleRows();

        }
    }
}
