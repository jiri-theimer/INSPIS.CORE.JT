using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryF31:baseQuery
    {
        public int a11id { get; set; }
        public int f19id { get; set; }
        public int f06id { get; set; }
        public int f18id { get; set; }
        

        public myQueryF31()
        {
            this.Prefix = "f31";
        }

        public override List<QRow> GetRows()
        {
            if (this.f06id > 0)
            {
                AQ("a11.f06ID=@f06id", "f06id", this.f06id);
            }
            if (this.a11id > 0)
            {
                AQ("a.a11ID=@a11id", "a11id", this.a11id);
            }
            if (this.f19id > 0)
            {

                AQ("a.f19ID=@f19id", "f19id", this.f19id);

            }
            if (this.f18id > 0)
            {
                AQ("f19.f18ID=@f18id", "f18id", this.f18id);
            }
           

            return this.InhaleRows();

        }
    }
}
