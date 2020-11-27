using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheEntitiesProvider
    {
        private readonly BL.RunningApp _app;
        private readonly BL.TheTranslator _tt;
        private List<BO.TheEntity> _lis;
        public TheEntitiesProvider(BL.RunningApp runningapp, BL.TheTranslator tt)
        {
            _app = runningapp;
            _tt = tt;

            SetupPallete();

            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            var dt = db.GetDataTable("select x29ID,x29Prefix from x29Entity");
            foreach (System.Data.DataRow dbrow in dt.Rows)
            {
                if (_lis.Where(p => p.Prefix == dbrow["x29Prefix"].ToString()).Count()>0)
                {
                    ByPrefix(dbrow["x29Prefix"].ToString()).x29ID = Convert.ToInt32(dbrow["x29ID"]);
                    
                }
                
                
            }
        }

        public BO.TheEntity ByPrefix(string strPrefix)
        {            
            return _lis.Where(p => p.Prefix == strPrefix).First();
        }
        public BO.TheEntity ByTable(string strTableName)
        {            
            return _lis.Where(p => p.TableName == strTableName).First();
        }
        public BO.TheEntity ByX29ID(int intX29ID)
        {
            if (intX29ID == 0) return null;

            if (_lis.Where(p => p.x29ID == intX29ID).Count() > 0)
            {
                return _lis.Where(p => p.x29ID == intX29ID).First();
            }

            return null;
        }


        private void SetupPallete()
        {
            _lis = new List<BO.TheEntity>();
            AE("j02Person", "Lidé", "Osobní profil", "j02Person a", "a.j02LastName,a.j02FirstName");
            AE("j03User", "Uživatelé", "Uživatel", "j03User a INNER JOIN j04UserRole j03_j04 ON a.j04ID=j03_j04.j04ID", "a.j03Login");
            AE("j04UserRole", "Aplikační role", "Aplikační role", "j04UserRole a ", "a.j04Name");
            AE("j11Team", "Týmy osob", "Tým", "j11Team a ", "a.j11Name");

            AE("j23NonPerson", "Nepersonální zdroje", "Nepersonální zdroj", "j23NonPerson a ", "a.j23Name");
            AE("j24NonPersonType", "Typy nepersonálních zdrojů", "Typ nepersonálního zdroje", "j24NonPersonType a ", "a.j24Name");

            if (_app.Implementation == "HD")
            {
                AE("a01Event", "Požadavky", "Požadavek", "a01Event a INNER JOIN b02WorkflowStatus bc ON a.b02ID=bc.b02ID", "a.a01ID DESC", "a.a01ID DESC");
            }
            else
            {
                AE("a01Event", "Akce", "Akce", "a01Event a INNER JOIN b02WorkflowStatus bc ON a.b02ID=bc.b02ID", "a.a01ID DESC", "a.a01ID DESC");
            }
            
            AE("a03Institution", "Instituce", "Instituce", "a03Institution a", "a.a03Name");
            AE("a04Inspectorate", "Inspektoráty", "Inspektorát", "a04Inspectorate a", "a.a04City");

            AE("a41PersonToEvent", "Účastníci akce", "Účastník akce", "a41PersonToEvent a", "a.a41ID", "a.a41ID");
            AE("a42Qes", "INEZ", "INEZ", "a42Qes a", "a.a42ID DESC", "a.a42ID DESC");

            AE("a05Region", "Regiony", "Region", "a05Region a", "a.a05Ordinal,a.a05Name", "a.a05Ordinal, a.a05Name");
            AE("a06InstitutionType", "Typy institucí", "Typ instituce", "a06InstitutionType a", "a.a06Name");

            AE("a08Theme", "Témata akcí", "Téma akce", "a08Theme a", "a.a08Name");
            AE("a09FounderType", "Typy zřizovatelů", "Typ zřizovatele", "a09FounderType a", "a.a09Ordinal", "a.a09Ordinal");
            AE("a10EventType", "Typy akcí", "Typ akce", "a10EventType a", "a.a10Name");
            AE("a11EventForm", "Formuláře v akci", "Formulář v akci", "a11EventForm a INNER JOIN a01Event a11_a01 ON a.a01ID=a11_a01.a01ID", "a.a11ID");
            ByPrefix("a11").IsWithoutValidity = true;
            AE("a17DepartmentType", "Typy činností", "Typ činnosti", "a17DepartmentType a","a.a17Name","a.a17Name");
            AE("a18DepartmentDomain", "Kódy vzdělávacích oborů", "Kód vzdělávacího oboru", "a18DepartmentDomain a", "a.a18Name");
            AE("a19DomainToInstitutionDepartment", "Vzdělávací obory", "Vzdělávací obor", "a19DomainToInstitutionDepartment a", "a.a19ID");            
            ByPrefix("a19").IsWithoutValidity = true;            

            AE("a21InstitutionLegalType", "Právní formy", "Právní forma", "a21InstitutionLegalType a", "a.a21Name");
            AE("a25EventFormGroup", "Skupiny formulářů", "Skupina formulářů", "a25EventFormGroup a", "a.a25Name", "a.a25ID DESC");
            AE("a29InstitutionList", "Pojmenované seznamy institucí", "Pojmenovaný seznam institucí", "a29InstitutionList a", "a.a29Name");
            AE("a35PersonEventPlan", "Časový plán akce", "Časový plán akce", "a35PersonEventPlan a", "a.a35ID");
            AE("a37InstitutionDepartment", "Činnosti instituce", "Činnost instituce", "a37InstitutionDepartment a", "a.a37Name");

            AE("a38NonPersonEventPlan", "Rezervace nepersonálních zdrojů", "Rezervace nepersonálních zdrojů", "a38NonPersonEventPlan a", "");

            AE("a39InstitutionPerson", "Kontaktní osoby", "Kontaktní osoba", "a39InstitutionPerson a", "");
            AE("a45EventRole", "Role v akci", "Role v akci", "a45EventRole a","a.a45ID");
            ByPrefix("a45").IsWithoutValidity = true;

            AE("b01WorkflowTemplate", "Workflow šablony", "Workflow šablona", "b01WorkflowTemplate a", "a.b01Name");
            AE("b02WorkflowStatus", "Workflow stavy", "Workflow stav", "b02WorkflowStatus a", "a.b02Order,a.b02Name","a.b02Order,a.b02Name");
            AE("b05Workflow_History", "Workflow historie", "Workflow historie", "b05Workflow_History a", "a.b05ID DESC");
            ByPrefix("b05").IsWithoutValidity = true;

            AE("b06WorkflowStep", "Workflow kroky", "Workflow krok", "b06WorkflowStep a", "a.b06Order", "a.b06Order");
            AE("b65WorkflowMessage", "Workflow notifikační zprávy", "Workflow notifikační zpráva", "b65WorkflowMessage a", "a.b65Name");

            AE("f06Form", "Formuláře", "Formulář", "f06Form a", "a.f06Ordinal,a.f06Name", "a.f06Ordinal,a.f06Name");
            AE("f12FormType", "Typy formulářů", "Typ formuláře", "f12FormType a", "a.f12TreeIndex");
            AE("f18FormSegment", "Formulářové segmenty", "Segment formuláře", "f18FormSegment a", "a.f18TreeIndex,a.f18Ordinal");
            AE("f19Question", "Formulářové otázky", "Otázka formuláře", "f19Question a", "a.f18ID,a.f19Ordinal", "a.f18ID,a.f19Ordinal");

            AE("h04ToDo", "Úkoly", "Úkol", "h04ToDo a", "a.h04ID DESC");
            AE("h07ToDoType", "Typy úkolů", "Typ úkolu", "h07ToDoType a", "a.h07ID");
            AE("h11NoticeBoard", "Články pro nástěnku", "Článek pro nástěnku", "h11NoticeBoard a", "a.h11ID DESC");

            AE("x40MailQueue", "OUTBOX", "OUTBOX", "x40MailQueue a", "a.x40ID DESC", "a.x40ID DESC");
            ByPrefix("x40").IsWithoutValidity = true;
            
            AE("k01Teacher", "Učitelé", "Učitel", "k01Teacher a LEFT OUTER JOIN k02TeacherSchool a03_k02 ON a.k01ID=a03_k02.k01ID", "a.k01LastName");

            AE("o15AutoComplete", "AutoComplete položky", "AutoComplete položka", "o15AutoComplete a", "a.o15Flag");
            
            AE("o13AttachmentType", "Typy příloh", "Typ přílohy", "o13AttachmentType a", "a.o13TreeIndex", "a.o13TreeIndex");
            AE("o27Attachment", "Přílohy", "Příloha", "o27Attachment a", "a.o27ID DESC", "a.o27ID DESC");
            AE("x29Entity", "Entity", "Entita", "x29Entity a","a.x29Name");
            ByPrefix("x29").IsWithoutValidity = true;

            AE_TINY("a02Inspector", "Inspektoři", "Inspektor");
            AE_TINY("a24EventRelation", "Související akce", "Související akce");
            AE_TINY("a28SchoolType", "Typy škol", "Typ školy");
            AE_TINY("a70SIS", "Školní IS", "Školní IS");

            AE_TINY("o51Tag", "Položky kategorií", "Položka kategorie");
            AE_TINY("o53TagGroup", "Kategorie", "Kategorie");
            AE_TINY("o54TagBindingInline", "Kategorizace", "Kategorizace");

            AE_TINY("j25NonPersonPlanReason", "Důvody rezervace", "Důvod rezervace");
            AE_TINY("j26Holiday", "Dny svátků", "Svátek");
            AE_TINY("j40MailAccount", "Poštovní účty", "Poštovní účet");

            AE("f23ReplyType", "Typy otázek", "Typ odpovědi", "f23ReplyType a", "a.f23Name", "a.f23Name");

            AE("f29PortalQuestionTab", "Portál boxy", "Portal box", "f29PortalQuestionTab a", "a.f29Ordinal", "a.f29Ordinal");
            AE_TINY("f44QuestionCluster", "Klastry otázek", "Klastr otázek");
            AE("f21ReplyUnit", "Jednotky odpovědí", "Jednotka odpovědi", "f21ReplyUnit a", "a.f21Ordinal,a.f21Name", "a.f21Ordinal,a.f21Name");
            AE_TINY("f22ReplySet", "Šablony jednotek", "Šablona jednotek");
            AE_TINY("f25ChessBoard", "Šachovnice otázek", "Šachovnice otázek");
            AE_TINY("f26BatteryBoard", "Baterie otázek", "Baterie otázek");
            //AE_TINY("f31FilledQuestionPublishing", "f31", "f31");
            //ByPrefix("f31").IsWithoutValidity = true;

            AE_TINY("h05ToDoStatus", "Stavy úkolu", "Stav úkolu");
            ByPrefix("h05").IsWithoutValidity = true;

            AE_TINY("j90LoginAccessLog", "LOGIN Log", "LOGIN Log");
            AE_TINY("j92PingLog", "PING Log", "PING Log");
            AE_TINY("x24DataType", "Datové typy", "Datový typ");
            AE_TINY("b09WorkflowCommandCatalog", "WORKFLOW příkazy", "WORKFLOW příkaz");
            ByPrefix("b09").IsWithoutValidity = true;
            AE_TINY("j72TheGridTemplate", "Grid", "Grid");
            ByPrefix("j72").IsWithoutValidity = true;

            AE("x27EvalFunction", "EVAL funkce", "EVAL funkce", "x27EvalFunction a", "a.x27Ordinal", "a.x27Ordinal");

            AE_TINY("x31Report", "Pevné tiskové sestavy", "Pevná tisková sestava");
            AE("z01_core_view_reports", "Pevné tiskové sestavy", "Tisková sestava", "z01_core_view_reports a", "a.x31Name", "a.x31Name");
            AE("x32ReportType", "Kategorie sestav", "Kategorie sestavy", "x32ReportType a", "a.x32TreeIndex", "a.x32TreeIndex");

            AE_TINY("b65WorkflowMessage", "Notifikační zprávy", "Notifikační zpráva");
            AE_TINY("x51HelpCore", "Uživatelská nápověda", "Uživatelská nápověda");
            AE_TINY("x91Translate", "Aplikační překlad", "Aplikační překlad");
            ByPrefix("x91").IsWithoutValidity = true;

            AE_TINY("p86TempStat", "Statistika", "Statistika");
            ByPrefix("p86").IsWithoutValidity = true;
            
            AE_TINY("v_uraz_jmenozraneneho", "Úraz1", "Úraz1");
            AE_TINY("v_uraz_datumzraneni", "Úraz2", "Úraz2");
            AE_TINY("v_uraz_poradovecislo", "Úraz3", "Úraz3");


        }

        private void AE(string strTabName, string strPlural, string strSingular, string strSqlFromGrid, string strSqlOrderByCombo, string strSqlOrderBy = null)
        {
            
            if (strSqlOrderBy == null) strSqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC";
            BO.TheEntity c = new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFromGrid = strSqlFromGrid, SqlOrderByCombo = strSqlOrderByCombo, SqlOrderBy = strSqlOrderBy };
            c.TranslateLang1 = _tt.DoTranslate(strPlural, 1);
            c.TranslateLang2 = _tt.DoTranslate(strPlural, 2);
            _lis.Add(c);

        }
        private void AE_TINY(string strTabName, string strPlural, string strSingular)
        {

            _lis.Add(new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFromGrid = strTabName + " a", SqlOrderByCombo = "a." + strTabName.Substring(0, 3) + "Name", SqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC",TranslateLang1= _tt.DoTranslate(strPlural, 1),TranslateLang2= _tt.DoTranslate(strPlural, 2) });
        }
        private BO.EntityRelation getREL(string strTabName, string strRelName, string strSingular, string strSqlFrom, string strDependOnRel = null)
        {
            return new BO.EntityRelation() { TableName = strTabName, RelName = strRelName, AliasSingular = strSingular, SqlFrom = strSqlFrom, RelNameDependOn = strDependOnRel,Translate1=_tt.DoTranslate(strSingular,1), Translate2 = _tt.DoTranslate(strSingular, 2) };



        }

        public List<BO.EntityRelation> getApplicableRelations(string strPrimaryPrefix)
        {
            
            var lis = new List<BO.EntityRelation>();
            BO.TheEntity ce = ByPrefix(strPrimaryPrefix);           

            switch (strPrimaryPrefix)
            {
                case "j02":
                    lis.Add(getREL("j03User", "j02_j03", "Uživatelský účet", "LEFT OUTER JOIN j03User j02_j03 ON a.j02ID=j02_j03.j02ID LEFT OUTER JOIN j04UserRole j03_j04 ON j02_j03.j04ID=j03_j04.j04ID"));
                    lis.Add(getREL("a04Inspectorate", "j02_a04", "Inspektorát", "LEFT OUTER JOIN a02Inspector j02_a02 ON a.j02ID=j02_a02.j02ID LEFT OUTER JOIN a04Inspectorate j02_a04 ON j02_a02.a04ID=j02_a04.a04ID"));
                    lis.Add(getREL("a05Region", "j02_a05", "Kraj", "LEFT OUTER JOIN a05Region j02_a05 ON j02_a04.a05ID=j02_a05.a05ID", "j02_a04"));

                    lis.Add(getREL("a03Institution", "j02_employer", "Zaměstnavatel", "LEFT OUTER JOIN (SELECT xb.j02ID,xa.* from a03Institution xa INNER JOIN a39InstitutionPerson xb ON xa.a03ID=xb.a03ID WHERE xb.a39RelationFlag=2) j02_employer ON a.j02ID=j02_employer.j02ID"));

                    lis.Add(getREL("o54TagBindingInline", "j02_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='j02') j02_o54 ON a.j02ID=j02_o54.o54RecordPid"));
                    break;
                case "j03":
                    lis.Add(getREL("j02Person", "j03_j02", "Osobní profil", "LEFT OUTER JOIN j02Person j03_j02 ON a.j02ID=j03_j02.j02ID"));
                    break;
                case "j90":
                    lis.Add(getREL("j03User", "j90_j03", "Uživatelský účet", "INNER JOIN j03User j90_j03 ON a.j03ID=j90_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j90_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "j92":
                    lis.Add(getREL("j03User", "j92_j03", "Uživatelský účet", "INNER JOIN j03User j92_j03 ON a.j03ID=j92_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j92_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "a01":
                    lis.Add(getREL("j02Person", "a01_j02", "Zadavatel", "LEFT OUTER JOIN j02Person a01_j02 ON a.j02ID_Issuer=a01_j02.j02ID"));
                    lis.Add(getREL("a03Institution", "a01_a03", "Instituce", "LEFT OUTER JOIN a03Institution a01_a03 ON a.a03ID=a01_a03.a03ID"));
                    lis.Add(getREL("b02WorkflowStatus", "a01_b02", "Aktuální stav", "LEFT OUTER JOIN b02WorkflowStatus a01_b02 ON a.b02ID=a01_b02.b02ID"));
                    lis.Add(getREL("a10EventType", "a01_a10", "Typ akce", "INNER JOIN a10EventType a01_a10 ON a.a10ID=a01_a10.a10ID"));
                    lis.Add(getREL("a08Theme", "a01_a08", "Téma akce", "INNER JOIN a08Theme a01_a08 ON a.a08ID=a01_a08.a08ID"));
                    lis.Add(getREL("a05Region", "a01_a05", "Kraj", "LEFT OUTER JOIN a05Region a01_a05 ON a01_a03.a05ID=a01_a05.a05ID", "a01_a03"));
                    lis.Add(getREL("a09FounderType", "a01_a09", "Typ zřizovatele", "LEFT OUTER JOIN a09FounderType a01_a09 ON a01_a03.a09ID=a01_a09.a09ID", "a01_a03"));
                    lis.Add(getREL("a42Qes", "a01_a42", "INEZ", "LEFT OUTER JOIN a42Qes a01_a42 ON a.a42ID=a01_a42.a42ID"));
                    lis.Add(getREL("a01Event", "a01_parent", "Nadřízená akce", "LEFT OUTER JOIN a01Event a01_parent ON a.a01ParentID=a01_parent.a01ID"));
                    lis.Add(getREL("o54TagBindingInline", "a01_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='a01') a01_o54 ON a.a01ID=a01_o54.o54RecordPid"));
                    if (_app.Implementation == "Default")
                    {
                        lis.Add(getREL("v_uraz_jmenozraneneho", "a01_xxa", "Úraz1", "LEFT OUTER JOIN v_uraz_jmenozraneneho a01_xxa ON a.a01ID=a01_xxa.a01ID"));
                        lis.Add(getREL("v_uraz_datumzraneni", "a01_xxb", "Úraz2", "LEFT OUTER JOIN v_uraz_datumzraneni a01_xxb ON a.a01ID=a01_xxb.a01ID"));
                        lis.Add(getREL("v_uraz_poradovecislo", "a01_xxc", "Úraz3", "LEFT OUTER JOIN v_uraz_poradovecislo a01_xxc ON a.a01ID=a01_xxc.a01ID"));
                    }
                    
                    break;
                case "a02":
                    lis.Add(getREL("a04Inspectorate", "a02_a04", "Inspektorát", "INNER JOIN a04Inspectorate a02_a04 ON a.a04ID=a02_a04.a04ID"));
                    lis.Add(getREL("a05Region", "a02_a05", "Kraj", "LEFT OUTER JOIN a05Region a02_a05 ON a02_a04.a05ID=a02_a05.a05ID", "a02_a04"));
                    lis.Add(getREL("j02Person", "a02_j02", "Osoba", "LEFT OUTER JOIN j02Person a02_j02 ON a.j02ID=a02_j02.j02ID"));
                    break;
                case "a03":
                    lis.Add(getREL("a05Region", "a03_a05", "Kraj", "LEFT OUTER JOIN a05Region a03_a05 ON a.a05ID=a03_a05.a05ID"));
                    lis.Add(getREL("a06InstitutionType", "a03_a06", "Typ", "LEFT OUTER JOIN a06InstitutionType a03_a06 ON a.a06ID=a03_a06.a06ID"));
                    lis.Add(getREL("a09FounderType", "a03_a09", "Typ zřizovatele", "LEFT OUTER JOIN a09FounderType a03_a09 ON a.a09ID=a03_a09.a09ID"));
                    lis.Add(getREL("a03Institution", "a03_founder", "Zřizovatel", "LEFT OUTER JOIN a03Institution a03_founder ON a.a03ID_Founder=a03_founder.a03ID"));
                    lis.Add(getREL("a03Institution", "a03_supervisory", "Dohled", "LEFT OUTER JOIN a03Institution a03_supervisory ON a.a03ID_Supervisory=a03_supervisory.a03ID"));
                    lis.Add(getREL("a03Institution", "a03_parent", "Nadřízená škola", "LEFT OUTER JOIN a03Institution a03_parent ON a.a03ID_Parent=a03_parent.a03ID"));
                    lis.Add(getREL("a21InstitutionLegalType", "a03_a21", "Právní forma", "LEFT OUTER JOIN a21InstitutionLegalType a03_a21 ON a.a21ID=a03_a21.a21ID"));
                    if (_app.Implementation == "Default")
                    {
                        lis.Add(getREL("a70SIS", "a03_a70", "Školní IS", "LEFT OUTER JOIN a70SIS a03_a70 ON a.a70ID=a03_a70.a70ID"));
                    }                    
                    lis.Add(getREL("a28SchoolType", "a03_a28", "Typ školy", "LEFT OUTER JOIN a28SchoolType a03_a28 ON a.a28ID=a03_a28.a28ID"));
                    if (_app.Implementation == "Default")
                    {
                        lis.Add(getREL("k01Teacher", "a03_k01", "Učitel", "LEFT OUTER JOIN k02TeacherSchool a03_k02 ON a.a03ID=a03_k02.a03ID LEFT OUTER JOIN k01Teacher a03_k01 ON a03_k02.k01ID=a03_k01.k01ID"));
                    }
                    

                    lis.Add(getREL("o54TagBindingInline", "a03_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='a03') a03_o54 ON a.a03ID=a03_o54.o54RecordPid"));
                    break;
                case "a04":                   
                    lis.Add(getREL("a05Region", "a04_a05", "Kraj", "LEFT OUTER JOIN a05Region a04_a05 ON a.a05ID=a04_a05.a05ID"));
                    break;
                case "a10":
                    lis.Add(getREL("b01WorkflowTemplate", "a10_b01", "Workflow šablona", "LEFT OUTER JOIN b01WorkflowTemplate a10_b01 ON a.b01ID=a10_b01.b01ID"));

                    break;
                case "a11":
                    lis.Add(getREL("f06Form", "a11_f06", "Formulář", "LEFT OUTER JOIN f06Form a11_f06 ON a.f06ID=a11_f06.f06ID"));
                    lis.Add(getREL("a37InstitutionDepartment", "a11_a37", "Činnost školy", "LEFT OUTER JOIN a37InstitutionDepartment a11_a37 ON a.a37ID = a11_a37.a37ID"));
                    lis.Add(getREL("a25EventFormGroup", "a11_a25", "Skupina formulářů", "LEFT OUTER JOIN a25EventFormGroup a11_a25 ON a.a25ID=a11_a25.a25ID"));
                    if (_app.Implementation == "Default")
                    {
                        lis.Add(getREL("k01Teacher", "a11_k01", "Učitel", "LEFT OUTER JOIN k01Teacher a11_k01 ON a.k01ID=a11_k01.k01ID"));
                    }
                        
                    break;
                case "a19":
                    lis.Add(getREL("a37InstitutionDepartment", "a19_a37", "Činnost školy", "INNER JOIN a37InstitutionDepartment a19_a37 ON a.a37ID = a19_a37.a37ID"));
                    lis.Add(getREL("a18DepartmentDomain", "a19_a18", "Kód oboru", "INNER JOIN a18DepartmentDomain a19_a18 ON a.a18ID=a19_a18.a18ID"));
                                        
                    break;
                case "a42":
                    lis.Add(getREL("a08Theme", "a42_a08", "Téma", "INNER JOIN a08Theme a42_a08 ON a.a08ID=a42_a08.a08ID"));
                    lis.Add(getREL("j40MailAccount", "a42_j40", "Poštovní účet", "LEFT OUTER JOIN j40MailAccount a42_j40 ON a.j40ID=a42_j40.j40ID"));
                    break;
                case "b02":
                    lis.Add(getREL("b01WorkflowTemplate", "b02_b01", "Workflow šablona", "INNER JOIN b01WorkflowTemplate b02_b01 ON a.b01ID=b02_b01.b01ID"));

                    break;
                case "b05":
                    lis.Add(getREL("b02WorkflowStatus", "b05_b02from", "Stav z", "LEFT OUTER JOIN b02WorkflowStatus b05_b02from ON a.b02ID_From=b05_b02from.b02ID"));
                    lis.Add(getREL("b02WorkflowStatus", "b05_b02to", "Stav do", "LEFT OUTER JOIN b02WorkflowStatus b05_b02to ON a.b02ID_To=b05_b02to.b02ID"));
                    lis.Add(getREL("b06WorkflowStep", "b05_b06", "Workflow krok", "LEFT OUTER JOIN b06WorkflowStep b05_b06 ON a.b06ID=b05_b06.b06ID"));
                    lis.Add(getREL("j03User", "b05_j03", "Uživatel", "INNER JOIN j03User b05_j03 ON a.j03ID_Sys=b05_j03.j03ID"));
                    lis.Add(getREL("j02Person", "b05_j02", "Jméno", "LEFT OUTER JOIN j02Person b05_j02 ON b05_j03.j02ID=b05_j02.j02ID", "b05_j03"));
                    lis.Add(getREL("a01Event", "b05_a01", "Akce", "INNER JOIN a01Event b05_a01 ON a.a01ID=b05_a01.a01ID"));

                    break;
                case "b65":
                    lis.Add(getREL("x29Entity", "b65_x29", "Entita", "LEFT OUTER JOIN x29Entity b65_x29 ON a.x29ID=b65_x29.x29ID"));
                    break;
                case "f06":
                    lis.Add(getREL("f12FormType", "f06_f12", "Typ formuláře", "INNER JOIN f12FormType f06_f12 ON a.f12ID=f06_f12.f12ID"));
                    break;
                case "f19":
                    lis.Add(getREL("f23ReplyType", "f19_f23", "Typ odpovědi", "INNER JOIN f23ReplyType f19_f23 ON a.f23ID=f19_f23.f23ID"));
                    lis.Add(getREL("x24DataType", "f19_x24", "Datový typ", "LEFT OUTER JOIN x24DataType f19_x24 ON a.x24ID=f19_x24.x24ID"));
                    lis.Add(getREL("f18FormSegment", "f19_f18", "Segment formuláře", "INNER JOIN f18FormSegment f19_f18 ON a.f18ID=f19_f18.f18ID"));
                    lis.Add(getREL("f25ChessBoard", "f19_f25", "Šachovnice", "LEFT OUTER JOIN f25ChessBoard f19_f25 ON a.f25ID=f19_f25.f25ID"));
                    lis.Add(getREL("f26BatteryBoard", "f19_f26", "Baterie", "LEFT OUTER JOIN f26BatteryBoard f19_f26 ON a.f26ID=f19_f26.f26ID"));
                    lis.Add(getREL("f29PortalQuestionTab", "f19_f29", "PORTAL box", "LEFT OUTER JOIN f29PortalQuestionTab f19_f29 ON a.f29ID=f19_f29.f29ID"));
                    lis.Add(getREL("f44QuestionCluster", "f19_f44", "Klastr", "LEFT OUTER JOIN f44QuestionCluster f19_f44 ON a.f44ID=f19_f44.f44ID"));
                    break;
                case "f21":
                    lis.Add(getREL("f22ReplySet", "f21_f22", "Šablony odpovědí", "LEFT OUTER JOIN (select xa.f21ID,xb.* from f43ReplyUnitToSet xa INNER JOIN f22ReplySet xb ON xa.f22ID=xb.f22ID) f21_f22 ON a.f21ID=f21_f22.f21ID"));
                    lis.Add(getREL("f19Question", "f21_f19", "Otázka", "LEFT OUTER JOIN f20ReplyUnitToQuestion f21_f20 ON a.f21ID=f21_f20.f21ID LEFT OUTER JOIN f19Question f21_f19 ON f21_f20.f19ID=f21_f19.f19ID"));
                    lis.Add(getREL("f18FormSegment", "f19_f18", "Segment formuláře", "LEFT OUTER JOIN f18FormSegment f19_f18 ON f21_f19.f18ID=f19_f18.f18ID", "f21_f19"));
                    break;
                case "h04":
                    lis.Add(getREL("h07ToDoType", "h04_h07", "Typ úkolu", "INNER JOIN h07ToDoType h04_h07 ON a.h07ID=h04_h07.h07ID"));
                    lis.Add(getREL("h05ToDoStatus", "h04_h05", "Stav úkolu", "LEFT OUTER JOIN h05ToDoStatus h04_h05 ON a.h05ID=h04_h05.h05ID"));
                    lis.Add(getREL("a01Event", "h04_a01", "Akce", "INNER JOIN a01Event h04_a01 ON a.a01ID=h04_a01.a01ID"));
                    lis.Add(getREL("j02Person", "h04_owner", "Vlastník záznamu", getOwnerSql("h04")));
                    break;
                case "h07":
                    lis.Add(getREL("b65WorkflowMessage", "h07_b65", "Notifikační zpráva", "LEFT OUTER JOIN b65WorkflowMessage h07_b65 ON a.b65ID=h07_b65.b65ID"));
                    break;
                case "a37":
                    lis.Add(getREL("a17DepartmentType", "a37_a17", "Typ činnosti", "INNER JOIN a17DepartmentType a37_a17 ON a.a17ID=a37_a17.a17ID"));
                    lis.Add(getREL("a03Institution", "a37_a03", "Škola", "INNER JOIN a03Institution a37_a03 ON a.a03ID=a37_a03.a03ID"));

                    break;
                case "k01":
                    lis.Add(getREL("a03Institution", "k01_a03", "Škola", "INNER JOIN k02TeacherSchool k01_k02 ON a.k01ID=k01_k02.k01ID INNER JOIN a03Institution k01_a03 ON k01_k02.a03ID=k01_a03.a03ID"));

                    break;
                case "h11":
                    lis.Add(getREL("j02Person", "h11_j02", "Autor", "INNER JOIN j02Person h11_j02 ON a.j02ID_Creator=h11_j02.j02ID"));
                    break;
                case "a39":
                    lis.Add(getREL("j02Person", "a39_j02", "Lidé", "INNER JOIN j02Person a39_j02 ON a.j02ID=a39_j02.j02ID"));
                    lis.Add(getREL("j04UserRole", "a39_j04", "Role", "LEFT OUTER JOIN j04UserRole a39_j04 ON a.j04ID_Explicit=a39_j04.j04ID"));
                    lis.Add(getREL("a03Institution", "a39_a03", "Škola", "INNER JOIN a03Institution a39_a03 ON a.a03ID=a39_a03.a03ID"));
                    break;

                case "o13":
                    lis.Add(getREL("x29Entity", "o13_x29", "Entita", "LEFT OUTER JOIN x29Entity o13_x29 ON a.x29ID=o13_x29.x29ID"));
                    break;

                //case "o23":
                //    lis.Add(getREL("b02Status", "o23_b02", "Workflow stav", "LEFT OUTER JOIN b02Status o23_b02 ON a.b02ID = o23_b02.b02ID"));

                //    lis.Add(getREL("j02Person", "o23_owner", "Vlastník záznamu", getOwnerSql("o23")));
                //    lis.Add(getREL("o54TagBindingInline", "o23_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='o23') o23_o54 ON a.o23ID=o23_o54.o54RecordPid"));
                //    break;
                case "j40":
                    lis.Add(getREL("j02Person", "j40_owner", "Vlastník záznamu", getOwnerSql("j40")));
                    break;
                case "j23":
                    lis.Add(getREL("a05Region", "j23_a05", "Kraj", "LEFT OUTER JOIN a05Region j23_a05 ON a.a05ID=j23_a05.a05ID"));
                    lis.Add(getREL("j24NonPersonType", "j23_j24", "Typ zdroje", "LEFT OUTER JOIN j24NonPersonType j23_j24 ON a.j24ID=j23_j24.j24ID"));
                    break;
                case "o51":
                    lis.Add(getREL("o53TagGroup", "o51_o53", "Kategorie", "LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID"));
                    break;
                case "x40":
                    lis.Add(getREL("a01Event", "x40_a01", "Akce", "LEFT OUTER JOIN a01Event x40_a01 ON (a.x40DataPID=x40_a01.a01ID AND a.x29ID=101)"));
                    lis.Add(getREL("h04ToDo", "x40_h04", "Úkol/Lhůta", "LEFT OUTER JOIN h04ToDo x40_h04 ON (a.x40DataPID=x40_h04.h04ID AND a.x29ID=604)"));
                    break;
                case "x31":
                case "z01":
                    lis.Add(getREL("x29Entity", "x31_x29", "Entita", "LEFT OUTER JOIN x29Entity x31_x29 ON a.x29ID=x31_x29.x29ID"));
                    break;
                //lis.Add(getREL("j02Person", "o51_owner", "Vlastník záznamu", getOwnerSql("o51")));

                default:
                    break;
            }

            return lis;
        }

        private string getOwnerSql(string prefix)
        {
            return string.Format("LEFT OUTER JOIN j02Person {0}_owner ON a.j02ID_Owner = {0}_owner.j02ID", prefix);
        }

    }
}
