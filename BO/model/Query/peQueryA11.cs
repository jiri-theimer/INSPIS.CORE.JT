using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class peQueryA11:peQuery
    {
        public int a01id { get; set; }
        public int f06id { get; set; }
        public override string GetSqlWhere()
        {
            if (this.a01id > 0)
            {
                
                this.AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
            if (this.f06id > 0)
            {

                this.AQ("a.f06ID=@f06id", "f06id", this.f06id);
            }

            return "";
        }
    }
}
