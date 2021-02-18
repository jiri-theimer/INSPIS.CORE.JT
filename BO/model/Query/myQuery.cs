using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery:baseQuery
    {
        public int x31id { get; set; }
        public int a10id { get; set; }
        public int b01id { get; set; }
        public int f06id { get; set; }
        public List<int> f06ids { get; set; }
        public int b06id { get; set; }
        public int f29id { get; set; }
        public int x29id { get; set; }
       
        public int h04id { get; set; }
        public int a03id { get; set; }
        public int a05id { get; set; }
        public int j02id { get; set; }
        public int o53id { get; set; }
        public int b02id { get; set; }
        public int f18id { get; set; }
        public int f21id { get; set; }
        public int a01id { get; set; }
        public int j04id { get; set; }
        public bool? a45ismanual { get; set; }
        public string x24controlname { get; set; }
        public bool statuses4createtodo { get; set; }   //filtr stavů pro nově zakládaný úkol
        public bool disponiblesmtpaccounts { get; set; }

        public myQuery(string prefix)
        {
            this.Prefix = prefix.Substring(0,3);
        }

       

        public override List<QRow> GetRows()
        {
            if (this.j04id > 0)
            {
                if (this.Prefix=="a39") AQ("(a.j04ID_Explicit=@j04id OR j03.j04ID=@j04id)", "j04id", this.j04id);
            }
            if (this.a01id > 0)
            {
                if (this.Prefix == "b05") AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
            if (this.f21id > 0)
            {                
                if (this.Prefix == "f22") AQ("a.f22ID IN (SELECT f22ID FROM f43ReplyUnitToSet WHERE f21ID=@f21id)", "f21id", this.f21id);
            }
            if (this.f18id > 0)
            {
                if (this.Prefix == "f26" || this.Prefix == "f25") AQ("a.f18ID=@f18id", "f18id", this.f18id);
            }
            if (this.b02id > 0)
            {
                if (this.Prefix=="b06") AQ("a.b02ID=@b02id", "b02id", this.b02id);
            }
            if (this.o53id > 0)
            {
                if (this.Prefix == "o51") AQ( "a.o53ID=@o53id", "o53id", this.o53id);
            }
            if (this.a05id > 0)
            {
                if (this.Prefix=="j23") AQ("a.a05ID=@a05id", "a05id", this.a05id);
            }

            if (this.b06id > 0)
            {                
                if (this.Prefix == "o13") AQ("a.o13ID IN (select o13ID FROM b14WorkflowRequiredAttachmentTypeToStep WHERE b06ID=@b06id)", "b06id", this.b06id);
                //if (this.Prefix == "a45" && this.param1 == "b12") AQ("a.a45ID IN (select a45ID FROM b12WorkflowReceiverToHistory WHERE a45ID IS NOT NULL AND b06ID=@b06id)", "b06id", this.b06id);
            }
            if (this.b01id > 0)
            {
                if (this.Prefix == "b06") AQ( "a.b02ID IN (select b02ID FROM b02WorkflowStatus WHERE b01ID=@b01id)", "b01id", this.b01id);
                if (this.Prefix == "b02") AQ( "a.b01ID=@b01id", "b01id", this.b01id);
            }
            if (this.j02id > 0)
            {
                if (this.Prefix == "j90" || this.Prefix == "j92") AQ("a.j03ID IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", this.j02id);
                if (this.Prefix == "a39" || this.Prefix == "j03") AQ("a.j02ID=@j02id", "j02id", this.j02id);
            }
            
            if (this.a03id > 0)
            {
                if (this.Prefix == "a37" || this.Prefix == "a39") AQ("a.a03ID=@a03id", "a03id", this.a03id);
                if (this.Prefix == "a19") AQ("a.a37ID IN (select a37ID FROM a37InstitutionDepartment WHERE a03ID=@a03id)", "a03id", this.a03id);
                if (this.Prefix == "k01") AQ("a03_k02.a03ID=@a03id", "a03id", this.a03id);
                if (this.Prefix == "a29") AQ("a.a29ID IN (select a29ID FROM a43InstitutionToList WHERE a03ID=@a03id)", "a03id", this.a03id);                                
            }
            
            if (this.h04id > 0)
            {
                if (this.Prefix == "j11") AQ("a.j11ID IN (select j11ID FROM h06ToDoReceiver WHERE h04ID=@h04id AND j11ID IS NOT NULL)", "h04id", this.h04id);
            }
            if (this.f29id > 0)
            {
                if (this.Prefix == "a17") AQ("a.a17ID IN (select a17ID FROM f41PortalQuestionTab_a17Binding WHERE f29ID=@f29id)", "f29id", this.f29id);
            }
            
            if (this.x31id > 0)
            {
                if (this.Prefix == "x32") AQ("a.x32ID IN (select x32ID FROM x34Report_Category WHERE x31ID=@x31id)", "x31id", this.x31id);
                if (this.Prefix == "j04") AQ( "a.j04ID IN (select j04ID FROM x37ReportRestriction_UserRole WHERE x31ID=@x31id)", "x31id", this.x31id);
                if (this.Prefix == "a10") AQ("a.a10ID IN (select a10ID FROM a23EventType_Report WHERE x31ID=@x31id)", "x31id", this.x31id);
                if (this.Prefix == "a08") AQ("a.a08ID IN (select a08ID FROM a27EventTheme_Report WHERE x31ID=@x31id)", "x31id", this.x31id);
            }
            if (this.a10id > 0)
            {
                if (this.Prefix == "a08") AQ("a.a08ID IN (select a08ID FROM a26EventTypeThemeScope WHERE a10ID=@a10id)", "a10id", this.a10id);
            }
            
            //if (this.Prefix == "x29")
            //{
            //    if (this.param1 == "x29IsAttachment") { AQ("a.x29IsAttachment=1", "", null); };    //filtr entit x29IsAttachment=1
            //    if (this.param1 == "x29IsReport") { AQ("a.x29IsReport=1", "", null); };    //filtr entit x29IsReport=1
            //}
            if (this.x29id > 0)
            {
                if (this.Prefix == "o13") AQ("a.x29ID=@x29id", "x29id", this.x29id);

            }
            //if (this.Prefix == "b02" && this.param1 != null)
            //{
            //    AQ("a.b02Entity=@prefix", "prefix", this.param1);    //filtr seznamu stavů podle druhu entity
            //}
            if (this.Prefix == "h05" && this.statuses4createtodo)
            {
                AQ("a.h05ID IN (1,2)", "", null);    //filtr stavů pro nově zakládaný úkol
            }
            
           
            if (this.Prefix == "x24" && this.x24controlname == "textbox")
            {
                AQ("a.x24ID IN (1,2,3,4,5)", "", null);    //filtr v datovém typu otázky pro TEXTBOX
            }
            

            
            if (this.f06id > 0)
            {               
                
                if (this.Prefix == "f18" || this.Prefix == "a11") AQ("a.f06ID=@f06id", "f06id", this.f06id);                
                
                          
            }
            if (this.f06ids != null && this.f06ids.Count > 0)
            {                
                if (this.Prefix == "f18") AQ("a.f06ID IN (" + string.Join(",", this.f06ids) + ")", "", null);
            }


            if (this.Prefix == "a45" && this.a45ismanual !=null)
            {
                AQ("a.a45IsManual=@a45ismanual", "a45ismanual", this.a45ismanual);
            }
            if (this.Prefix == "b65" && this.x29id>0)
            {
                AQ( "a.x29ID=@x29id", "x29id", this.x29id);
            }
            
            //if (this.Prefix == "b02" && this.param1 != null)
            //{
            //    AQ( "a.b02Entity=@prefix", "prefix", this.param1);    //filtr seznamu stavů podle druhu entity
            //}
         
          
            //if (this.Prefix == "x29")
            //{
            //    if (this.param1 == "x29IsAttachment") { AQ("a.x29IsAttachment=1", "", null); };    //filtr entit x29IsAttachment=1
            //    if (this.param1 == "x29IsReport") { AQ("a.x29IsReport=1", "", null); };    //filtr entit x29IsReport=1
            //}

            if (this.MyRecordsDisponible && this.Prefix == "h11")
            {
                AQ("(a.h11IsPublic=1 OR a.h11ID IN (SELECT h11ID FROM h12NoticeBoard_Permission WHERE j04ID=@j04id_me)) AND GETDATE() BETWEEN a.h11ValidFrom AND a.h11ValidUntil", "j04id_me", this.CurrentUser.j04ID);

            }
            if (this.MyRecordsDisponible && this.Prefix == "x55")
            {
                AQ("(a.x55ID NOT IN (select x55ID FROM x57WidgetRestriction) OR a.x55ID IN (SELECT x55ID FROM x57WidgetRestriction WHERE j04ID=@j04id_me)) AND GETDATE() BETWEEN a.x55ValidFrom AND a.x55ValidUntil", "j04id_me", this.CurrentUser.j04ID);

            }
            if (this.MyRecordsDisponible && this.Prefix == "j40")
            {
                AQ("(a.j40UsageFlag=2 OR (a.j02ID=@j02id AND a.j40UsageFlag=1)) AND GETDATE() between a.j40ValidFrom AND a.j40ValidUntil", "j02id", this.CurrentUser.j02ID);

            }

            return this.InhaleRows();

        }
    }
}
