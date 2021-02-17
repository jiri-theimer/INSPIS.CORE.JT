using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA41:baseQuery
    {
        public int a01id { get; set; }
        public int j02id { get; set; }
        public myQueryA41()
        {
            this.Prefix = "a41";
        }
      
        public override List<QRow> GetRows()
        {
            if (this.a01id > 0)
            {

                this.AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
            if (this.j02id > 0)
            {               
                if (this.CurrentUser.AppImplementation == "HD") //V HD implementaci si hrajeme i na týmy řešitelů
                {
                    if (this.CurrentUser.j02ID == j02id && this.CurrentUser.j11IDs_Cache != null)
                    {
                        this.AQ("(a.j02ID=@j02id OR a.j11ID IN ("+this.CurrentUser.j11IDs_Cache+"))", "j02id", this.j02id);                        
                    }
                    else
                    {
                        this.AQ("(a.j02ID=@j02id OR a.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id))", "j02id", this.j02id);                        
                    }
                }
                else
                {
                    this.AQ("a.j02ID=@j02id", "j02id", this.j02id);
                }


            }

            

            return this.InhaleRows();

        }
    }
}
