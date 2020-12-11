using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery000:baseQuery
    {
        public int x31id { get; set; }
        public int a10id { get; set; }
        public int b01id { get; set; }
        public int f06id { get; set; }
        public int b06id { get; set; }
        public int h11id { get; set; }
        public int f29id { get; set; }
        public int x29id { get; set; }
        public int x55id { get; set; }
        public int a11id { get; set; }
        public myQuery000(string prefix)
        {
            this.Prefix = prefix;
        }

        public override List<QRow> GetRows()
        {
            if (this.h11id > 0)
            {
                if (this.Prefix == "j04") AQ("a.j04ID IN (select j04ID FROM h12NoticeBoard_Permission WHERE h11ID=@h11id)", "h11id", this.h11id);
            }
            if (this.f29id > 0)
            {
                if (this.Prefix == "a17") AQ("a.a17ID IN (select a17ID FROM f41PortalQuestionTab_a17Binding WHERE f29ID=@f29id)", "f29id", this.f29id);
            }
            if (this.h11id > 0)
            {
                if (this.Prefix == "j04") AQ("a.j04ID IN (select j04ID FROM h12NoticeBoard_Permission WHERE h11ID=@h11id)", "h11id", this.h11id);
            }
            if (this.x31id > 0)
            {
                if (this.Prefix == "x32") AQ("a.x32ID IN (select x32ID FROM x34Report_Category WHERE x31ID=@x31id)", "x31id", this.x31id);
                if (this.Prefix == "j04") AQ( "a.j04ID IN (select j04ID FROM x37ReportRestriction_UserRole WHERE x31ID=@x31id)", "x31id", this.x31id);
                if (this.Prefix == "a10") AQ("a.a10ID IN (select a10ID FROM a23EventType_Report WHERE x31ID=@x31id)", "x31id", this.x31id);
                if (this.Prefix == "a08") AQ("a.a08ID IN (select a08ID FROM a27EventTheme_Report WHERE x31ID=@x31id)", "x31id", this.x31id);
            }
            if (this.x55id > 0)
            {
                if (this.Prefix == "j04") AQ("a.j04ID IN (select j04ID FROM x57WidgetRestriction WHERE x55ID=@x55id)", "x55id", this.x55id);
            }
            if (this.Prefix == "x29")
            {
                if (this.param1 == "x29IsAttachment") { AQ("a.x29IsAttachment=1", "", null); };    //filtr entit x29IsAttachment=1
                if (this.param1 == "x29IsReport") { AQ("a.x29IsReport=1", "", null); };    //filtr entit x29IsReport=1
            }
            if (this.x29id > 0)
            {
                if (this.Prefix == "o13" || this.Prefix == "x31") AQ("a.x29ID=@x29id", "x29id", this.x29id);

            }
            if (this.Prefix == "b02" && this.param1 != null)
            {
                AQ("a.b02Entity=@prefix", "prefix", this.param1);    //filtr seznamu stavů podle druhu entity
            }
            if (this.Prefix == "h05" && this.param1 == "createtodo")
            {
                AQ("a.h05ID IN (1,2)", "", null);    //filtr stavů pro nově zakládaný úkol
            }
            if (this.Prefix == "j04" && this.param1 == "j04IsAllowInSchoolAdmin")
            {
                AQ("a.j04IsAllowInSchoolAdmin=1", "", null);    //filtr školních rolí podle j04IsAllowInSchoolAdmin=1
            }
            if (this.Prefix == "j04" && this.param1 == "institution")
            {
                AQ("(a.j04IsAllowInSchoolAdmin=1 OR a.j04RelationFlag=2)", "", null);    //filtr školních rolí podle j04IsAllowInSchoolAdmin=1
            }
            if (this.Prefix == "x24" && this.param1 == "textbox")
            {
                AQ("a.x24ID IN (1,2,3,4,5)", "", null);    //filtr v datovém typu otázky pro TEXTBOX
            }
            if (this.Prefix == "x31" && this.param1 == "x31Is4SingleRecord=1")
            {
                AQ("a.x31Is4SingleRecord=1", "", null);    //pouze kontextové sestavy
            }

            if (this.a11id > 0)
            {
                if (this.Prefix == "f32") AQ("a.a11ID=@a11id", "a11id", this.a11id);
            }
            if (this.f06id > 0)
            {
                if (this.Prefix == "j04") AQ("a.j04ID IN (select j04ID FROM f07Form_UserRole_EncryptedPermission WHERE f06ID=@f06id)", "f06id", this.f06id);
                if (this.Prefix == "x31") AQ("a.x31ID IN (select x31ID FROM f08Form_Report WHERE f06ID=@f06id)", "f06id", this.f06id);
                if (this.Prefix == "f18" || this.Prefix == "a11") AQ("a.f06ID=@f06id", "f06id", this.f06id);
                if (this.Prefix == "f19") AQ("a.f18ID IN (SELECT f18ID FROM f18FormSegment WHERE f06ID=@f06id)", "f06id", this.f06id);
                if (this.Prefix == "f32") AQ("a11.f06ID=@f06id", "f06id", this.f06id);
                if (this.Prefix == "xx1") AQ("f18.f06ID=@f06id", "f06id", this.f06id); //f21ReplyUnitJoinedF19: GetListJoinedF19
                if (this.Prefix == "f31") AQ("a11.f06ID=@f06id", "f06id", this.f06id);                
            }


            return this.InhaleRows();

        }
    }
}
