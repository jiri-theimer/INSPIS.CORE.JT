using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA10:baseQuery
    {
        public int x31id { get; set; }
        public bool? MyDisponible4Create { get; set; }
        public myQueryA10()
        {
            this.Prefix = "a10";
        }

        public override List<QRow> GetRows()
        {
            if (this.MyDisponible4Create==true)
            {
                if (this.CurrentUser.j04IsAllowedAllEventTypes)
                {
                    AQ("GETDATE() BETWEEN a.a10ValidFrom AND a.a10ValidUntil", null, null);
                }
                else
                {
                    AQ("GETDATE() BETWEEN a.a10ValidFrom AND a.a10ValidUntil AND a.a10ID IN (select a10ID FROM j08UserRole_EventType WHERE j04id=@j04id AND j08IsAllowedCreate=1)", "j04id", this.CurrentUser.j04ID);
                }                

            }

            if (this.x31id > 0)
            {
                AQ("a.a10ID IN (select a10ID FROM a23EventType_Report WHERE x31ID=@x31id)", "x31id", this.x31id);
            }


            return this.InhaleRows();

        }

    }
}
