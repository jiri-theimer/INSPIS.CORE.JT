﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryJ04:baseQuery
    {
        public int x31id { get; set; }
        
        
        public int f06id { get; set; }
        public int b06id { get; set; }
        public int h11id { get; set; }
        public myQueryJ04()
        {
            this.Prefix = "j04";
        }

        public override List<QRow> GetRows()
        {
            if (this.x31id > 0)
            {
                AQ("a.j04ID IN (select j04ID FROM x37ReportRestriction_UserRole WHERE x31ID=@x31id)", "x31id", this.x31id);
            }
            if (this.h11id > 0)
            {
                AQ("a.j04ID IN (select j04ID FROM h12NoticeBoard_Permission WHERE h11ID=@h11id)", "h11id", this.h11id);
            }
            if (this.b06id > 0)
            {
                AQ("a.j04ID IN (select j04ID FROM b12WorkflowReceiverToHistory WHERE j04ID IS NOT NULL AND b06ID=@b06id)", "b06id", this.b06id);
            }
            if (this.f06id > 0)
            {
                AQ("a.j04ID IN (select j04ID FROM f07Form_UserRole_EncryptedPermission WHERE f06ID=@f06id)", "f06id", this.f06id);
            }

            if (this.param1 == "j04IsAllowInSchoolAdmin")
            {
                AQ("a.j04IsAllowInSchoolAdmin=1", "", null);    //filtr školních rolí podle j04IsAllowInSchoolAdmin=1
            }
            if (this.param1 == "institution")
            {
                AQ("(a.j04IsAllowInSchoolAdmin=1 OR a.j04RelationFlag=2)", "", null);    //filtr školních rolí podle j04IsAllowInSchoolAdmin=1
            }

            return this.InhaleRows();

        }
    }
}
