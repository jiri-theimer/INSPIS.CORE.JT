using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery0:baseQuery
    {
        public int x31id { get; set; }
        public int a10id { get; set; }
        public int b01id { get; set; }
        public int f06id { get; set; }
        public int b06id { get; set; }
        public int h11id { get; set; }
        public int f29id { get; set; }
        public int x29id { get; set; }
        
        public int a11id { get; set; }
        public int h04id { get; set; }
        public int a03id { get; set; }
        public int a05id { get; set; }
        public int j02id { get; set; }
        public myQuery0(string prefix)
        {
            this.Prefix = prefix.Substring(0,3);
        }

       

        public override List<QRow> GetRows()
        {     
            if (this.a05id > 0)
            {
                if (this.Prefix=="j23") AQ("a.a05ID=@a05id", "a05id", this.a05id);
            }

            if (this.b06id > 0)
            {                
                if (this.Prefix == "o13") AQ("a.o13ID IN (select o13ID FROM b14WorkflowRequiredAttachmentTypeToStep WHERE b06ID=@b06id)", "b06id", this.b06id);
                if (this.Prefix == "a45" && this.param1 == "b12") AQ("a.a45ID IN (select a45ID FROM b12WorkflowReceiverToHistory WHERE a45ID IS NOT NULL AND b06ID=@b06id)", "b06id", this.b06id);
            }
            if (this.b01id > 0)
            {
                if (this.Prefix == "b06") AQ( "a.b02ID IN (select b02ID FROM b02WorkflowStatus WHERE b01ID=@b01id)", "b01id", this.b01id);
                if (this.Prefix == "b02") AQ( "a.b01ID=@b01id", "b01id", this.b01id);
            }
            if (this.j02id > 0)
            {
                if (this.Prefix == "j90" || this.Prefix == "j92") AQ("a.j03ID IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", this.j02id);
            }
            
            if (this.a03id > 0)
            {
                if (this.Prefix == "a37" || this.Prefix == "a39") AQ("a.a03ID=@a03id", "a03id", this.a03id);
                if (this.Prefix == "a19") AQ("a.a37ID IN (select a37ID FROM a37InstitutionDepartment WHERE a03ID=@a03id)", "a03id", this.a03id);
                if (this.Prefix == "k01") AQ("a03_k02.a03ID=@a03id", "a03id", this.a03id);
                if (this.Prefix == "a29") AQ("a.a29ID IN (select a29ID FROM a43InstitutionToList WHERE a03ID=@a03id)", "a03id", this.a03id);                                
            }
            if (this.h11id > 0)
            {
                if (this.Prefix == "j04") AQ("a.j04ID IN (select j04ID FROM h12NoticeBoard_Permission WHERE h11ID=@h11id)", "h11id", this.h11id);
            }
            if (this.h04id > 0)
            {
                if (this.Prefix == "j11") AQ("a.j11ID IN (select j11ID FROM h06ToDoReceiver WHERE h04ID=@h04id AND j11ID IS NOT NULL)", "h04id", this.h04id);
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


            if (this.Prefix == "a45" && this.param1 == "a45IsManual1")
            {
                AQ( "a.a45IsManual=1", "", null);
            }
            if (this.Prefix == "b65" && this.param1 != null)
            {
                AQ( "a.x29ID=@x29id", "x29id", BO.BAS.InInt(this.param1));
            }
            if (this.Prefix == "b02" && this.param1 != null)
            {
                AQ( "a.b02Entity=@prefix", "prefix", this.param1);    //filtr seznamu stavů podle druhu entity
            }
            if (this.Prefix == "h05" && this.param1 == "createtodo")
            {
                AQ("a.h05ID IN (1,2)", "", null);    //filtr stavů pro nově zakládaný úkol
            }
           
           
            if (this.Prefix == "x24" && this.param1 == "textbox")
            {
                AQ("a.x24ID IN (1,2,3,4,5)", "", null);    //filtr v datovém typu otázky pro TEXTBOX
            }
            if (this.Prefix == "x31" && this.param1 == "x31Is4SingleRecord=1")
            {
                AQ("a.x31Is4SingleRecord=1", "", null);    //pouze kontextové sestavy
            }
            if (this.Prefix == "x29")
            {
                if (this.param1 == "x29IsAttachment") { AQ("a.x29IsAttachment=1", "", null); };    //filtr entit x29IsAttachment=1
                if (this.param1 == "x29IsReport") { AQ("a.x29IsReport=1", "", null); };    //filtr entit x29IsReport=1
            }

            return this.InhaleRows();

        }
    }
}
