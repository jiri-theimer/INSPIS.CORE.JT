using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryX31 : baseQuery
    {
        public int x32id { get; set; }
        public int f06id { get; set; }
        public int a01id { get; set; }
        public int a03id { get; set; }
        public int j02id { get; set; }
        public int h04id { get; set; }
        public int x29id { get; set; }
        public bool? x31is4singlerecord { get; set; }
        public myQueryX31()
        {
            this.Prefix = "x31";
        }

        public override List<QRow> GetRows()
        {
            if (this.a01id > 0) this.x29id = 101;
            if (this.a03id > 0) this.x29id = 103;
            if (this.j02id > 0) this.x29id = 502;
            if (this.h04id > 0) this.x29id = 604;
            


            if (this.x29id > 0)
            {
                AQ("a.x29ID=@x29id", "x29id", this.x29id);
            }

            if (this.f06id > 0)
            {
                AQ("a.x31ID IN (select x31ID FROM f08Form_Report WHERE f06ID=@f06id)", "f06id", this.f06id);


            }
            if (this.CurrentUser != null && this.CurrentUser.TestPermission(j05PermValuEnum.AdminGlobal) == false)
            {
                AQ("a.x31ID IN (select x31ID FROM x37ReportRestriction_UserRole WHERE j04ID=@j04id)", "j04id", this.CurrentUser.j04ID);
            }
            
            if (this.a01id > 0)
            {
                AQ("a.x31ID IN (select a.x31ID FROM a23EventType_Report a INNER JOIN a10EventType b ON a.a10ID=b.a10ID INNER JOIN a01Event c ON b.a10ID=c.a10ID WHERE c.a01ID=@a01id) OR a.x31ID IN (select a.x31ID FROM a27EventTheme_Report a INNER JOIN a08Theme b ON a.a08ID=b.a08ID INNER JOIN a01Event c ON b.a08ID=c.a08ID WHERE c.a01ID=@a01id)", "a01id", this.a01id);
            }
            
            
           

            if (this.x32id > 0)
            {
                AQ("a.x31ID IN (select x31ID FROM x34Report_Category WHERE x32ID=@x32id)", "x32id", this.x32id);
            }

            if (this.x31is4singlerecord !=null)
            {
                AQ("a.x31Is4SingleRecord=@x31is4singlerecord", "x31is4singlerecord", this.x31is4singlerecord);    //pouze kontextové sestavy
            }

            return this.InhaleRows();

        }
    }
}
