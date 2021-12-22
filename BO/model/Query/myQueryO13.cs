using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryO13: baseQuery
    {
        public int x29id { get; set; }
        public int a08id { get; set; }
        public int b06id { get; set; }
        public myQueryO13()
        {
            this.Prefix = "o13";
        }

        public override List<QRow> GetRows()
        {
            if (this.a08id > 0)
            {
                AQ("a.o13ID IN (SELECT o13ID FROM a14AttachmentToTheme WHERE a08ID=@a08id)", "a08id", this.a08id);
            }
            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }
           
            if (this.b06id > 0)
            {
                AQ("a.o13ID IN (select o13ID FROM b14WorkflowRequiredAttachmentTypeToStep WHERE b06ID=@b06id)", "b06id", this.b06id);
            }

            return this.InhaleRows();

        }
    }
}
