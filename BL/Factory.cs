using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Primitives;

namespace BL
{
    public class Factory
    {
        public BO.RunningUser CurrentUser { get; set; }
        public BL.RunningApp App { get; set; }
        public BL.TheGlobalParams GlobalParams {get;set;}
        public BL.TheEntitiesProvider EProvider { get; set; }
        public BL.TheTranslator Translator { get; set; }
        

        private Ij02PersonBL _j02;
        private Ij03UserBL _j03;
        private Ij23NonPersonBL _j23;
        private Ij24NonPersonTypeBL _j24;
        private Ij25NonPersonPlanReasonBL _j25;
        private Ij26HolidayBL _j26;
        private Ij72TheGridTemplateBL _j72;
        private Ia01EventBL _a01;
        private Ia02InspectorBL _a02;
        private Ia03InstitutionBL _a03;
        private Ia04InspectorateBL _a04;
        private Ia05RegionBL _a05;
        private Ia08ThemeBL _a08;
        private Ia09FounderTypeBL _a09;
        private Ia10EventTypeBL _a10;
        private Ia11EventFormBL _a11;
        private Ia17DepartmentTypeBL _a17;
        private Ia18DepartmentDomainBL _a18;
        private Ia19DomainToInstitutionDepartmentBL _a19;
        private Ia25EventFormGroupBL _a25;
        private Ia29InstitutionListBL _a29;
        private Ia39InstitutionPersonBL _a39;
        private Ia37InstitutionDepartmentBL _a37;
        private Ia35PersonEventPlanBL _a35;
        private Ia38NonPersonEventPlanBL _a38;
        private Ia41PersonToEventBL _a41;
        private Ia42QesBL _a42;
        private Ia70SISBL _a70;

        private Ib01WorkflowTemplateBL _b01;
        private Ib02WorkflowStatusBL _b02;
        private Ib06WorkflowStepBL _b06;
        private Ib05Workflow_HistoryBL _b05;
        private Ib65WorkflowMessageBL _b65;

        private If06FormBL _f06;
        private If12FormTypeBL _f12;
        private If18FormSegmentBL _f18;
        private If19QuestionBL _f19;
        private If21ReplyUnitBL _f21;
        private If22ReplySetBL _f22;
        private If25ChessBoardBL _f25;
        private If26BatteryBoardBL _f26;
        private If29PortalQuestionTabBL _f29;
        private If31FilledQuestionPublishingBL _f31;
        private If32FilledValueBL _f32;
        private If44QuestionClusterBL _f44;
        private Ih04ToDoBL _h04;
        private Ih07ToDoTypeBL _h07;
        private Ih11NoticeBoardBL _h11;

        private Ij04UserRoleBL _j04;
        private Ij11TeamBL _j11;

        private Io15AutoCompleteBL _o15;
        private Io13AttachmentTypeBL _o13;
        private Io27AttachmentBL _o27;
        private Io53TagGroupBL _o53;
        private Io51TagBL _o51;
        
        private IDataGridBL _grid;
        private ICBL _cbl;
        private IFBL _fbl;
        private Ip85TempboxBL _p85;
        private Ix27EvalFunctionBL _x27;
        private Ix31ReportBL _x31;
        private Ix32ReportTypeBL _x32;
        private Ix51HelpCoreBL _x51;

        private IMailBL _mail;
        private IWorkflowBL _workflow;
        private Ik01TeacherBL _k01;

        public Factory(BO.RunningUser c,BL.RunningApp runningapp,BL.TheEntitiesProvider ep,BL.TheGlobalParams gp,BL.TheTranslator tt)
        {
                        
            this.CurrentUser = c;
            this.App = runningapp;
            this.EProvider = ep;
            this.GlobalParams = gp;
            this.Translator = tt;
            
            if (c.pid == 0 && string.IsNullOrEmpty(c.j03Login)==false)
            {
                InhaleUserByLogin(c.j03Login);
                
            }
            
        }

      
        public void InhaleUserByLogin(string strLogin)
        {
            DL.DbHandler db = new DL.DbHandler(this.App.ConnectString, this.CurrentUser,this.App.LogFolder);
            //this.CurrentUser= db.Load<BO.RunningUser>("SELECT a.j03ID as pid,a.j02ID,a.j03Login,j02.j02FirstName+' '+j02.j02LastName as FullName,a.j03FontStyleFlag,a.j03GridSelectionModeFlag,a.j03IsMustChangePassword,j04.j04RoleValue,CASE WHEN GETDATE() BETWEEN a.j03ValidFrom AND a.j03ValidUntil THEN 0 ELSE 1 end as isclosed,a.j03LiveChatTimestamp,j02.j02Email,a.j03PingTimestamp,a.j03LangIndex,a.j04ID FROM j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID WHERE a.j03Login LIKE @login", new { login = strLogin });
            this.CurrentUser = db.Load<BO.RunningUser>("exec dbo._core_j03user_load_sysuser @login", new { login = strLogin });
           


        }
        //logování přihlášení musí být zde, protože se logují i neńsspěšné pokusy o přihlášení a nešlo by to řešit v j03UserBL
        public void Write2AccessLog(BO.j90LoginAccessLog c) //zápis úspěšných přihlášení i neúspěšných pokusů o přihlášení
        {                                  
            DL.DbHandler db = new DL.DbHandler(this.App.ConnectString, this.CurrentUser, this.App.LogFolder);
            string s = "INSERT INTO j90LoginAccessLog(j03ID,j90Date,j90ClientBrowser,j90BrowserFamily,j90BrowserOS,j90BrowserDeviceType,j90BrowserDeviceFamily,j90BrowserAvailWidth,j90BrowserAvailHeight,j90BrowserInnerWidth,j90BrowserInnerHeight,j90LoginMessage,j90LoginName,j90CookieExpiresInHours,j90LocationHost,j90AppClient,j90Platform,j90UserHostAddress)";
            s += " VALUES(@j03id,GETDATE(),@useragent,@browser,@os,@devicetype,@devicefamily,@aw,@ah,@iw,@ih,@mes,@loginname,@cookieexpire,@host,'CORE',@os,@host)";
            db.RunSql(s,new {j03id=BO.BAS.TestIntAsDbKey(c.j03ID), useragent = c.j90ClientBrowser,browser= c.j90BrowserFamily,os=c.j90BrowserOS, devicetype=c.j90BrowserDeviceType, devicefamily=c.j90BrowserDeviceFamily,aw=c.j90BrowserAvailWidth,ah=c.j90BrowserAvailHeight,iw=c.j90BrowserInnerWidth,ih=c.j90BrowserInnerHeight,mes=c.j90LoginMessage, loginname=c.j90LoginName, cookieexpire=c.j90CookieExpiresInHours, host=c.j90LocationHost });
        }
        public bool IsUserAdmin()
        {
            if (this.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string tra(string strExpression)   //lokalizace do ostatních jazyků
        {
            if (this.CurrentUser.j03LangIndex == 0) return strExpression;
            return this.Translator.DoTranslate(strExpression, this.CurrentUser.j03LangIndex);
        }
        

        public IDataGridBL gridBL
        {
            get
            {
                if (_grid == null) _grid = new DataGridBL(this);
                return _grid;
            }
        }
        public ICBL CBL
        {
            get
            {
                if (_cbl == null) _cbl = new CBL(this);
                return _cbl;
            }
        }
        public IFBL FBL
        {
            get
            {
                if (_fbl == null) _fbl = new FBL(this);
                return _fbl;
            }
        }

        public If06FormBL f06FormBL
        {
            get
            {
                if (_f06 == null) _f06 = new f06FormBL(this);
                return _f06;
            }
        }
        public If12FormTypeBL f12FormTypeBL
        {
            get
            {
                if (_f12 == null) _f12 = new f12FormTypeBL(this);
                return _f12;
            }
        }
        public If18FormSegmentBL f18FormSegmentBL
        {
            get
            {
                if (_f18 == null) _f18 = new f18FormSegmentBL(this);
                return _f18;
            }
        }
        public If19QuestionBL f19QuestionBL
        {
            get
            {
                if (_f19 == null) _f19 = new f19QuestionBL(this);
                return _f19;
            }
        }
        public If21ReplyUnitBL f21ReplyUnitBL
        {
            get
            {
                if (_f21 == null) _f21 = new f21ReplyUnitBL(this);
                return _f21;
            }
        }
        public If22ReplySetBL f22ReplySetBL
        {
            get
            {
                if (_f22 == null) _f22 = new f22ReplySetBL(this);
                return _f22;
            }
        }
        public If25ChessBoardBL f25ChessBoardBL
        {
            get
            {
                if (_f25 == null) _f25 = new f25ChessBoardBL(this);
                return _f25;
            }
        }
        public If26BatteryBoardBL f26BatteryBoardBL
        {
            get
            {
                if (_f26 == null) _f26 = new f26BatteryBoardBL(this);
                return _f26;
            }
        }
        public If29PortalQuestionTabBL f29PortalQuestionTabBL
        {
            get
            {
                if (_f29 == null) _f29 = new f29PortalQuestionTabBL(this);
                return _f29;
            }
        }
        public If31FilledQuestionPublishingBL f31FilledQuestionPublishingBL
        {
            get
            {
                if (_f31 == null) _f31 = new f31FilledQuestionPublishingBL(this);
                return _f31;
            }
        }
        public If32FilledValueBL f32FilledValueBL
        {
            get
            {
                if (_f32 == null) _f32 = new f32FilledValueBL(this);
                return _f32;
            }
        }
        public If44QuestionClusterBL f44QuestionClusterBL
        {
            get
            {
                if (_f44 == null) _f44 = new f44QuestionClusterBL(this);
                return _f44;
            }
        }
        public Ih04ToDoBL h04ToDoBL
        {
            get
            {
                if (_h04 == null) _h04 = new h04ToDoBL(this);
                return _h04;
            }
        }
        public Ih07ToDoTypeBL h07ToDoTypeBL
        {
            get
            {
                if (_h07 == null) _h07 = new h07ToDoTypeBL(this);
                return _h07;
            }
        }
        public Ih11NoticeBoardBL h11NoticeBoardBL
        {
            get
            {
                if (_h11 == null) _h11 = new h11NoticeBoardBL(this);
                return _h11;
            }
        }
        public Ij03UserBL j03UserBL
        {
            get
            {
                if (_j03 == null) _j03 = new j03UserBL(this);
                return _j03;
            }
        }
        public Ij02PersonBL j02PersonBL
        {
            get
            {
                if (_j02 == null) _j02 = new j02PersonBL(this);
                return _j02;
            }
        }
        public Ij04UserRoleBL j04UserRoleBL
        {
            get
            {
                if (_j04 == null) _j04= new j04UserRoleBL(this);
                return _j04;
            }
        }
        public Ij11TeamBL j11TeamBL
        {
            get
            {
                if (_j11 == null) _j11 = new j11TeamBL(this);
                return _j11;
            }
        }
        public Ij23NonPersonBL j23NonPersonBL
        {
            get
            {
                if (_j23 == null) _j23 = new j23NonPersonBL(this);
                return _j23;
            }
        }
        public Ij24NonPersonTypeBL j24NonPersonTypeBL
        {
            get
            {
                if (_j24 == null) _j24 = new j24NonPersonTypeBL(this);
                return _j24;
            }
        }
        public Ij25NonPersonPlanReasonBL j25NonPersonPlanReasonBL
        {
            get
            {
                if (_j25 == null) _j25 = new j25NonPersonPlanReasonBL(this);
                return _j25;
            }
        }
        public Ij26HolidayBL j26HolidayBL
        {
            get
            {
                if (_j26 == null) _j26 = new j26HolidayBL(this);
                return _j26;
            }
        }
        public Ij72TheGridTemplateBL j72TheGridTemplateBL
        {
            get
            {
                if (_j72 == null) _j72 = new j72TheGridTemplateBL(this);
                return _j72;
            }
        }
        public Ia25EventFormGroupBL a25EventFormGroupBL
        {
            get
            {
                if (_a25 == null) _a25 = new a25EventFormGroupBL(this);
                return _a25;
            }
        }
        public Ia29InstitutionListBL a29InstitutionListBL
        {
            get
            {
                if (_a29 == null) _a29 = new a29InstitutionListBL(this);
                return _a29;
            }
        }

        public Ia01EventBL a01EventBL
        {
            get
            {
                if (_a01 == null) _a01 = new a01EventBL(this);
                return _a01;
            }
        }
        public Ia02InspectorBL a02InspectorBL
        {
            get
            {
                if (_a02 == null) _a02 = new a02InspectorBL(this);
                return _a02;
            }
        }
        public Ia03InstitutionBL a03InstitutionBL
        {
            get
            {
                if (_a03 == null) _a03 = new a03InstitutionBL(this);
                return _a03;
            }
        }
        public Ia04InspectorateBL a04InspectorateBL
        {
            get
            {
                if (_a04 == null) _a04 = new a04InspectorateBL(this);
                return _a04;
            }
        }
        public Ia05RegionBL a05RegionBL
        {
            get
            {
                if (_a05 == null) _a05 = new a05RegionBL(this);
                return _a05;
            }
        }
        public Ia08ThemeBL a08ThemeBL
        {
            get
            {
                if (_a08 == null) _a08 = new a08ThemeBL(this);
                return _a08;
            }
        }
        public Ia09FounderTypeBL a09FounderTypeBL
        {
            get
            {
                if (_a09 == null) _a09 = new a09FounderTypeBL(this);
                return _a09;
            }
        }
        public Ia10EventTypeBL a10EventTypeBL
        {
            get
            {
                if (_a10 == null) _a10 = new a10EventTypeBL(this);
                return _a10;
            }
        }
        public Ia11EventFormBL a11EventFormBL
        {
            get
            {
                if (_a11 == null) _a11 = new a11EventFormBL(this);
                return _a11;
            }
        }
        public Ia17DepartmentTypeBL a17DepartmentTypeBL
        {
            get
            {
                if (_a17 == null) _a17 = new a17DepartmentTypeBL(this);
                return _a17;
            }
        }
        public Ia18DepartmentDomainBL a18DepartmentDomainBL
        {
            get
            {
                if (_a18 == null) _a18 = new a18DepartmentDomainBL(this);
                return _a18;
            }
        }
        public Ia19DomainToInstitutionDepartmentBL a19DomainToInstitutionDepartment
        {
            get
            {
                if (_a19 == null) _a19 = new a19DomainToInstitutionDepartmentBL(this);
                return _a19;
            }
        }
        public Ia39InstitutionPersonBL a39InstitutionPersonBL
        {
            get
            {
                if (_a39 == null) _a39 = new a39InstitutionPersonBL(this);
                return _a39;
            }
        }
        public Ia37InstitutionDepartmentBL a37InstitutionDepartmentBL
        {
            get
            {
                if (_a37 == null) _a37 = new a37InstitutionDepartmentBL(this);
                return _a37;
            }
        }
        public Ia35PersonEventPlanBL a35PersonEventPlanBL
        {
            get
            {
                if (_a35 == null) _a35 = new a35PersonEventPlanBL(this);
                return _a35;
            }
        }
        public Ia38NonPersonEventPlanBL a38NonPersonEventPlanBL
        {
            get
            {
                if (_a38 == null) _a38 = new a38NonPersonEventPlanBL(this);
                return _a38;
            }
        }
        public Ia41PersonToEventBL a41PersonToEventBL
        {
            get
            {
                if (_a41 == null) _a41 = new a41PersonToEventBL(this);
                return _a41;
            }
        }
        public Ia42QesBL a42QesBL
        {
            get
            {
                if (_a42 == null) _a42 = new a42QesBL(this);
                return _a42;
            }
        }
        public Ia70SISBL a70SISBL
        {
            get
            {
                if (_a70 == null) _a70 = new a70SISBL(this);
                return _a70;
            }
        }
        public Ib01WorkflowTemplateBL b01WorkflowTemplateBL
        {
            get
            {
                if (_b01 == null) _b01 = new b01WorkflowTemplateBL(this);
                return _b01;
            }
        }
        public Ib02WorkflowStatusBL b02WorkflowStatusBL
        {
            get
            {
                if (_b02 == null) _b02 = new b02WorkflowStatusBL(this);
                return _b02;
            }
        }
        public Ib06WorkflowStepBL b06WorkflowStepBL
        {
            get
            {
                if (_b06 == null) _b06 = new b06WorkflowStepBL(this);
                return _b06;
            }
        }
        public Ib05Workflow_HistoryBL b05Workflow_HistoryBL
        {
            get
            {
                if (_b05 == null) _b05 = new b05Workflow_HistoryBL(this);
                return _b05;
            }
        }
        public Ib65WorkflowMessageBL b65WorkflowMessageBL
        {
            get
            {
                if (_b65 == null) _b65 = new b65WorkflowMessageBL(this);
                return _b65;
            }
        }
        public Io15AutoCompleteBL o15AutoCompleteBL
        {
            get
            {
                if (_o15 == null) _o15 = new o15AutoCompleteBL(this);
                return _o15;
            }
        }
        public Io13AttachmentTypeBL o13AttachmentTypeBL
        {
            get
            {
                if (_o13 == null) _o13 = new o13AttachmentTypeBL(this);
                return _o13;
            }
        }
        public Io27AttachmentBL o27AttachmentBL
        {
            get
            {
                if (_o27 == null) _o27 = new o27AttachmentBL(this);
                return _o27;
            }
        }
        public Io53TagGroupBL o53TagGroupBL
        {
            get
            {
                if (_o53 == null) _o53 = new o53TagGroupBL(this);
                return _o53;
            }
        }
        public Io51TagBL o51TagBL
        {
            get
            {
                if (_o51 == null) _o51 = new o51TagBL(this);
                return _o51;
            }
        }
        
        
        
        public Ip85TempboxBL p85TempboxBL
        {
            get
            {
                if (_p85 == null) _p85 = new p85TempboxBL(this);
                return _p85;
            }
        }

        public Ix27EvalFunctionBL x27EvalFunctionBL
        {
            get
            {
                if (_x27 == null) _x27 = new x27EvalFunctionBL(this);
                return _x27;
            }
        }
        public Ix31ReportBL x31ReportBL
        {
            get
            {
                if (_x31 == null) _x31 = new x31ReportBL(this);
                return _x31;
            }
        }

        public Ix32ReportTypeBL x32ReportTypeBL
        {
            get
            {
                if (_x32 == null) _x32 = new x32ReportTypeBL(this);
                return _x32;
            }
        }
        public Ix51HelpCoreBL x51HelpCoreBL
        {
            get
            {
                if (_x51 == null) _x51 = new x51HelpCoreBL(this);
                return _x51;
            }
        }
        public Ik01TeacherBL k01TeacherBL
        {
            get
            {
                if (_k01 == null) _k01 = new k01TeacherBL(this);
                return _k01;
            }
        }

        public IMailBL MailBL
        {
            get
            {
                if (_mail == null) _mail = new MailBL(this);
                return _mail;
            }
        }
        public IWorkflowBL WorkflowBL
        {
            get
            {
                if (_workflow == null) _workflow = new WorkflowBL(this);
                return _workflow;
            }
        }
    }
}
