using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryX40: baseQuery
    {
        public int j02id { get; set; }
        public int a42id { get; set; }
        public myQueryX40()
        {
            this.Prefix = "x40";
        }

        public override List<QRow> GetRows()
        {
            
            if (this.j02id > 0)
            {
                AQ( "a.j03ID_Creator IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", this.j02id);
            }
            if (this.a42id > 0)
            {
                AQ("a.x29ID=101 AND a.x40DataPid IN (select a01ID FROM a01Event WHERE a42ID=@a42id)", "a42id", this.a42id);
            }
            return this.InhaleRows();

        }
    }
}
