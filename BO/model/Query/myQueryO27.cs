using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryO27:baseQuery
    {
        public int a01id { get; set; }
        public int a03id { get; set; }
        public int a42id { get; set; }
        public int f06id { get; set; }
        public int f32id { get; set; }
        public int f18id { get; set; }
        public int f19id { get; set; }
        public int j02id { get; set; }
        public int o13id { get; set; }
        public int x29id { get; set; }
        public int recpid { get; set; }
        public myQueryO27()
        {
            this.Prefix = "o27";
        }

        public override List<QRow> GetRows()
        {
            if (this.o13id > 0)
            {
                AQ("a.o13ID=@o13id", "o13id", this.o13id);
            }

            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);

            }
            if (this.recpid > 0)
            {
               AQ("a.o27DataPID=@recpid", "recpid", this.recpid);
            }
            if (this.a03id > 0)
            {
                AQ("a.x29ID=101 AND a.o27DataPID IN (select a01ID FROM a01Event WHERE a03ID=@a03id)", "a03id", this.a03id);
            }
            
            if (this.a01id > 0)
            {                
                AQ("a.x29ID=101 AND a.o27DataPID=@a01id", "a01id", this.a01id);
            }
            
            if (this.a42id > 0)
            {
                AQ( "a.x29ID=142 AND a.o27DataPID=@a42id", "a42id", this.a42id);
            }
            if (this.f06id > 0)
            {
                AQ("a.x29ID=406 AND a.o27DataPID=@f06id", "f06id", this.f06id);
            }
            if (this.f32id > 0)
            {
                AQ( "a.x29ID=432 AND a.o27DataPID=@f32id", "f32id", this.f32id);
            }
            if (this.f18id > 0)
            {
                AQ("a.x29ID=418 AND a.o27DataPID=@f18id", "f18id", this.f18id);
            }
            if (this.f19id > 0)
            {
                AQ("a.x29ID=419 AND a.o27DataPID=@f19id", "f19id", this.f19id);
            }
            if (this.j02id > 0)
            {
                AQ("a.x29ID=502 AND a.o27DataPID=@j02id", "j02id", this.j02id);
            }



            return this.InhaleRows();

        }
    }
}
