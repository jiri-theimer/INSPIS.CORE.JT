using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BL
{
    public class TheColumnsProvider
    {
        private readonly BL.RunningApp _app;
        private readonly BL.TheEntitiesProvider _ep;
        private readonly BL.TheTranslator _tt;
        private List<BO.TheGridColumn> _lis;        
        private string _lastEntity;
        private string _curEntityAlias;
        
        public TheColumnsProvider(BL.RunningApp runningapp,BL.TheEntitiesProvider ep,BL.TheTranslator tt)
        {
            _app = runningapp;
            _ep = ep;
            _tt = tt;
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_DbOpers();

        }

        public void Refresh()
        {
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
            Handle_DbOpers();
        }

        private int SetDefaultColWidth(string strFieldType)
        {
            switch (strFieldType)
            {
                case "date":
                    return 90;                    
                case "datetime":
                    return 120;                   
                case "num":
                case "num4":
                case "num5":
                case "num3":
                    return 100;                    
                case "num0":
                    return 75;
                case "bool":
                    return 75;
                default:
                    return 0;
            }
            
        }

        private BO.TheGridColumn AF(string strEntity, string strField, string strHeader, int intDefaultFlag = 0, string strSqlSyntax = null, string strFieldType = "string", bool bolIsShowTotals = false,bool bolNotShowRelInHeader=false)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = _ep.ByTable(strEntity).AliasSingular;
            }
           
            _lis.Add(new BO.TheGridColumn() { Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, DefaultColumnFlag = intDefaultFlag, SqlSyntax = strSqlSyntax, FieldType = strFieldType, IsShowTotals = bolIsShowTotals,NotShowRelInHeader= bolNotShowRelInHeader,FixedWidth= SetDefaultColWidth(strFieldType),TranslateLang1=strHeader,TranslateLang2=strHeader,TranslateLang3=strHeader });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }

        


        private void AF_TIMESTAMP(string strEntity, string strField, string strHeader, string strSqlSyntax, string strFieldType)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = _ep.ByTable(strEntity).AliasSingular;
            }
            _lis.Add(new BO.TheGridColumn() { IsTimestamp = true, Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, SqlSyntax = strSqlSyntax, FieldType = strFieldType, FixedWidth = SetDefaultColWidth(strFieldType) });
            _lastEntity = strEntity;
        }

        private void AppendTimestamp(string strEntity,bool include_validity =true)
        {
            string prefix = strEntity.Substring(0, 3);
            AF_TIMESTAMP(strEntity, "DateInsert_" + strEntity, "Založeno", "a."+ prefix+"DateInsert", "datetime");
            AF_TIMESTAMP(strEntity, "UserInsert_" + strEntity, "Založil", "a."+ prefix+"UserInsert", "string");
            AF_TIMESTAMP(strEntity, "DateUpdate_" + strEntity, "Aktualizace", "a."+ prefix+"DateUpdate", "datetime");
            AF_TIMESTAMP(strEntity, "UserUpdate_" + strEntity, "Aktualizoval", "a."+ prefix+"UserUpdate", "string");
            if (include_validity == true)
            {
                AF_TIMESTAMP(strEntity, "ValidFrom_" + strEntity, "Platné od", "a." + prefix + "ValidFrom", "datetime");
                AF_TIMESTAMP(strEntity, "ValidUntil_" + strEntity, "Platné do", "a." + prefix + "ValidUntil", "datetime");

                AF_TIMESTAMP(strEntity, "IsValid_" + strEntity, "Časově platné", string.Format("convert(bit,case when GETDATE() between a.{0}ValidFrom AND a.{0}ValidUntil then 1 else 0 end)", prefix), "bool");
            }
            
        }

        private void Handle_DbOpers()
        {
            //Překlad do ostatních jazyků: Musí být před načtením kategorií, aby se názvy kategorií nepřekládali!
            foreach (var col in _lis)
            {
                bool b = true;
                if (col.Header.Length > 3 && col.Header.Substring(0, 3) == "Col")
                {
                    b = false;
                }
                if (b)
                {
                    col.TranslateLang1 = _tt.DoTranslate(col.Header, 1);
                    col.TranslateLang2 = _tt.DoTranslate(col.Header, 2);
                }

            }

            BO.TheGridColumn onecol;
            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            var dt = db.GetDataTable("select * from o53TagGroup WHERE o53Field IS NOT NULL AND o53Entities IS NOT NULL ORDER BY o53Ordinary");
            foreach (System.Data.DataRow dbrow in dt.Rows)
            {
               
                onecol = AF("o54TagBindingInline", dbrow["o53Field"].ToString(), dbrow["o53Name"].ToString(), 0, null, "string", false, true);
                onecol.VisibleWithinEntityOnly = dbrow["o53Entities"].ToString();
            }
            
            
        }
        private void SetupPallete()
        {
            BO.TheGridColumn onecol;

            //a01Event = akce
            
            
            if (_app.Implementation == "HD")
            {
                onecol = AF("a01Event", "a01Signature", "ID požadavku", 1, null, "string", false, true);
                onecol.FixedWidth = 100;
                AF("a01Event", "a01InstitutionPlainText", "Zadavatel", 1);
                AF("a01Event", "a01LeaderInLine", "Vedoucí");
                AF("a01Event", "a01MemberInLine", "Členové");
                AF("a01Event", "a01DateFrom", "Plán od", 0, null, "date");
                AF("a01Event", "a01DateUntil", "Plán do", 0, null, "date");
            }
            else
            {
                onecol = AF("a01Event", "a01Signature", "Signatura", 1, null, "string", false, true);
                onecol.FixedWidth = 100;
                AF("a01Event", "a01LeaderInLine", "Vedoucí", 1);
                AF("a01Event", "a01MemberInLine", "Členové", 2);
                AF("a01Event", "a01DateFrom", "Plán od", 1, null, "date");
                AF("a01Event", "a01DateUntil", "Plán do", 2, null, "date");
            }
            AF("a01Event", "a01CaseCode", "Spisová značka");
            AF("a01Event", "Uzavreno", "Uzavřeno", 0, "case when a.a01IsClosed=1 or a.a01ValidUntil<GETDATE() then 1 else 0 end", "bool",false,true);
            AF("a01Event", "a01DateClosed", "Datum uzavření", 0, null, "datetime");
            
            AF("a01Event", "a01IsAllFormsClosed", "Formuláře uzavřeny", 0, null, "bool",false,true);
            AF("a01Event", "a01Name", "Název");
            AF("a01Event", "a01Description", "Popis");
            AF("a01Event", "a01IsTemporary", "Dočasná akce", 0, null, "bool");
            AF("a01Event", "SLA", "SLA (hod.)", 0, "case when a01DurationSLA<3600 then 0 else a01DurationSLA / 60 / 60 end", "num");
            AppendTimestamp("a01Event");

            //a03Institution = instituce
            onecol=AF("a03Institution", "a03REDIZO", "REDIZO",1,null,"string",false,true);
            onecol.FixedWidth = 100;
            AF("a03Institution", "a03Name", "Instituce", 1, null, "string", false, false);
            AF("a03Institution", "a03ShortName", "Zkrácený název", 0);
            AF("a03Institution", "NazevPlusRedizo", "Název+REDIZO", 0, "a.a03Name+'['+a.a03REDIZO+']'");
            onecol=AF("a03Institution", "a03ICO", "IČ");
            AF("a03Institution", "Ares", "ARES url", 0, "case when a.a03ICO IS NOT NULL AND a.a06ID=1 then '<a class='+char(34)+'grid-link'+char(34)+' target='+char(34)+'_blank'+char(34)+' href='+char(34)+'https://wwwinfo.mfcr.cz/cgi-bin/ares/darv_sko.cgi?ico='+a.a03ICO+'&jazyk=cz&xml=1'+CHAR(34)+'>ares</a>' end");
            AF("a03Institution", "Justice", "Justice url", 0, "case when a.a03ICO IS NOT NULL then '<a class='+char(34)+'grid-link'+char(34)+' target='+char(34)+'_blank'+char(34)+' href='+char(34)+'https://or.justice.cz/ias/ui/rejstrik-$firma?ico='+a.a03ICO+CHAR(34)+'>justice</a>' end");

            onecol.FixedWidth = 100;

            AF("a03Institution", "a03Street", "Ulice", 1);
            AF("a03Institution", "a03City", "Obec", 1);
            onecol=AF("a03Institution", "a03PostCode", "PSČ");
            onecol.FixedWidth = 70;
            AF("a03Institution", "FullAddress","Adresa",0, "isnull(a.a03Street+', ','')+isnull(a.a03PostCode+' ','')+isnull(a.a03City,'')");
            AF("a03Institution", "Mapy", "Mapy.cz url", 0, "'<a class='+char(34)+'grid-link'+char(34)+' target='+char(34)+'_blank'+char(34)+' href='+char(34)+'https://mapy.cz/zakladni?q='+a.a03City+isnull('+'+a.a03Street,'')+CHAR(34)+'>mapy</a>'");
            AF("a03Institution", "GMaps", "Google Maps url", 0, "'<a class='+char(34)+'grid-link'+char(34)+' target='+char(34)+'_blank'+char(34)+' href='+char(34)+'https://www.google.com/maps/place/'+a.a03City+isnull('+'+a.a03Street,'')+CHAR(34)+'>google</a>'");

            AF("a03Institution", "a03Email", "E-mail");
            AF("a03Institution", "a03Web", "WWW");
            AF("a03Institution", "a03Phone", "TEL");
            AF("a03Institution", "a03Mobile", "Mobil");
            AF("a03Institution", "a03Fax", "FAX");                      

            AF("a03Institution", "a03DirectorFullName", "Ředitel (textově)");
            AF("a03Institution", "a03IsTestRecord", "Testovací záznam", 0, null, "bool");
            AF("a03Institution","a29Names", "Pojmenované seznamy", 0, "dbo._core_a03_get_a29name_inline(a.a03ID)");
            onecol=AF("a03Institution", "a03ParentFlag", "Úroveň", 0, "case a.a03ParentFlag when 1 then '1' when 2 then '2' end");
            onecol.FixedWidth = 100;
            AppendTimestamp("a03Institution");

            //a02 = osoba v inspektorátu            
            AppendTimestamp("a02Inspector");

            //a04 = inspektorát
            AF("a04Inspectorate", "PostAddress", "Adresa", 1, "isnull(a.a04Street,'')+isnull(', '+a.a04PostCode,'')+isnull(' '+a.a04City,'')", "string",false,true);
            AF("a04Inspectorate", "a04Name", "Inspektorát",1,null,"string",false,true);

            AF("a04Inspectorate", "a04City", "Město");
            AF("a04Inspectorate", "a04Street", "Ulice");
            AF("a04Inspectorate", "a04PostCode", "PSČ");

            AF("a04Inspectorate", "a04IsRegional", "Krajský", 2, null, "bool");
            AF("a04Inspectorate", "a04Email", "E-mail");
            AF("a04Inspectorate", "a04Phone", "TEL",2);
            AF("a04Inspectorate", "a04Mobile", "Mobil");
            AF("a04Inspectorate", "a04Fax", "FAX");            
            AppendTimestamp("a04Inspectorate");

            //a05 = regiony
            AF("a05Region", "a05Name", "Kraj", 1, null, "string", false, true);            
            onecol=AF("a05Region", "a05UIVCode", "UIV kód", 2);
            onecol.FixedWidth = 80;
            AF("a05Region", "a05RZCode", "RZ kód");
            AF("a05Region", "a05VUSCCode", "VUSC kód");
            AF("a05Region", "a05RZCode", "RZ kód",2);
            AF("a05Region", "a05Ordinal", "#", 2, null, "num0");
            AppendTimestamp("a05Region");

            //a06 = typy institucí
            onecol=AF("a06InstitutionType", "a06Name", "Typ instituce", 1,null,"string",false,true);
            onecol.FixedWidth = 120;
            AppendTimestamp("a06InstitutionType");

            //a06 = typy institucí
            AF("a21InstitutionLegalType", "a21Name", "Právní forma", 1, null, "string", false, true);
            AF("a21InstitutionLegalType", "a21Code", "Kód", 2);



            //a08 = téma akce
            AF("a08Theme", "a08Name", "Téma akce", 1,null,"string",false,true);
            AF("a08Theme", "a08Description", "Popis");
            AppendTimestamp("a08Theme");

            //a09 = typ zřizovatele
            AF("a09FounderType", "a09Name", "Typ zřizovatele", 1, null, "string", false, true);
            AF("a09FounderType", "a09UIVCode", "UIV", 2);
            AF("a09FounderType", "a09Description", "Popis");
            AF("a09FounderType", "a09Ordinal", "#", 0, null, "num0");
            AppendTimestamp("a09FounderType");

            //a10 = typ akce
            AF("a10EventType", "a10Name", "Typ akce", 1, null, "string", false, true);
            AF("a10EventType", "a10Ident", "Kód",1);
            AF("a10EventType", "a10ViewUrl_Insert", "URL [INSERT]", 2);
            AF("a10EventType", "a10ViewUrl_Page", "URL [DETAIL]", 2);
            AF("a10EventType", "a10Description", "Popis");
            AF("a10EventType", "a10CoreFlag", "CORE Flag");
            AppendTimestamp("a10EventType");

            //a11 = formulář v akci
            AF("a11EventForm", "a11AccessToken", "PIN",2);
            AF("a11EventForm", "a11Description", "Poznámka", 2);
            AF("a11EventForm", "a11IsLocked", "Uzamknuto",1,null,"bool");
            AF("a11EventForm", "a11IsPoll", "Anketní", 0, null, "bool");
            AF("a11EventForm", "a11ProcessingStart", "První odpověď", 1, null, "datetime");
            AF("a11EventForm", "a11ProcessingLast", "Poslední odpověď", 1, null, "datetime");
            AppendTimestamp("a11EventForm");

            //a25 = skupina formulářů v rámci akce
            AF("a25EventFormGroup","a25Name","Skupina formulářů",1, null, "string", false, true);            
            AF("a25EventFormGroup", "a25Color", "🚩", 1, "case when a.a25Color IS NOT NULL then '<div style='+char(34)+'background-color:'+a.a25Color+';'+char(34)+'>&nbsp;</div>' end");
            AppendTimestamp("a25EventFormGroup");

            //a37 = typy činností školy
            AF("a37InstitutionDepartment", "a37Name", "Název činnosti", 1, null, "string", false, true);
            AF("a37InstitutionDepartment", "a37IZO", "IZO", 1, null, "string", false, true);
            AF("a37InstitutionDepartment", "Adresa", "Adresa",0, "isnull(a.a37Street + ', ', '') + isnull(a.a37PostCode + ' ', '') + isnull(a.a37City, '')");
            AF("a37InstitutionDepartment", "a37City", "Obec",1);
            AF("a37InstitutionDepartment", "a37Street", "Ulice",1);
            onecol=AF("a37InstitutionDepartment", "a37PostCode", "PSČ",1);
            onecol.FixedWidth = 80;
            AF("a37InstitutionDepartment", "a37Mapy", "Mapy.cz url", 0, "'<a class='+char(34)+'grid-link'+char(34)+' target='+char(34)+'_blank'+char(34)+' href='+char(34)+'https://mapy.cz/zakladni?q='+a.a37City+isnull('+'+a.a37Street,'')+CHAR(34)+'>mapy</a>'");
            AF("a37InstitutionDepartment", "a37GMaps", "Google Maps url", 0, "'<a class='+char(34)+'grid-link'+char(34)+' target='+char(34)+'_blank'+char(34)+' href='+char(34)+'https://www.google.com/maps/place/'+a.a37City+isnull('+'+a.a37Street,'')+CHAR(34)+'>google</a>'");

            AF("a37InstitutionDepartment", "a37Email", "E-mail");
            AF("a37InstitutionDepartment", "a37Web", "WWW");
            AF("a37InstitutionDepartment", "a37Phone", "TEL");
            AF("a37InstitutionDepartment", "a37Mobile", "Mobil");
            AF("a37InstitutionDepartment", "a37Fax", "FAX");
            AppendTimestamp("a37InstitutionDepartment");

            AF("a42Qes", "a42Name", "INEZ", 1, null, "string", false, true);
            AF("a42Qes", "a42JobState", "Stav generování", 1, "case a.a42JobState when 1 then 'Úvodní návrh' when 2 then 'Generují se akce' when 3 then 'Pozastaveno generování akcí' when 4 then 'Akce vygenerovány' when 5 then 'Poštovní zprávy vygenerovány' when 6 then 'Rozesílání zpráv' when 7 then 'Rozesílání zpráv zastaveno' when 99 then 'Hotovo' end");
            AF("a42Qes", "a42DateFrom", "Datum od", 1, null, "date");
            AF("a42Qes", "a42DateUntil", "Datum do", 1, null, "date");
            AF("a42Qes", "a42Description", "Poznámka", 0);
            AppendTimestamp("a42Qes");


            //j02 = osoby
            AF("j02Person", "fullname_desc", "Příjmení+Jméno", 1, "a.j02LastName+' '+a.j02FirstName+isnull(' '+a.j02TitleBeforeName,'')", "string",false,true);
            AF("j02Person", "fullname_asc", "Jméno+Příjmení", 0, "isnull(a.j02TitleBeforeName+' ','')+a.j02FirstName+' '+a.j02LastName+isnull(' '+a.j02TitleAfterName,'')", "string", false, true);
            onecol=AF("j02Person", "j02PID", "Personální číslo", 2,null, "string", false, true);
            onecol.FixedWidth = 100;                       
            AF("j02Person", "j02Email", "E-mail", 1);
            AF("j02Person", "j02FirstName", "Jméno");
            AF("j02Person", "j02LastName", "Příjmení");
            AF("j02Person", "j02TitleBeforeName", "Titul před");
            AF("j02Person", "j02TitleAfterName", "Titul za");
            AF("j02Person", "j02Phone", "TEL");
            AF("j02Person", "j02Mobile", "Mobil");
            AF("j02Person", "j02Address", "Adresa bydliště");
            AF("j02Person", "j02Position", "Pracovní funkce");
            AF("j02Person", "j02IsInvitedPerson", "Přizvaná osoba", 0, null, "bool");
            AppendTimestamp("j02Person");

            //j03 = uživatelé
            AF("j03User", "j03Login", "Login", 1,null,"string",false,true);
            AF("j03User", "j04Name", "Role", 1, "j03_j04.j04Name","string",false,true);
            AF("j03User", "Lang", "Jazyk", 1, "case isnull(a.j03LangIndex,0) when 0 then 'Česky' when 1 then 'English' when 2 then convert(nvarchar(20),N'Українська') end");
            AF("j03User", "j03PingTimestamp", "Last ping", 0, "a.j03PingTimestamp", "datetime");
            AF("j03User", "j03IsDebugLog", "Debug log", 0, null, "bool");
            AppendTimestamp("j03User");

            //j04=aplikační role
            AF("j04UserRole", "j04Name", "Aplikační role", 1,null,"string",false,true);
            if (_app.Implementation == "UA")
            {
                AF("j04UserRole", "RelationFlag", "Vztah", 1, "case a.j04RelationFlag when 1 then N'Без обмежень/Bez omezení' when 2 then N'Школу/Instituce' when 3 then N'Інспекція/Inspektorát' end");
                AF("j04UserRole", "PortalFaceFlag", "Portál", 0, "case a.j04PortalFaceFlag when 1 then 'ČŠI' when 2 then 'Школу/Škola' when 3 then 'Засновник/Zřizovatel' when 4 then 'Громадський/Veřejnost' end");
                
            }
            else
            {
                AF("j04UserRole", "RelationFlag", "Vztah", 1, "case a.j04RelationFlag when 1 then 'Bez omezení' when 2 then 'Instituce' when 3 then 'Inspektorát' end");
                AF("j04UserRole", "PortalFaceFlag", "Portál", 0, "case a.j04PortalFaceFlag when 1 then 'ČŠI' when 2 then 'Škola' when 3 then 'Zřizovatel' when 4 then 'Veřejnost' end");
                
            }


            AF("j04UserRole", "j04IsAllowedAllEventTypes", "Všechny typy akcí", 0, null, "bool");

            AF("j04UserRole", "j04Description", "Popis");
            AF("j04UserRole", "j04ViewUrl_Page", "URL výchozí stránky");
            AppendTimestamp("j04UserRole");

            //j11 = týmy osob
            AF("j11Team", "j11Name", "Tým osob", 1, null, "string", false, true);           
            AppendTimestamp("j11Team");

            //a17 = typy činností školy
            AF("a17DepartmentType", "a17Name", "Typ činnosti", 1,null,"string",false,true);
            AF("a17DepartmentType", "a17UIVCode", "UIV", 1,null,"string",false,true);
            AF("a17DepartmentType", "a17Description", "Popis", 1);            
            AppendTimestamp("a17DepartmentType");

            //a18 = kódy vzdělávacích oborů
            AF("a18DepartmentDomain", "a18Name", "Název oboru", 1, null, "string", false, true);
            AF("a18DepartmentDomain", "a18Code", "Kód oboru", 1, null, "string", false, true);
            AF("a18DepartmentDomain", "a18SubCode1", "Kategorie",1);
            AF("a18DepartmentDomain", "Skupina", "Skupina",1, "case when a.a18SubCode2 is not null then a.a18SubCode1+a.a18SubCode2 end");
            AF("a18DepartmentDomain", "Obor", "Obor", 1, "case when LEN(a.a18Code)>=5 then LEFT(a.a18Code,5) end");           
            AppendTimestamp("a18DepartmentDomain");

            //a19 = Vzdělávací obory školy            
            AF("a19DomainToInstitutionDepartment", "a19StudyPlatform", "Forma vzdělávání", 1, null, "string", false, true);
            AF("a19DomainToInstitutionDepartment", "a19StudyDuration", "Délka vzdělávání", 1, null, "string", false, true);
            AF("a19DomainToInstitutionDepartment", "a19IsShallEnd", "Dobíhající", 2, null, "bool");

            //a45 = role v akci
            AF("a45EventRole", "a45Name", "Role v akci", 1, null, "string", false, true);
            


            //o51 = položka kategorie
            AF("o51Tag", "o51Name", "Položka kategorie", 1, null, "string", false, true);            
            AF("o51Tag", "o51IsColor", "Má barvu", 2, null, "bool");
            AF("o51Tag", "o51ForeColor", "Barva písma",2, "'<div style=\"background-color:'+a.o51ForeColor+';\">písmo</div>'");
            AF("o51Tag", "o51BackColor", "Barva pozadí", 2, "'<div style=\"background-color:'+a.o51BackColor+';\">pozadí</div>'");
            AF("o51Tag", "o51Ordinary", "Pořadí", 0, null, "num0");


            //o53 = kategorie (skupiny položek kategorie)
            AF("o53TagGroup", "o53Name", "Název kategorie", 1, null, "string", false, true);            
            AF("o53TagGroup", "o53IsMultiSelect", "Multi-Select",0,null,"bool");
            //AF("o53TagGroup", "o53Entities", "Vazby", 1, "dbo.getEntityAlias_Multi(a.o53Entities)");
            AF("o53TagGroup", "o53Ordinary", "Pořadí", 0, null, "num0");

            AF("o54TagBindingInline", "o54InlineHtml", "Kategorie", 1, null, "string", false, true);
            AF("o54TagBindingInline", "o54InlineText", "Kategorie (pouze text)",1, null, "string", false, true);


            //zatím provizorně v rámci SINGLETON režimu této třídy:
            //DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            //var dt = db.GetDataTable("select * from o53TagGroup WHERE o53Field IS NOT NULL AND o53Entities IS NOT NULL ORDER BY o53Ordinary");
            //foreach(System.Data.DataRow dbrow in dt.Rows)
            //{                
            //   onecol= AF("o54TagBindingInline", dbrow["o53Field"].ToString(), dbrow["o53Name"].ToString(), 0, null, "string", false, true);
            //   onecol.VisibleWithinEntityOnly = dbrow["o53Entities"].ToString();
            //}


            //a28 = Typy škol
            AF("a28SchoolType", "a28Name", "Typ školy", 1, null, "string", false, true);
            AF("a28SchoolType", "a28Code", "Kód", 2);
            AF("a28SchoolType", "a28Ordinary", "Pořadí", 2, null, "num0");            
            AppendTimestamp("a28SchoolType");


            //a70 = Školní IS
            AF("a70SIS", "a70Name", "Školní IS", 1,null,"string",false,true);
            AF("a70SIS", "a70Code", "Kód", 1);
            AF("a70SIS", "Rozsah", "Rozsah integrace", 1, "case a.a70ScopFlag when 2 then 'Pouze reálné školy' when 1 then 'Pouze testovací školy' when 3 then 'Všechny školy' when 4 then 'Obecně integrovaný' end");
            AF("a70SIS", "a70Description", "Popis");
            AppendTimestamp("a70SIS");

            //a29 = pojmenovaný seznam institucí
            AF("a29InstitutionList", "a29Name", "Pojmenovaný seznam", 1, null, "string", false, true);            
            AF("a29InstitutionList", "a29Description", "Poznámka");
            AppendTimestamp("a29InstitutionList");            

            //x27 = eval funkce
            AF("x27EvalFunction", "x27Name", "Název funkce", 1,null,"string",false,true);
            AF("x27EvalFunction", "x27Returns", "Vrací",1);
            AF("x27EvalFunction", "x27Description", "Popis", 1);
            AF("x27EvalFunction", "x27Parameters", "Parametry");
            AF("x27EvalFunction", "x27Ordinal", "#", 0, null, "num0");
            AppendTimestamp("x27EvalFunction");


            //f06 = formulář
            AF("f06Form", "f06Name", "Formulář", 1, null, "string", false, true);
            AF("f06Form", "f06IsTemplate", "Pro šablony otázek", 0, null, "bool");
            AF("f06Form", "f06IsExportToDoc", "Export do DOC", 0, null, "bool");
            AF("f06Form", "f06IsReportDialog", "Tisková sestava", 0, null, "bool");
            AF("f06Form", "f06IsA37Required", "Povinné IZO", 0, null, "bool");
            AF("f06Form", "f06IsA01PeriodStrict", "Vyplňovat pouze v plán období", 0, null, "bool");
            AF("f06Form", "f06IsA01ClosedStrict", "Nelze vyplňovat pokud akce uzavřena", 0, null, "bool");
            AF("f06Form", "f06IsWorkflowDialog", "Nabízet [Posunout/doplnit]", 0, null, "bool");
            AF("f06Form", "f06Ordinal", "#", 2, null, "num0");
            AF("f06Form", "f06ExportCode", "STAT kód");
            AF("f06Form", "f06Description", "Popis");
            AF("f06Form", "f06BindScopeQuery", "Ankety", 0, "case ISNULL(a.f06BindScopeQuery,0) when 0 then 'Ne-anketní i anketní' when 1 then 'Pouze ne-anketní' when 2 then 'Pouze anketní' end");
            AF("f06Form", "f06UserLockFlag", "Zamykání", 0, "case a.f06UserLockFlag when 1 then 'Uživatel nemá možnost' when 2 then 'Ano, pokud je korektně vyplněn' when 3 then 'Kdykoliv' end");
            
            AppendTimestamp("f06Form");

            //a39 = kontaktní osoby
            AF("a39InstitutionPerson", "a39IsAllowInspisWS", "Přístup k IS", 0, null, "bool",false,true);
            AF("a39InstitutionPerson", "a39Description", "Poznámka", 2);
            AF("a39InstitutionPerson", "RelationFlag", "Vztah", 2, "case a.a39RelationFlag when 2 then 'Zaměstnanec' else 'Kontaktní osoba' end");

            //f12 = typ formuláře            
            AF("f12FormType", "TreeItem", "Typ formuláře", 1, "case when a.f12TreeLevel > 1 then replace(space(2 * (a.f12TreeLevel - 1)), ' ', '-') else '' END + a.f12Name", "string", false, true);
            AF("f12FormType", "f12Name", "Název", 2);            
            AF("f12FormType", "ParentPath", "Nadřízený", 1, "dbo._core_f12_get_parent_inline(a.f12ID)");
            AF("f12FormType", "f12Hint", "Úvodní instrukce");
            AF("f12FormType", "f06IsExportToDoc", "Export do DOC");
            AF("f12FormType", "f06IsWorkflowDialog", "Workflow dialog");
            AF("f12FormType", "f06IsReportDialog", "Tisk dialog");
            AF("f12FormType", "f06IsTemplate", "Vzorový formulář");
            AppendTimestamp("f12FormType");

            //f18 = segment formuláře            
            AF("f18FormSegment", "TreeItem", "Segment formuláře", 1, "case when a.f18TreeLevel > 1 then replace(space(2 * (a.f18TreeLevel - 1)), ' ', '-') else '' END + a.f18Name", "string", false, true);
            AF("f18FormSegment", "f18Name", "Název", 2);
            AF("f18FormSegment", "f18RazorTemplate", "Razor šablona");
            AF("f18FormSegment", "ParentPath", "Nadřízený", 1, "dbo._core_f18_get_parent_inline(a.f18ID)");
            AF("f18FormSegment", "f18Text", "Hlavní text");
            AF("f18FormSegment", "f18Ordinal", "#",2,null,"num0");            
            AppendTimestamp("f18FormSegment");

            //f19 = formulářové otázky
            AF("f19Question", "f19Name", "Otázka", 1, null, "string", false, true);
            AF("f19Question", "f19SupportingText", "Rozšířený text");
            
            AF("f19Question", "f19Ident", "Kód");
            AF("f19Question", "f19StatID", "STAT ID");
            AF("f19Question", "f19DefaultValue", "Výchozí hodnota");
            AF("f19Question", "f19ReadonlyExpression", "READONLY podmínka");
            AF("f19Question", "f19SkipExpression", "SKIP podmínka");
            AF("f19Question", "f19RequiredExpression", "REQUIRED podmínka");
            AF("f19Question", "f19CancelValidateExpression", "CANCEL podmínka");
            AF("f19Question", "f19CancelValidateExpression_Message", "Vlastní chybové hlášení");

            AF("f19Question", "f19PortalPublishFlag", "Vztah k portálu", 0, "case a.f19PortalPublishFlag when 0 then 'Nepublikovat' when 1 then 'Publikovat se souhlasem školy' when 2 then 'Publikovat vždy' end", "string", false, true);
            
            AF("f19Question", "f19CHLMaxAnswers", "Max.počet odpovědí", 0, null, "num0");

            AF("f19Question", "f19Ordinal", "#", 0, null, "num0");
            AF("f19Question", "f19IsMultiselect", "Multiselect", 0, null, "bool");
            AF("f19Question", "f19IsRequired", "Povinná", 0, null, "bool");
            AF("f19Question", "f19IsTextboxMultiline", "Multiline", 0, null, "bool");
            AF("f19Question", "f19IsHorizontalDisplay", "Horizontální směr odpovědi", 0, null, "bool");
            AF("f19Question", "f19LinkURL", "URL tlačítka");
            AF("f19Question", "f19MaxUploadFiles", "Max.počet příloh", 0, null, "num0");
            AF("f19Question", "f19AllowedFileUploadExtensions", "Povolené souborové přípony");
            AF("f19Question", "f19Regex", "Regulární výraz");
            AppendTimestamp("f19Question");

            //f21 = jednotky odpovědi
            AF("f21ReplyUnit", "f21Name", "Jednotka odpovědi", 1, null, "string", false, true);
            //AF("f21ReplyUnit", "f22Names", "Šablony jednotek", 2, "dbo._core_f21_get_f22names_inline(a.f21ID)");
            AF("f21ReplyUnit", "f21ExportValue", "STAT ID");
            AF("f21ReplyUnit", "f21Description", "Popis");
            AF("f21ReplyUnit", "f21IsCommentAllowed", "PKO", 0, null, "bool");
            AF("f21ReplyUnit", "f21IsNegation", "Negovat ostatní jednotky", 0, null, "bool");
            AF("f21ReplyUnit", "f21Ordinal", "#", 1, null, "num0");
            AF("f21ReplyUnit", "RecPid", "PID", 1, "Convert(varchar(10),a.f21ID)");

            AppendTimestamp("f21ReplyUnit");

            //f22 = šablony jednotek odpovědi
            AF("f22ReplySet", "f22Name", "Šablona jednotek", 1, null, "string", false, true);
            AF("f22ReplySet", "f21Names", "Jednotky v šabloně", 1, "dbo._core_f22_get_f21names_inline(a.f22ID)");
            AF("f22ReplySet", "f22Description", "Popis", 1);
            AF("f22ReplySet", "f22Ordinal", "#", 0, null, "num0");
            
            AppendTimestamp("f22ReplySet");

            //f23 = typy otázek
            AF("f23ReplyType", "f23Name", "Typ odpovědi", 1, null, "string", false, true);          
            AppendTimestamp("f23ReplyType");

            //f25 = šachovnice otázek
            AF("f25ChessBoard", "f25Name", "Šachovnice otázek", 1, null, "string", false, true);
            AF("f25ChessBoard", "f25ColumnCount", "Sloupce", 0, null, "num0");
            AF("f25ChessBoard", "f25ColumnHeaders", "Popisky sloupců");
            AF("f25ChessBoard", "f25RowCount", "Řádky", 0, null, "num0");
            AF("f25ChessBoard", "f25RowHeaders", "Popisky řádků");
            AppendTimestamp("f25ChessBoard");

            //f26 = baterie otázek
            AF("f26BatteryBoard", "f26Name", "Baterie otázek", 1, null, "string", false, true);
            AF("f26BatteryBoard", "f26Ordinal", "#", 0, null, "num0");
            AF("f26BatteryBoard", "f26ColumnHeaders", "Popisky sloupců");            
            AF("f26BatteryBoard", "f26SupportingText", "Rozšířený text");
            AppendTimestamp("f26BatteryBoard");

            //f29 = Portal boxy
            AF("f29PortalQuestionTab", "f29Name", "Portal box", 1,null,"string",false,true);
            AF("f29PortalQuestionTab", "f29Description", "Popis");
            AF("f29PortalQuestionTab", "f29Ordinal", "#", 0, null, "num0");
            AppendTimestamp("f29PortalQuestionTab");

            //f44 = klastr otázek
            AF("f44QuestionCluster", "f44Code", "Kód klastru");
            AF("f44QuestionCluster", "f44Name", "Klastr otázek", 1,null,"string",false,true);
            AF("f44QuestionCluster", "Questions", "Svázané otázky", 1, "dbo._core_f44_questions_inline(a.f44ID)");
            AF("f44QuestionCluster", "f44Ordinal", "#", 2, null, "num0");
            AppendTimestamp("f44QuestionCluster");

            //j24 = typ nepersonálního zdroje
            AF("j24NonPersonType", "j24Name", "Typ zdroje", 1,null,"string",false,true);
            AF("j24NonPersonType", "j24IsDriver", "Nabízet řidiče", 0,null,"bool");
            AppendTimestamp("j24NonPersonType");

            //j23 = Nepersonální zdroj
            AF("j23NonPerson", "j23Name", "Nepersonální zdroj", 1, null, "string", false, true);
            AF("j23NonPerson", "j23Code", "Kód",1);
            AF("j23NonPerson", "j23Description", "Popis");
            AppendTimestamp("j23NonPerson");
          
            //j25 = důvody rezervace
            AF("j25NonPersonPlanReason", "j25Name", "Důvod rezervace", 1);
            AppendTimestamp("j25NonPersonPlanReason");

            //j26 = dny svátků
            AF("j26Holiday", "j26Date", "Datum", 1, null, "date");
            AF("j26Holiday", "j26Name", "Název", 1);            
            AppendTimestamp("j26Holiday");

            //h07 = typ úkolu
            AF("h07ToDoType", "h07Name", "Typ úkolu", 1, null, "string", false, true);
            AF("h07ToDoType", "h07IsToDo", "Charakter úkolu", 1, null, "bool");
            AF("h07ToDoType", "h07IsDefault", "Výchozí typ", 0, null, "bool");
            AF("h07ToDoType", "h07IsCapacityPlan", "Vazba na kapacitní plán", 1, null, "bool");
            AF("h07ToDoType", "h07Description", "Popis");
            AppendTimestamp("h07ToDoType");


            //h04 = úkol
            AF("h04ToDo", "h04Name", "Úkol", 1, null, "string", false, true);
            onecol=AF("h04ToDo", "h04Signature", "ID úkolu", 1);
            onecol.FixedWidth = 100;
            AF("h04ToDo", "h04Deadline", "Termín", 1, null, "datetime");
            AF("h04ToDo", "h04ReminderDate", "Připomenutí", 2, null, "datetime");
            AF("h04ToDo", "h04CapacityPlanFrom", "Plán zahájení", 2, null, "datetime");
            AF("h04ToDo", "h04CapacityPlanUntil", "Plán dokončení", 2, null, "datetime");
            AF("h04ToDo", "h04Description", "Popis");
            AF("h04ToDo", "h04IsClosed", "Uzavřeno",0, null, "bool");
            AppendTimestamp("h04ToDo");

            //h05 = stav úkolu
            AF("h05ToDoStatus", "h05Name", "Stav úkolu", 1,null,"string",false,true);
            
            AF("h11NoticeBoard", "h11Name", "Článek", 1, null, "string", false, true);
            AF("h11NoticeBoard", "h11IsPublic", "Pro všechny", 1, null, "bool");
            AppendTimestamp("h11NoticeBoard");

            //b01 = workflow šablona            
            AF("b01WorkflowTemplate", "b01Name", "Workflow šablona", 1, null, "string", false, true);
            onecol=AF("b01WorkflowTemplate", "b01Ident", "Kód", 2);
            onecol.FixedWidth = 70;
            AppendTimestamp("b01WorkflowTemplate");

            //b02 = workflow stav            
            //AF("b02WorkflowStatus", "b02Name", "Stav", 1, null, "string", false, true);
            AF("b02WorkflowStatus", "b02Name", "Stav", 1);
            onecol =AF("b02WorkflowStatus", "b02Ident", "Kód stavu", 1);
            onecol.FixedWidth = 70;
            AF("b02WorkflowStatus", "b02IsDefaultStatus", "Výchozí stav", 2, null, "bool");
            AF("b02WorkflowStatus", "b02IsHoldStatus", "Záchytný stav", 2, null, "bool");
            AF("b02WorkflowStatus", "b02Order", "#", 2, null, "num0");
           
            AppendTimestamp("b02WorkflowStatus");




            //b05 = workflow historie          
            AF("b05Workflow_History", "Kdy", "Čas", 1, "a.b05DateInsert", "datetime");
            AF("b05Workflow_History", "b05Comment", "Text", 1, null, "string", false, true);
            
            AF("b05Workflow_History", "b05IsCommentOnly", "Pouze komentář", 2, null, "bool");
            AF("b05Workflow_History", "b05IsManualStep", "Ruční krok", 0, null, "bool");
            AF("b05Workflow_History", "b05IsNominee", "Nominace řešitele", 2, null, "bool");
            AF("b05Workflow_History", "b05IsCommentRestriction", "Interní komentář", 2, null, "bool");            
            AppendTimestamp("b05Workflow_History",false);

            //b06 = workflow krok
            AF("b06WorkflowStep", "b06Name", "Workflow krok", 1, null, "string", false, true);
            AF("b06WorkflowStep", "b06Order", "#", 2, null, "num0");
            AppendTimestamp("b06WorkflowStep");

            //b65 = notifikační šablona
            AF("b65WorkflowMessage", "b65Name", "Notifikační šablona", 1, null, "string", false, true);
            AF("b65WorkflowMessage", "b65MessageSubject", "Předmět zprávy", 2);
            AF("b65WorkflowMessage", "SystemFlag", "🚩", 1, "case when isnull(a.b65SystemFlag,0)>0 then '<div style='+char(34)+'background-color:red;'+char(34)+'>&nbsp;</div>' end");
            AppendTimestamp("b65WorkflowMessage");
            
            //b09 = workflow příkaz
            
            AF("b09WorkflowCommandCatalog", "b09Name", "Workflow příkaz", 1, null, "string", false, true);
            AF("b09WorkflowCommandCatalog", "b09Name_Lang2", "UA",1);
            AF("b09WorkflowCommandCatalog", "b09Hint", "?", 1);

            //j90 = access log uživatelů
            AF("j90LoginAccessLog", "j90Date", "Čas", 1, null, "datetime");
            AF("j90LoginAccessLog", "j90BrowserFamily", "Prohlížeč", 1);
            AF("j90LoginAccessLog", "j90BrowserOS", "OS", 1);
            AF("j90LoginAccessLog", "j90BrowserDeviceType", "Device");
            AF("j90LoginAccessLog", "j90BrowserAvailWidth", "Šířka (px)", 1);
            AF("j90LoginAccessLog", "j90BrowserAvailHeight", "Výška (px)", 1);
            AF("j90LoginAccessLog", "j90LocationHost", "Host", 1);
            AF("j90LoginAccessLog", "j90LoginMessage", "Chyba", 1);
            AF("j90LoginAccessLog", "j90CookieExpiresInHours", "Expirace přihlášení", 1, null, "num0");
            AF("j90LoginAccessLog", "j90LoginName", "Login", 1);

            //j92 = ping log uživatelů
            AF("j92PingLog", "j92Date", "Čas", 1, null, "datetime");
            AF("j92PingLog", "j92BrowserFamily", "Prohlížeč", 1);
            AF("j92PingLog", "j92BrowserOS", "OS", 1);
            AF("j92PingLog", "j92BrowserDeviceType", "Device", 1);
            AF("j92PingLog", "j92BrowserAvailWidth", "Šířka (px)", 1);
            AF("j92PingLog", "j92BrowserAvailHeight", "Výška (px)", 1);
            AF("j92PingLog", "j92RequestURL", "Url", 1);


            //j40 = poštovní účty                        
            AF("j40MailAccount", "j40SmtpName", "Jméno odesílatele", 1, null, "string", false, true);
            AF("j40MailAccount", "j40SmtpHost", "Smtp server", 2);
            
            AF("j40MailAccount", "j40SmtpEmail", "Adresa odesílatele", 1);
            AF("j40MailAccount", "j40SmtpPort", "Smtp Port", 2, null, "num0");
            if (_app.Implementation == "UA")
            {
                AF("j40MailAccount", "j40UsageFlag", "Typ účtu", 1, "case a.j40UsageFlag when 1 then N'Приватний рахунок/Privátní Smtp účet' when 2 then N'Глобальний рахунок/Globální Smtp účet' when 3 then 'Osobní Imap účet' when 4 then 'Globální Imap účet' else null end");

            }
            else
            {
                AF("j40MailAccount", "j40UsageFlag", "Typ účtu", 1, "case a.j40UsageFlag when 1 then 'Privátní Smtp účet' when 2 then 'Globální Smtp účet' when 3 then 'Osobní Imap účet' when 4 then 'Globální Imap účet' else null end");

            }


            //x40 = OUTBOX            
            AF("x40MailQueue", "MessageTime", "Čas", 1, "case when a.x40DatetimeProcessed is not null then a.x40DatetimeProcessed else a.x40DateInsert end", "datetime",false,true);            
            AF("x40MailQueue", "x40SenderName", "Odesílatel", 0);
            AF("x40MailQueue", "x40SenderAddress", "Odesílatel (adresa)");
            AF("x40MailQueue", "x40Recipient", "Komu", 1);
            AF("x40MailQueue", "x40CC", "Cc");
            AF("x40MailQueue", "x40BCC", "Bcc");
            if (_app.Implementation == "UA")
            {
                AF("x40MailQueue", "x40Status", "Stav", 1, "case a.x40Status when 1 then N'Чекає відправлення/Čeká na odeslání' when 2 then N'Помилка/Chyba' when 3 then N'Надісланий/Odesláno' when 4 then N'Зупинився/Zastaveno' when 5 then N'В очікуванні схвалення/Čeká na schválení' end");
            }
            else
            {
                AF("x40MailQueue", "x40Status", "Stav", 1, "case a.x40Status when 1 then 'Čeká na odeslání' when 2 then 'Chyba' when 3 then 'Odesláno' when 4 then 'Zastaveno' when 5 then 'Čeká na schválení' end");
            }
            
            AF("x40MailQueue", "x40Subject", "Předmět zprávy", 1);
            AF("x40MailQueue", "x40Body", "Text zprávy", 1, "convert(nvarchar(150),a.x40Body)+'...'");
            AF("x40MailQueue", "x40Attachments", "Přílohy");
            AF("x40MailQueue", "x40EmlFileSize_KB", "Velikost (kB)", 0, "a.x40EmlFileSize/1024", "num0", true);
            AF("x40MailQueue", "x40EmlFileSize_MB", "Velikost (MB)", 0, "convert(float,a.x40EmlFileSize)/1048576", "num", true);
            AF("x40MailQueue", "x40ErrorMessage", "Chyba", 1);



            //x31 = tisková sestava
            AF("x31Report", "x31Name", "Tisková sestava", 1, null, "string", false, true);
            AF("x31Report", "x31PID", "Kód sestavy",2);
            AF("x31Report", "x31Is4SingleRecord", "Kontextová sestava", 2, null, "bool");
            
            AF("x31Report", "Roles", "Oprávnění", 2, "dbo._core_x31_get_role_inline(a.x31ID)");
            AF("x31Report", "Categories", "Kategorie", 1, "dbo._core_x31_get_category_inline(a.x31ID)");
            AF("x31Report", "a10Names", "Typy akcí", 2, "dbo._core_x31_get_a10name_inline(a.x31ID)");
            AF("x31Report", "a08Names", "Témata akcí", 2, "dbo._core_x31_get_a08name_inline(a.x31ID)");
            AF("x31Report", "RepFormat", "Formát", 1, "case a.x31ReportFormat when 1 then 'REP' when 2 then 'DOCX' when 3 then 'XLSX' when 4 then 'MSREP' end");
            AF("x31Report", "x31Description", "Popis");
            AppendTimestamp("x31Report");

            //z01 = pevné tiskové sestavy pro tisk
            AF("z01_core_view_reports", "x31Name", "Tisková sestava", 1, null, "string", false, true);
            AF("z01_core_view_reports", "Preview", "Náhled", 2, "'<a class='+char(34)+'grid-link'+char(34)+' href='+char(34)+'javascript:report_nocontext('+convert(varchar(10),a.x31ID)+',false)'+CHAR(34)+'><img src='+CHAR(34)+'/images/print.png'+CHAR(34)+'/></a>'");
            AF("z01_core_view_reports", "PreviewBlank", "Náhled v nové záložce", 2, "'<a class='+char(34)+'grid-link'+char(34)+' href='+char(34)+'javascript:report_nocontext('+convert(varchar(10),a.x31ID)+',true)'+CHAR(34)+'><img src='+CHAR(34)+'/images/print.png'+CHAR(34)+'/></a>'");

            AF("z01_core_view_reports", "x31PID", "Kód sestavy", 2);
            AF("z01_core_view_reports", "Categories", "Kategorie", 1, "dbo._core_x31_get_category_inline(a.x31ID)");
            AF("z01_core_view_reports", "x31Description", "Popis",1);
            

            //x32 = kategorie sestav
            AF("x32ReportType", "TreeItem", "Kategorie sestavy", 1, "case when a.x32TreeLevel > 1 then replace(space(2 * (a.x32TreeLevel - 1)), ' ', '-') else '' END + a.x32Name", "string", false, true);
            AF("x32ReportType", "x32Name", "Název", 2);            
            AF("x32ReportType", "ParentPath", "Nadřízený", 1, "dbo._core_x32_get_parent_inline(a.x32ID)");
            AF("x32ReportType", "x32Description", "Popis");
            AF("x32ReportType", "x32Ordinal", "#", 2, null, "num0");
            AppendTimestamp("x32ReportType");
            

            //x50 = nápověda
            AF("x51HelpCore", "x51Name", "Nápověda", 1, null, "string", false, true);
            AF("x51HelpCore", "x51ViewUrl", "View Url", 2);
            AF("x51HelpCore", "x51NearUrls", "Související Urls", 2);
            AF("x51HelpCore", "x51ExternalUrl", "Externí Url");
            AppendTimestamp("x51HelpCore");

            //x55 = dashboard widget
            AF("x55Widget", "x55Name", "Widget", 1, null, "string", false, true);
            AF("x55Widget", "x55Code", "Kód widgetu",0,null,"string",false,true);
            AF("x55Widget", "x55Description", "Poznámka",1);
            AF("x55Widget", "x55Skin", "Cílový dashboard");
            AF("x55Widget", "x55DataTablesLimit", "Minimum záznamů pro [DataTables]", 2);            
            AF("x55Widget", "x55Ordinal", "#", 2, null, "num0");
            AppendTimestamp("x55Widget");

            //x24 - datový typ
            AF("x24DataType", "x24Name","Datový typ",1,null,"string",false,true);

            //x29 = entita
            AF("x29Entity", "x29Name", "Entita", 1, null, "string", false, true);
            AF("x29Entity", "x29NamePlural", "Plurál", 2);
            AF("x29Entity", "x29IsAttachment", "Přílohy", 2,null,"bool");

            //x91 = entita
            AF("x91Translate", "x91Code", "Originál", 1, null, "string", false, true);
            AF("x91Translate", "x91Lang1", "English", 1);
            AF("x91Translate", "x91Lang2", "Українська", 1);



            if (_app.Implementation == "Default")
            {
                //k01 = učitel
                AF("k01Teacher", "k01fullname_desc", "Příjmení+Jméno", 1, "a.k01LastName+' '+a.k01FirstName+isnull(' '+a.k01TitleBeforeName,'')", "string", false, true);
                AF("k01Teacher", "k01fullname_asc", "Jméno+Příjmení", 0, "isnull(a.k01TitleBeforeName+' ','')+a.k01FirstName+' '+a.k01LastName+isnull(' '+a.k01TitleAfterName,'')", "string", false, true);
                onecol = AF("k01Teacher", "k01PID", "Kód", 2, null, "string", false, true);
                onecol.FixedWidth = 100;
                AF("k01Teacher", "k01FirstName", "Jméno");
                AF("k01Teacher", "k01LastName", "Příjmení");
                AF("k01Teacher", "k01TitleBeforeName", "Titul před");
                AF("k01Teacher", "k01TitleAfterName", "Titul za");
                AppendTimestamp("k01Teacher");
            }
                

            //o13 = typy příloh
            AF("o13AttachmentType", "TreeItem", "Typ přílohy", 1, "case when a.o13TreeLevel > 1 then replace(space(2 * (a.o13TreeLevel - 1)), ' ', '-') else '' END + a.o13Name", "string", false, true);
            AF("o13AttachmentType", "o13Name", "Název", 2);
            AF("o13AttachmentType", "ParentPath", "Nadřízený",1, "dbo._core_o13_get_parent_inline(a.o13ID)");

            AF("o13AttachmentType", "o13DefaultArchiveFolder", "Upload složka", 2);
            AF("o13AttachmentType", "o13Description", "Popis");
            AF("o13AttachmentType", "o13FilePrefix", "Prefix");
            AF("o13AttachmentType", "o13IsPortalDoc", "Portál", 0, null, "bool");

            AF("o15AutoComplete", "o15Value", "Hodnota", 1);
            AF("o15AutoComplete", "o15Flag", "Typ dat",1, "case a.o15Flag when 1 then 'Titul před' when 2 then 'Titul za' when 3 then 'Pracovní funkce' when 328 then 'Stát' when 427 then 'URL adresa' end");
            AF("o15AutoComplete", "o15Ordinary", "#", 2, null, "num0");
            if (_app.Implementation == "Default")
            {
                AF("v_uraz_jmenozraneneho", "JmenoZraneneho", "Jméno zraněného", 0, null, "string", false, true);
                AF("v_uraz_datumzraneni", "DatumZraneni", "Datum zranění", 0, null, "datetime", false, true);
                AF("v_uraz_poradovecislo", "PoradoveCislo", "Pořadové číslo", 0, null, "string", false, true);
            }
            

            //p86 = Statistika
            AF("p86TempStat", "a01Signature", "ID akce",1);
            
            AF("p86TempStat", "a03REDIZO", "REDIZO", 1);
            AF("p86TempStat", "a37IZO", "IZO", 1);
            AF("p86TempStat", "a17Name", "Typ IZO", 1);
            AF("p86TempStat", "a03Name", "Instituce", 1);            
            AF("p86TempStat", "a09Name", "Typ zřizovatele", 1);
            AF("p86TempStat", "a05Name", "Kraj", 1);
            AF("p86TempStat", "a01DateFrom", "Od", 1, null, "date");
            AF("p86TempStat", "a01DateUntil", "Do", 1, null, "date");
            AF("p86TempStat", "b02Name", "Stav", 1);
            AF("p86TempStat", "a10Name", "Typ akce", 1);
            AF("p86TempStat", "a08Name", "Téma", 1);
            AF("p86TempStat", "a25Name", "SF", 1);
            for (int i = 0; i <= 400; i++)
            {
                AF("p86TempStat", "col"+i.ToString(),"Col"+i.ToString());
               
            }
            
            
        }

        
        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns, BO.baseQuery mq)
        {
            int intDefaultFlag1 = 1; int intDefaultFlag2 = 2;
            if (bolComboColumns == true)
            {
                intDefaultFlag2 = 3;
            }

            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            foreach (BO.TheGridColumn c in _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)))
            {
                ret.Add(Clone2NewInstance(c));
            }

            //List<BO.TheGridColumn> ret = _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)).ToList();

            List<BO.EntityRelation> rels = _ep.getApplicableRelations(mq.Prefix);

            switch (mq.Prefix)
            {
                case "j02":
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j03Login", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j04Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j02_a04", "a04Inspectorate", "a04Name", rels, bolComboColumns));

                    break;
                case "a01":
                    if (_app.Implementation == "HD")
                    {
                        ret.Add(InhaleColumn4Relation("a01_j02", "j02Person", "fullname_asc", rels, bolComboColumns));
                        ret.Add(InhaleColumn4Relation("a01_a03", "a03Institution", "a03REDIZO", rels, bolComboColumns));
                    }
                    else
                    {
                        ret.Add(InhaleColumn4Relation("a01_a10", "a10EventType", "a10Name", rels, bolComboColumns));
                        ret.Add(InhaleColumn4Relation("a01_a03", "a03Institution", "a03Name", rels, bolComboColumns));
                    }
                    
                    ret.Add(InhaleColumn4Relation("a01_a08", "a08Theme", "a08Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a01_b02", "b02WorkflowStatus", "b02Name", rels, bolComboColumns));
                    
                    break;
                case "a02":
                    ret.Add(InhaleColumn4Relation("a02_j02", "j02Person", "fullname_asc", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a02_a04", "a04Inspectorate", "PostAddress", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a02_a05", "a05Region", "a05Name", rels, bolComboColumns));                    
                    break;
                case "a03":
                    if (!bolComboColumns)
                    {
                        ret.Add(InhaleColumn4Relation("a03_a06", "a06InstitutionType", "a06Name", rels, bolComboColumns));
                        ret.Add(InhaleColumn4Relation("a03_a05", "a05Region", "a05Name", rels, bolComboColumns));
                    }                    
                    break;
                case "a04":
                    ret.Add(InhaleColumn4Relation("a04_a05", "a05Region", "a05Name", rels, bolComboColumns));                    
                    break;
                case "a10":
                    ret.Add(InhaleColumn4Relation("a10_b01", "b01WorkflowTemplate", "b01Name", rels, bolComboColumns));
                    break;
                case "a11":
                    ret.Add(InhaleColumn4Relation("a11_f06", "f06Form", "f06Name", rels, bolComboColumns));
                    break;
                case "a19":
                    ret.Add(InhaleColumn4Relation("a19_a37", "a37InstitutionDepartment", "a37Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a19_a37", "a37InstitutionDepartment", "a37IZO", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a19_a18", "a18DepartmentDomain", "a18Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a19_a18", "a18DepartmentDomain", "a18Code", rels, bolComboColumns));
                    break;
                case "a37":
                    ret.Add(InhaleColumn4Relation("a37_a17", "a17DepartmentType", "a17Name", rels, bolComboColumns));                    
                    break;
                case "a39":
                    ret.Add(InhaleColumn4Relation("a39_j02", "j02Person", "fullname_desc", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("a39_j04", "j04UserRole", "j04Name", rels, bolComboColumns));
                    break;
                case "a42":
                    ret.Add(InhaleColumn4Relation("a42_a08", "a08Theme", "a08Name", rels, bolComboColumns));
                    break;
                case "b02":
                    ret.Add(InhaleColumn4Relation("b02_b01", "b01WorkflowTemplate", "b01Name", rels, bolComboColumns));
                   
                    break;
                case "b05":
                    ret.Add(InhaleColumn4Relation("b05_j03", "j03User", "j03Login", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("b05_b06", "b06WorkflowStep", "b06Name", rels, bolComboColumns));
                    break;
                case "b65":
                    ret.Add(InhaleColumn4Relation("b65_x29", "x29Entity", "x29Name", rels, bolComboColumns));
                    break;
                case "f06":
                    ret.Add(InhaleColumn4Relation("f06_f12", "f12FormType", "TreeItem", rels, bolComboColumns));                   
                    break;
                case "f19":
                    ret.Add(InhaleColumn4Relation("f19_f18", "f18FormSegment", "TreeItem", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("f19_f23", "f23ReplyType", "f23Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("f19_x24", "x24DataType", "x24Name", rels, bolComboColumns));
                    break;
                case "f21":
                    ret.Add(InhaleColumn4Relation("f21_f22", "f22ReplySet", "f22Name", rels, bolComboColumns));
                    break;
                case "j23":
                    ret.Add(InhaleColumn4Relation("j23_a05", "a05Region", "a05Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j23_j24", "j24NonPersonType", "j24Name", rels, bolComboColumns));
                    break;
                //case "a37":
                //    ret.Add(InhaleColumn4Relation("a37_a17", "a17DepartmentType", "a17Name", rels, bolComboColumns));
                //    break;


                case "o51":
                    ret.Add(InhaleColumn4Relation("o51_o53", "o53TagGroup", "o53Name", rels, bolComboColumns));
                    break;
            }

            return ret;


        }
        public IEnumerable<BO.TheGridColumn> AllColumns()
        {

            return _lis;


        }
        private BO.TheGridColumn InhaleColumn4Relation(string strRelName, string strFieldEntity, string strFieldName, List<BO.EntityRelation> applicable_rels, bool bolComboColumns)
        {
            BO.TheGridColumn c0 = ByUniqueName("a__" + strFieldEntity + "__" + strFieldName);
            BO.TheGridColumn c = Clone2NewInstance(c0);


            BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == strRelName).First();
            c.RelName = strRelName;
            c.RelSql = rel.SqlFrom;
            if (rel.RelNameDependOn != null)
            {
                c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
            }

            if (c.NotShowRelInHeader == true)
            {
                return c;   //nezobrazovat u sloupce název relace
            }

            if (bolComboColumns == true)
            {
                c.Header = rel.AliasSingular;
            }
            else
            {
                c.Header = c.Header + " [" + rel.AliasSingular + "]";

            }


            return c;
        }
        public BO.TheGridColumn ByUniqueName(string strUniqueName)
        {
            if (_lis.Where(p => p.UniqueName == strUniqueName).Count() > 0)
            {
                return _lis.Where(p => p.UniqueName == strUniqueName).First();
            }
            else
            {
                return null;
            }
        }
        private BO.TheGridColumn Clone2NewInstance(BO.TheGridColumn c)
        {
            return new BO.TheGridColumn() { Entity = c.Entity, EntityAlias = c.EntityAlias, Field = c.Field, FieldType = c.FieldType, FixedWidth = c.FixedWidth, Header = c.Header, SqlSyntax = c.SqlSyntax, IsFilterable = c.IsFilterable, IsShowTotals = c.IsShowTotals, IsTimestamp = c.IsTimestamp, RelName = c.RelName, RelSql = c.RelSql, RelSqlDependOn = c.RelSqlDependOn,NotShowRelInHeader=c.NotShowRelInHeader, TranslateLang1= c.TranslateLang1, TranslateLang2=c.TranslateLang2, TranslateLang3= c.TranslateLang3 };

        }



        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix, string strJ72Columns,int intLangIndex)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field


            List<BO.EntityRelation> applicable_rels = _ep.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();

            string[] arr;
            BO.EntityRelation rel;

            for (var i = 0; i < sels.Count; i++)
            {
                arr = sels[i].Split("__");

                if (_lis.Exists(p => p.Entity == arr[1] && p.Field == arr[2]))
                {
                    //var c0 = _lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First();
                    BO.TheGridColumn c = Clone2NewInstance(_lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First());
                    switch (intLangIndex)
                    {
                        case 1:
                            c.Header = c.TranslateLang1;
                            break;
                        case 2:
                            c.Header = c.TranslateLang2;
                            break;
                        case 3:
                            c.Header = c.TranslateLang3;
                            break;
                        default:
                            c.Header = c.Header;
                            break;
                    }
                    if (arr[0] == "a")
                    {
                        c.RelName = null;
                    }
                    else
                    {
                        c.RelName = arr[0]; //název relace v sql dotazu
                        rel = applicable_rels.Where(p => p.RelName == c.RelName).First();
                        c.RelSql = rel.SqlFrom;    //sql klauzule relace    
                        if (c.NotShowRelInHeader == false)
                        {
                            c.Header = c.Header + " [" + rel.AliasSingular + "]";   //zobrazovat název entity v záhlaví sloupce                           
                        }
                        

                        if (rel.RelNameDependOn != null)
                        {
                            c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
                        }
                    }



                    if ((i == sels.Count - 1) && (c.FieldType == "num" || c.FieldType == "num0" || c.FieldType == "num3"))
                    {
                        c.CssClass = "tdn_lastcol";
                    }
                    ret.Add(c);
                }


            }

            return ret;


        }

        public List<BO.TheGridColumnFilter> ParseAdhocFilterFromString(string strJ72Filter, IEnumerable<BO.TheGridColumn> explicit_cols,int langindex)
        {
            var ret = new List<BO.TheGridColumnFilter>();
            if (String.IsNullOrEmpty(strJ72Filter) == true) return ret;

           
            List<string> lis = BO.BAS.ConvertString2List(strJ72Filter, "$$$");
            foreach (var s in lis)
            {
                List<string> arr = BO.BAS.ConvertString2List(s, "###");
                if (explicit_cols.Where(p => p.UniqueName == arr[0]).Count() > 0)
                {
                    var c = new BO.TheGridColumnFilter() { field = arr[0], oper = arr[1], value = arr[2] };
                    c.BoundColumn = explicit_cols.Where(p => p.UniqueName == arr[0]).First();
                    ParseFilterValue(ref c,langindex);
                    ret.Add(c);
                }


            }
            return ret;
        }

        private void ParseFilterValue(ref BO.TheGridColumnFilter col,int langindex)
        {

            {
                if (col.value.Contains("|"))
                {
                    var a = col.value.Split("|");
                    col.c1value = a[0];
                    col.c2value = a[1];
                }
                else
                {
                    col.c1value = col.value;
                    col.c2value = "";
                }
                switch (col.oper)
                {
                    case "1":
                        {
                            col.value_alias = "Je prázdné";
                            if (langindex == 2) col.value_alias = "пусто";
                            break;
                        }

                    case "2":
                        {
                            col.value_alias = "Není prázdné";
                            if (langindex == 2) col.value_alias = "не є порожнім";
                            break;
                        }

                    case "3":  // obsahuje
                        {
                            col.value_alias = col.c1value;
                            break;
                        }

                    case "5":  // začíná na
                        {

                            col.value_alias = "[*=] " + col.c1value;
                            break;
                        }

                    case "6":  // je rovno
                        {
                            col.value_alias = "[=] " + col.c1value;
                            break;
                        }

                    case "7":  // není rovno
                        {
                            col.value_alias = "[<>] " + col.c1value;
                            break;
                        }

                    case "8":
                        {
                            col.value_alias = "ANO";
                            if (langindex == 2) col.value_alias = "ТАК";
                            break;
                        }

                    case "9":
                        {
                            col.value_alias = "NE";
                            if (langindex == 2) col.value_alias = "НІ";
                            break;
                        }

                    case "10": // je větší než nula
                        {
                            col.value_alias = "větší než 0";
                            if (langindex == 2) col.value_alias = "&gt;0";
                            break;
                        }

                    case "11":
                        {
                            col.value_alias = "0 nebo prázdné";
                            if (langindex == 2) col.value_alias = "0 або порожній";
                            break;
                        }

                    case "4":  // interval
                        {

                            if (col.BoundColumn.FieldType == "date" | col.BoundColumn.FieldType == "datetime")
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;   // datum
                            }
                            else
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;
                            }    // číslo

                            break;
                        }
                }





            }


        }


    }
}
