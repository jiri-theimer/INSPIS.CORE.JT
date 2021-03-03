using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryZ01 : baseQuery
    {
        public int x32id { get; set; }
        public myQueryZ01()
        {
            this.Prefix = "z01";
        }

        public override List<QRow> GetRows()
        {
            if (this.x32id > 0)
            {
                AQ("a.x31ID IN (select x31ID FROM x34Report_Category WHERE x32ID=@x32id)", "x32id", this.x32id);
            }

            if (this.CurrentUser != null && !this.CurrentUser.TestPermission(j05PermValuEnum.AdminGlobal_Ciselniky))
            {
                AQ("(a.x31ID NOT IN (SELECT x31ID FROM x37ReportRestriction_UserRole) OR a.x31ID IN (SELECT x31ID FROM x37ReportRestriction_UserRole WHERE j04ID=@j04id))", "j04id", this.CurrentUser.j04ID);
            }

            return this.InhaleRows();

        }

    }
}
