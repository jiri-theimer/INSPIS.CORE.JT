using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA11 : baseQuery
    {        
        public int a01id { get; set; }
        public int a01parentid { get; set; }
        public int a03id { get; set; }
        public int f06id { get; set; }
        public bool? a11ispoll {get;set;}
        public bool? a11issimulation { get; set; }
        public myQueryA11()
        {
            this.Prefix = "a11";
        }
        public override List<QRow> GetRows()
        {
            if (this.a01id > 0)
            {
                
                this.AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
            if (this.a01parentid > 0)
            {

                this.AQ("a.a01ID IN (select a01ID FROM a01Event WHERE a01ParentID=@a01parentid)", "a01parentid", this.a01parentid);
            }
            if (this.f06id > 0)
            {

                this.AQ("a.f06ID=@f06id", "f06id", this.f06id);
            }
            if (this.a03id > 0)
            {
                AQ("a11_a01.a03ID=@a03id", "a03id", this.a03id);
            }
            if (this.a11ispoll !=null) AQ("a.a11IsPoll=@a11ispoll", "a11ispoll", this.a11ispoll);
            if (this.a11issimulation !=null) AQ("a.a11IsSimulation=@a11issimulation", "a11issimulation",this.a11issimulation);

            return this.InhaleRows();
            
        }
    }
}
