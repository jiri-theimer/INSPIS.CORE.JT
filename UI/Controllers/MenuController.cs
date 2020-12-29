using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BO;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class MenuController : BaseController
    {
        

        private List<BO.MenuItem> _lis;

        public MenuController()
        {
            _lis = new List<BO.MenuItem>();
        }


        public IActionResult Index()
        {
            return View();
        }

        public string GlobalNavigateMenu()
        {
            //AMI_NOTRA(string.Format("<img src='{0}'/>",Factory.App.LogoImage), null);

            //if (Factory.App.Implementation == "HD")
            //{
            //    AMI_NOTRA("InspIS DATA", "https://inspis.csicr.cz/app");
            //}
            //else
            //{
            //    AMI_NOTRA("InspIS HELPDESK", "https://helpdesk.csicr.cz/app");
            //}
            //AMI_NOTRA("InspIS E-LEARNING", "https://elearning.csicr.cz");
            //AMI_NOTRA("InspIS PORTAL", "https://portal.csicr.cz");
            //AMI_NOTRA("InspIS SET", "https://set.csicr.cz");
            //AMI_NOTRA("InspIS ŠVP", "https://svp.csicr.cz/app");
            

            DIV();
            AMI("Odhlásit se", "/Home/logout");

            return FlushResult_UL(true,false);
        }
        public string CurrentUserMyProfile()
        {                
            AMI("Aktuální stránku uložit jako domovskou", "javascript:_save_as_home_page()",null,null,null, "k-i-heart-outline");
            if (Factory.CurrentUser.j03HomePageUrl !=null)
            {
                if (Factory.CurrentUser.j04ViewUrl_Page == null || Factory.CurrentUser.j03HomePageUrl.ToLower() != Factory.CurrentUser.j04ViewUrl_Page.ToLower())
                {
                    AMI("Vyčistit odkaz na domovskou stránku", "javascript:_clear_home_page()",null,null,null, "k-i-heart-outline k-flip-v");                    
                }                    
            }
            if (Factory.CurrentUser.j03HomePageUrl != null)
            {
                AMI("Tovární HOME stránka", "/Dashboard/Widgets",null,null,null, "k-i-star-outline");
            }
            DIV();
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.SendMail))
            {
                AMI("Můj profil", "/Home/MyProfile",null,null,null, "k-i-user");
                AMI("Odeslat zprávu", "javascript:_sendmail()",null,null,null, "k-i-email");
            }
            
            AMI("Změnit přístupové heslo", "/Home/ChangePassword",null,null,null, "k-i-password");
            DIV();
            AMI("Vyplnit anketní formulář", Factory.App.UiftUrl,null,null,"_blank");
            DIV();
            AMI("Nápověda", "javascript: _window_open('/x51/Index')",null,null,null, "k-i-question");
            //AMI("O aplikaci", "/Home/About");
            DIV();
            AMI("Odhlásit se", "/Home/logout",null,null,null, "k-i-logout");



            return FlushResult_UL(true,false);
        }
        public string CurrentUserLangIndex()
        {
            for (int i = 0; i <= 2; i++)
            {
                string s = "Česky";
                if (i == 1) s = "English";
                if (i == 2) s = "Українська";                
                if (Factory.CurrentUser.j03LangIndex == i) s += "&#10004;";
                if (i != 1) //English zatím neukazovat!
                {
                    AMI_NOTRA(s, string.Format("javascript: save_langindex_menu({0})", i));
                }
                
            }
            return FlushResult_UL(false,false);            
        }
        public string CurrentUserFontSize()
        {
            
            for (int i = 1; i <= 4; i++)
            {
                string s = Factory.tra("Malé písmo");
                if (i == 2) s = Factory.tra("Výchozí velikost písma");
                if (i == 3) s = Factory.tra("Větší");
                if (i == 4) s = Factory.tra("Velké");
                if (Factory.CurrentUser.j03FontStyleFlag == i) s += "&#10004;";
                AMI_NOTRA(s, string.Format("javascript: save_fontsize_menu({0})", i));
                
            }
          
            return FlushResult_UL(false,false);
        }
        public string AdminMenu()
        {
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.uzivatel_er,j03AdminRoleValueFlagEnum.uzivatel_ro))
            {
                AMI("Uživatelé", "/Admin/Users");
            }
            
            
            if (TUP(BO.j05PermValuEnum.AdminGlobal_Ciselniky))
            {
                AMI("Číselníky", "/Admin/Ciselniky");
            }
          
            if (TUP(BO.j05PermValuEnum.FormDesigner))
            {
                AMI("Formuláře", "/Admin/Forms");
                AMI("Návrhář formuláře", "/AdminOneForm/Index");
            }
            if (TUP(BO.j05PermValuEnum.WorkflowDesigner))
            {
                AMI("Workflow", "/Admin/Workflow?prefix=b01");
                AMI("Návrhář workflow", "/AdminOneWorkflow/Index");
            }
            

            AMI("Import"+": "+Factory.tra("Instituce"), "/import/a03");
            //if (TUP(BO.PermValueEnum.H11Admin))
            //{
            //    AMI("Nástěnka", "/Admin/NoticeBoard");
            //}

            return FlushResult_UL(false,false);
        }
        public string AdminWorkflow(string prefix)
        {
            AMI("Workflow šablony", "/Admin/Workflow?prefix=b01");
            AMI("Šablony notifikačních zpráv", "/Admin/Workflow?prefix=b65");

            handle_selected_item(prefix);

            return FlushResult_UL(true,true);
        }
        public string AdminForms(string prefix)
        {            
            AMI("Typy formulářů", url_forms("f12"));
            AMI("Šablony formulářů", url_forms("f06"));
            if (Factory.App.Implementation == "Default")
            {
                AMI("PORTAL boxy", url_forms("f29"));
            }
            
            AMI("Klastry otázek", url_forms("f44"));
            AMI("EVALuation funkce", url_forms("x27"));
            AMI("Jednotky odpovědí", url_forms("f21"));
            AMI("Šablony jednotek odpovědí", url_forms("f22"));

            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        public string AdminUsers(string prefix)
        {
            DIV_TRANS("Správa uživatelů");
            AMI("Uživatelské účty", url_users("j03"));
            AMI("Aplikační role", url_users("j04"));
            AMI("Osobní stránka", null, null, "Dashboard");
            AMI("Osobní stránka inspektora", "/Dashboard/Inspector", "Dashboard");
            AMI("Osobní stránka školy", "/Dashboard/School", "Dashboard");
            AMI("Přihlásit se pod jinou identitou", "/Admin/LogAsUser");

            DIV_TRANS("Osobní profily");
            AMI("Osobní profily", url_users("j02"));
            AMI("Týmy osob", url_users("j11"));
            AMI("Osoby v inspektorátech", url_users("a02"));

            DIV_TRANS("Prostředí");
            AMI("Widgety", url_users("x55"));

            DIV_TRANS("Provoz");
            AMI("PING Log", url_users("j92"));
            //AMI_NOTRA(Factory.tra("PING Log")+": "+Factory.tra(string.Format("{0} minut",5)), url_users("j92&dateadd=-5m"));
            AMI("LOGIN Log", url_users("j90"));
            AMI("WORKFLOW Log", url_users("b05"));
            DIV_TRANS("Pošta");
            AMI("Poštovní účty", url_users("j40"));
            AMI("OUTBOX", url_users("x40"));
            AMI("MAIL fronta", "/mail/MailBatchFramework");
            
            

            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        public string AdminCiselniky(string prefix)
        {                        
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.inspektorat_er, j03AdminRoleValueFlagEnum.inspektorat_ro))
            {
                DIV("Inspektoráty");
                AMI("Osoby v inspektorátech", url_ciselniky("a02"));
                AMI("Inspektoráty", url_ciselniky("a04"));
                AMI("Kraje", url_ciselniky("a05"));
            }
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.neper_er, j03AdminRoleValueFlagEnum.neper_ro))
            {
                DIV_TRANS("Nepersonální zdroje");
                AMI("Zdroje", url_ciselniky("j23"));
                AMI("Typy zdrojů", url_ciselniky("j24"));
                AMI("Důvody rezervace", url_ciselniky("j25"));
            }
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.instituce_er, j03AdminRoleValueFlagEnum.instituce_ro))
            {
                DIV_TRANS("Instituce");
                AMI("Pojmenované seznamy", url_ciselniky("a29"));
                AMI("Typy zřizovatelů", url_ciselniky("a09"));
                AMI("Typy škol", url_ciselniky("a28"));
                AMI("Typy činností", url_ciselniky("a17"));
                AMI("Kódy vzdělávacích oborů", url_ciselniky("a18"));
                AMI("Školní informační systémy", url_ciselniky("a70"));
            }
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.akce_er, j03AdminRoleValueFlagEnum.akce_ro))
            {
                DIV_TRANS("Akce");
                AMI("Typy akcí", url_ciselniky("a10"));
                AMI("Témata akcí", url_ciselniky("a08"));
            }
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.ukol_er, j03AdminRoleValueFlagEnum.ukol_ro))
            {
                DIV_TRANS("Úkoly/Lhůty");
                AMI("Typy úkolů a lhůt", url_ciselniky("h07"));
            }
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.sestava_er, j03AdminRoleValueFlagEnum.sestava_ro))
            {
                DIV_TRANS("Pevné tiskové sestavy");
                AMI("Report šablony", url_ciselniky("x31"));
                AMI("Report kategorie", url_ciselniky("x32"));
            }

            DIV_TRANS("Ostatní");
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.priloha_er, j03AdminRoleValueFlagEnum.priloha_ro))
            {
                AMI("Typy příloh", url_ciselniky("o13"));
            }
            if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.ostatni_er, j03AdminRoleValueFlagEnum.ostatni_ro))
            {                
                //DIV_TRANS("Kategorie");
                AMI("Kategorie (Skupiny položek)", url_ciselniky("o53"));
                AMI("Položky kategorií", url_ciselniky("o51"));
                
                AMI("Šablony notifikačních zpráv", url_ciselniky("b65"));
                AMI("Články pro nástěnku", url_ciselniky("h11"));
                AMI("Dny svátků", url_ciselniky("j26"));
                AMI("AutoComplete položky", url_ciselniky("o15"));
                
                AMI("Uživatelská nápověda", url_ciselniky("x51"));
                AMI("Aplikační překlad", url_ciselniky("x91"));
            }


            handle_selected_item(prefix);

            return FlushResult_UL(false,true);
        }
        private string url_ciselniky(string prefix)
        {
            return "/Admin/Ciselniky?prefix=" + prefix;

        }
        private string url_forms(string prefix)
        {
            return "/Admin/Forms?prefix=" + prefix;

        }
        private string url_users(string prefix)
        {
            return "/Admin/Users?prefix=" + prefix;
        }
        private void handle_selected_item(string prefix)
        {
            if (prefix != null)
            {
                if (_lis.Where(p => p.Url != null && p.Url.Contains("=" + prefix)).Count() > 0)
                {
                    _lis.Where(p => p.Url != null && p.Url.Contains("=" + prefix)).First().IsActive = true;
                }
            }
        }

        public string A01Menu(string userdevice)
        {
            if (Factory.j04UserRoleBL.IsA01Create(Factory.CurrentUser.j04ID)==true)
            {
                AMI("Nový", "/a01Create/Index");
                DIV();
            }
            AMI("Stránka", "/a01/RecPage");
            if (TUP(BO.j05PermValuEnum.A01Grid))
            {
               
                if (userdevice != "Phone" && Factory.CBL.LoadUserParamBool("grid-a01-show11", false))
                {
                    AMI("Grid 1+1", "/TheGrid/MasterView?prefix=a01");
                }
                else
                {
                    AMI("Grid 1", "/TheGrid/FlatView?prefix=a01");
                }                                
            }
            
            
            if (Factory.App.Implementation != "HD")
            {
                if (Factory.CurrentUser.TestPermission(j05PermValuEnum.Menu_RS))
                {
                    DIV();
                    AMI("INEZ: Grid", "/TheGrid/MasterView?prefix=a42");
                    AMI("INEZ: Nový", "/a42/CreatePre");
                }
                
                DIV();
                AMI("Časové kapacity inspektorů", "/a35/TimeLine",null,null,"_blank");
                AMI("Rezervace nepersonálních zdrojů", "/a38/TimeLine",null,null, "_blank");                
                
            }            

            return FlushResult_UL(false,false);
        }
        public string A03Menu(string userdevice)
        {
            if (TUP(BO.j05PermValuEnum.A03Admin)==true)
            {
                AMI("Nový", "javascript:_edit('a03',0,'"+Factory.tra("Založit instituci")+"')");
                DIV();
            }
            AMI("Stránka", "/a03/RecPage");
            if (userdevice != "Phone" && Factory.CBL.LoadUserParamBool("grid-a03-show11", false))
            {
                AMI("Grid 1+1", "/TheGrid/MasterView?prefix=a03");
            }
            else
            {
                AMI("Grid 1", "/TheGrid/FlatView?prefix=a03");
            }           
            
            return FlushResult_UL(false,false);
        }
        public string J02Menu(string userdevice)
        {
            
            AMI("Stránka", "/j02/RecPage");
            if (userdevice != "Phone" && Factory.CBL.LoadUserParamBool("grid-j02-show11", false))
            {
                AMI("Grid 1+1", "/TheGrid/MasterView?prefix=j02");
            }
            else
            {
                AMI("Grid 1", "/TheGrid/FlatView?prefix=j02");
            }
            //DIV();
            //AMI("Učitelé", "/TheGrid/FlatView?prefix=k01");

            return FlushResult_UL(false,false);
        }
        public string H04Menu()
        {
            AMI("Nový", "javascript:_edit('h04',0,'Vytvořit úkol')");
            DIV();
            AMI("Stránka", "/h04/ThePage");            
            AMI("Grid 1", "/TheGrid/FlatView?prefix=h04");

            return FlushResult_UL(false,false);
        }
        public string X31Menu()
        {
            if (TUP(BO.j05PermValuEnum.Menu_Reports))
            {
                AMI("Pevné tiskové sestavy", "/TheGrid/FlatView?prefix=z01");
            }
            if (TUP(BO.j05PermValuEnum.Menu_Analyze))
            {
                AMI("Statistiky", "/Stat/Index");
            }

            return FlushResult_UL(false,false);
        }
        public string ContextMenu(string entity,int pid, string flag)
        {            
            if (entity == "Ciselniky") { return AdminCiselniky(flag); };          
            if (entity == "Users") { return AdminUsers(flag); };
            if (entity == "Forms") { return AdminForms(flag); };
            if (entity == "Workflow") { return AdminWorkflow(flag); };

            string prefix = entity.Substring(0, 3);

            switch (prefix)
            {
                case "a01":
                    var recA01 = Factory.a01EventBL.Load(pid);
                    if (flag != "recpage")
                    {
                        HEADER(recA01.a01Signature);
                    }                    
                    var recA10 = Factory.a10EventTypeBL.Load(recA01.a10ID);
                    var permA01 = Factory.a01EventBL.InhalePermission(recA01);
                    if (recA01.a01IsTemporary)
                    {
                        DIV();
                        AMI("Nenávratně odstranit akci", string.Format("javascript: _window_open('/a01/KillRecord?pid={0}')", pid));
                    }
                    if (recA10.a10CoreFlag =="injury" || recA10.a10CoreFlag=="inez")
                    {
                        //akce s jednoduchým menu jako ÚRAZ
                        AMI("Posunout/Doplnit", string.Format("javascript: _window_open('/workflow/Dialog?pid={0}')", pid));
                        AMI("Tisková sestava", string.Format("javascript: _window_open('/x31/ReportContext?pid={0}&prefix=a01',2)", pid));
                        if (recA01.a42ID > 0)
                        {
                            if (string.IsNullOrEmpty(Factory.a42QesBL.Load(recA01.a42ID).f06IDs_Poll )==false)
                            {
                                AMI("Anketní formuláře", string.Format("javascript: _window_open('/a11/AppendPoll?a01id={0}',2)", pid));
                            }
                        }
                        
                        
                        break;
                    }                    
                    
                    var lisA24 = Factory.a01EventBL.GetList_a24(pid);
                    if (lisA24.Where(p => p.a46ID == 4 && p.a01ID_Right==pid).Count() > 0)
                    {
                        AMI_NOTRA(Factory.tra("Podřízené akce") + " (" + lisA24.Where(p => p.a46ID == 4 && p.a01ID_Right == pid).Count().ToString() + ")<img src='/Images/child.png'/>", null, null, "Podrizena");
                        //RenderPodrizeneAkce(recA01.pid, "Nadrizena", recA01);
                        RenderAkceSeVztahem("Podrizena", recA01, lisA24.Where(p => p.a46ID == 4));
                    }
                    
                    if (lisA24.Where(p => p.a46ID == 3).Count() > 0)
                    {
                        AMI_NOTRA(Factory.tra("Souvisí s") + " (" + lisA24.Where(p => p.a46ID == 3).Count().ToString() + ")<img src='/Images/souvisejici.png'/>", null, null, "Souvisi");
                        RenderAkceSeVztahem("Souvisi", recA01, lisA24.Where(p => p.a46ID == 3));
                    }
                    if (lisA24.Where(p => p.a46ID == 1).Count() > 0)
                    {
                        AMI_NOTRA(Factory.tra("Závisí na") + " (" + lisA24.Where(p => p.a46ID == 1).Count().ToString() + ")<img src='/Images/zavisla.png'/>", null, null, "Zavisi");
                        RenderAkceSeVztahem("Zavisi", recA01, lisA24.Where(p => p.a46ID == 1));
                    }
                    if (lisA24.Where(p => p.a46ID == 2).Count() > 0)
                    {
                        AMI_NOTRA(Factory.tra("Je duplikátem") + " (" + lisA24.Where(p => p.a46ID == 2).Count().ToString() + ")<img src='/Images/duplikat.png'/>", null, null, "Duplikat");
                        RenderAkceSeVztahem("Duplikat", recA01, lisA24.Where(p => p.a46ID == 2));
                    }
                    if (recA01.a01ParentID > 0)
                    {
                        AMI_NOTRA(Factory.tra("Nadřízená")+ "<img src='/Images/mother.png'/>"+Factory.a01EventBL.Load(recA01.a01ParentID).a01Signature, Factory.a01EventBL.GetPageUrl(recA01,recA01.a01ParentID),null,"Nadrizena","_top");
                        RenderAkceSeVztahem("Nadrizena", recA01, lisA24.Where(p => p.a46ID == 4 && p.a01ID_Left !=pid));
                        //RenderPodrizeneAkce(recA01.a01ParentID, "Nadrizena",recA01);                        
                    }
                    else
                    {
                        AMI("Posunout/Doplnit", string.Format("javascript: _window_open('/workflow/Dialog?pid={0}')", pid));
                        if (!recA01.isclosed)
                        {
                            AMI("Nahrát přílohu", string.Format("javascript:_window_open('/a01/AddAttachment?pid={0}')", pid));
                        }
                        
                    }



                    AMI("Tisková sestava", string.Format("javascript: _window_open('/x31/ReportContext?pid={0}&prefix=a01',2)", pid));
                  
                    if (permA01.PermValue == a01EventPermissionENUM.ShareTeam_Leader || permA01.PermValue == a01EventPermissionENUM.ShareTeam_Owner || permA01.PermValue == a01EventPermissionENUM.FullAccess)
                    {
                        
                        if (recA10.a10IsUse_ReChangeForms || recA10.a10IsUse_Poll)
                        {
                            AMI("Formuláře", null, null, "Formulare");
                        }                            
                        if (recA10.a10IsUse_ReChangeForms)
                        {
                            if (!recA01.a01IsAllFormsClosed && !recA01.isclosed)
                            {
                                AMI("Přidat formuláře", string.Format("javascript: _window_open('/a11/Append?a01id={0}')", pid), "Formulare");
                                AMI("Přidat skupinu formulářů", string.Format("javascript: _window_open('/a25/Record?a01id={0}')", pid), "Formulare");
                            }                            
                            AMI("Otestovat vyplnění formulářů v akci", string.Format("javascript: _window_open('/a11/ValidateForms?a01id={0}')", pid), "Formulare");
                        }
                        if (!recA01.isclosed)
                        {
                            if (recA10.a10IsUse_Poll)
                            {
                                DIV(null, "Formulare");
                                AMI("Anketní formuláře", string.Format("javascript: _window_open('/a11/AppendPoll?a01id={0}',2)", pid), "Formulare");
                                AMI("Založit+notifikovat anketu", string.Format("javascript: _window_open('/a11/AppendPollWizard?a01id={0}')", pid), "Formulare");
                            }
                            if (recA10.a10IsUse_A41 && recA01.a01ParentID == 0)
                            {
                                AMI("Přidat účastníky akce", string.Format("javascript: _window_open('/a41/Append?a01id={0}')", pid));
                            }
                        }
                        
                        if (recA10.a10IsUse_Period && recA01.a01ParentID == 0)
                        {
                            DIV();
                            AMI("Upravit časový plán akce", string.Format("javascript: _window_open('/a35/CapacityEdit?pid={0}')", pid));
                            AMI("Časové kapacity inspektorů", string.Format("javascript: _window_open('/a35/TimeLine?a05id={0}&a01id={1}',2)", recA01.a05ID,pid));
                            if (recA10.a10CoreFlag == "aus")
                            {
                                //AUS rezervace mimo inspekce
                                AMI("Rezervace nepersonálního zdroje", string.Format("javascript: _window_open('/a38/CreateAus?a01id={0}',2)", pid));   //standardní akce                                
                            }
                            else
                            {
                                AMI("Rezervace nepersonálního zdroje", string.Format("javascript: _window_open('/a38/AppendToA01?pid={0}',2)", pid));   //standardní akce
                            }
                            
                        }

                        
                    }
                    
                    

                    DIV();
                    AMI("Nový úkol (lhůta)", string.Format("javascript: _window_open('/h04/Record?pid=0&a01id={0}')", pid));
                    AMI("Zadat související akci", string.Format("javascript: _window_open('/a01/AddSouvisejici?pid={0}')", pid));
                    AMI("Odeslat zprávu", string.Format("javascript: _window_open('/Mail/SendMail?x29id=101&x40datapid={0}',2)", pid));


                    AMI("Záznam akce", null, null, "Zaznam");
                    if (permA01.PermValue == a01EventPermissionENUM.ShareTeam_Leader || permA01.PermValue == a01EventPermissionENUM.ShareTeam_Owner || permA01.PermValue == a01EventPermissionENUM.FullAccess)
                    {
                        if (recA01.a01ParentID == 0 && recA10.a10ViewUrl_Insert==null)
                        {
                            AMI("Kopírovat akci", string.Format("/a01Create/Standard?clonebypid={0}", pid), "Zaznam",null,"_top");
                        }
                        AMI("Upravit základní vlastnosti (kartu)", string.Format("javascript: _window_open('/a01/Record?pid={0}')", pid), "Zaznam");
                    }

                    if (Factory.CurrentUser.TestPermission(j05PermValuEnum.AdminGlobal))
                    {
                        AMI("Nenávratně odstranit akci", string.Format("javascript: _window_open('/a01/KillRecord?pid={0}')", pid), "Zaznam");
                        DIV(null, "Zaznam");
                    }
                    
                    AMI("Stránka akce", "javascript:_location_replace_top('"+Factory.a01EventBL.GetPageUrl(recA01)+"')","Zaznam");
                    if (recA01.isclosed == true)
                    {
                        AMI_NOTRA("<kbd>" + Factory.tra("Záznam je v archivu.") + "</kbd>", "");
                    }
                    break;
                case "a03":
                    var recA03 = Factory.a03InstitutionBL.Load(pid);
                    if (TUP(j05PermValuEnum.A03Admin))
                    {
                        AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                        AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                        DIV();
                        AMI("Přidat kontaktní osobu", string.Format("javascript: _window_open('/a39/Record?pid=0&a03id={0}')", pid));
                        if (recA03.a06ID == 1)
                        {
                            AMI("Přidat činnost školy", string.Format("javascript: _window_open('/a37/Record?pid=0&a03id={0}')", pid));
                            AMI("Přidat vzdělávací obor", string.Format("javascript: _window_open('/a19/Record?pid=0&a03id={0}')", pid));
                        }
                    }
                    
                    if (Factory.j04UserRoleBL.IsA01Create(Factory.CurrentUser.j04ID))
                    {
                        DIV();
                        AMI("Založit novou akci", "/a01Create/Index?a03id="+pid.ToString());                        
                    }
                    DIV();
                    AMI("Tisková sestava", string.Format("javascript: _window_open('/x31/ReportContext?pid={0}&prefix=a03',2)", pid));
                    AMI("Odeslat zprávu", string.Format("javascript: _window_open('/Mail/SendMail?x29id=103&x40datapid={0}',2)", pid));
                    AMI("Stránka instituce", string.Format("javascript:_location_replace_top('/a03/RecPage?pid={0}')", recA03.pid));
                    if (recA03.isclosed == true)
                    {
                        AMI_NOTRA("<kbd>"+Factory.tra("Záznam je v archivu.")+"</kbd>", "");
                    }
                    break;
                case "j02":
                    var recJ02 = Factory.j02PersonBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                    
                    if (Factory.CurrentUser.TestPermCiselniky(j03AdminRoleValueFlagEnum.uzivatel_er, j03AdminRoleValueFlagEnum._none))
                    {
                        DIV();
                        if (recJ02.j03ID > 0)
                        {
                            AMI("Uživatelský účet", string.Format("javascript:_edit('j03',{0})", recJ02.j03ID));
                        }
                        else
                        {
                            //AMI("Založit uživatelský účet", "javascript:_edit('j03',0)");
                            AMI("Založit uživatelský účet", string.Format("javascript: _window_open('/j03/Record?pid=0&j02id={0}')", pid));
                        }
                        
                    }
                    DIV();
                    AMI("Přidat kontaktní instituci", string.Format("javascript: _window_open('/a39/RecordByPerson?pid=0&j02id={0}')", pid));
                    DIV();                    
                    AMI("Tisková sestava", string.Format("javascript: _window_open('/x31/ReportContext?pid={0}&prefix=j02',2)", pid));
                    AMI("Odeslat zprávu", string.Format("javascript: _window_open('/Mail/SendMail?j02id={0}&x29id=502&x40datapid={0}',2)", pid));
                    AMI("Stránka osoby", string.Format("javascript:_location_replace_top('/j02/RecPage?pid={0}')",pid));
                    break;
                case "a11":
                    var recA11 = Factory.a11EventFormBL.Load(pid);
                    
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    DIV();
                    if (recA11.isclosed==false)
                    {
                        AMI("Vyplnit formulář", Factory.App.UiftUrl + "/Formular/" + pid.ToString(), null, null, "_blank");
                    }
                    AMI("Náhled vyplněného formuláře", Factory.App.UiftUrl + "/Preview/Formular/" + pid.ToString(), null, null, "_blank");
                    DIV();
                    AMI("Otestovat vyplnění formuláře", string.Format("javascript: _window_open('/a11/ValidateForms?pid={0}')", pid));
                    AMI("Otestovat vyplnění všech formulářů v akci", string.Format("javascript: _window_open('/a11/ValidateForms?a01id={0}')", recA11.a01ID));
                    DIV();
                    AMI("Zobrazit historii přístupů k šifrovaným otázkám", "");
                    bool b = Factory.CurrentUser.TestPermission(j05PermValuEnum.AdminGlobal);
                    if (!b)
                    {
                        var perm = Factory.a01EventBL.InhalePermission(Factory.a01EventBL.Load(recA11.a01ID));
                        if (perm.PermValue == BO.a01EventPermissionENUM.FullAccess || perm.PermValue == BO.a01EventPermissionENUM.ShareTeam_Owner || perm.PermValue == BO.a01EventPermissionENUM.ShareTeam_Leader)
                        {
                            b = true;
                        }
                    }
                    if (b)
                    {
                        string s = "'"+Factory.tra("Opravdu nenávratně odtranit vyčistit odpovědi ve formuláři?")+"'";
                        AMI("Vyčistit data ve formuláři (nenávratně)", "javascript:clear_form("+pid.ToString()+","+s+")");
                    }                    
                    if (TUP(BO.j05PermValuEnum.FormDesigner))
                    {
                        DIV();
                        AMI("Návrhář formuláře", string.Format("/AdminOneForm/Index?f06id={0}", recA11.f06ID),null,null,"_top");
                    }
                        

                    break;
                case "a41":
                    var recA41 = Factory.a41PersonToEventBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    DIV();
                    AMI_NOTRA(string.Format("{0}: Osobní stránka", recA41.PersonAsc), string.Format("/j02/RecPage?pid={0}", recA41.j02ID),null,null,"_top");
                    break;
                case "a42":
                    var recA42 = Factory.a42QesBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    DIV();
                    AMI("Nový", "javascript:_location_replace_top('/a42/CreatePre')");
                    break;
                case "a39":
                    var recA39 = Factory.a39InstitutionPersonBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                    DIV();
                    AMI("Stránka osoby", string.Format("javascript:_location_replace_top('/j02/RecPage?pid={0}')", recA39.j02ID));
                    break;
                case "h04":
                    var recH04 = Factory.h04ToDoBL.Load(pid);
                    if (recH04.h07IsToDo)
                    {
                        AMI("Posunout stav úkolu", string.Format("javascript: _window_open('/h04/MoveStatus?pid={0}')", pid));
                        DIV();
                    }
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                    DIV();
                    AMI("Odeslat zprávu", string.Format("javascript: _window_open('/Mail/SendMail?x29id=604&x40datapid={0}',2)", pid));
                    DIV();
                    var cA01 = Factory.a01EventBL.Load(recH04.a01ID);
                    AMI_NOTRA(string.Format(Factory.tra("Stránka akce {0}"), cA01.a01Signature), "javascript:_location_replace_top('" + Factory.a01EventBL.GetPageUrl(cA01) + "')");
                    
                    break;
                case "f06":
                    if (flag != "recpage")
                    {
                        HEADER(Factory.f06FormBL.Load(pid).f06Name);
                        AMI("Návrhář formuláře", string.Format("/AdminOneForm/Index?f06id={0}", pid));
                        DIV();
                    }
                    
                    AMI("Nový segment formuláře", string.Format("javascript: _window_open('/f18/Record?f06id={0}')", pid));
                    AMI("Nová otázka", string.Format("javascript: _window_open('/f19/Record?f06id={0}')", pid));
                    DIV();
                    AMI("Upravit hlavičku formuláře", string.Format("javascript:_edit('f06',{0})", pid));
                    AMI("Kopírovat formulář", string.Format("javascript:_clone('f06',{0})", pid));
                    
                    break;
                case "f18":
                    HEADER(Factory.f18FormSegmentBL.Load(pid).f18Name);
                    AMI("Karta záznamu", string.Format("javascript:_edit('f18',{0})", pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('f18',{0})", pid));
                    DIV();
                    AMI("Nová otázka", string.Format("javascript: _window_open('/f19/Record?f18id={0}')", pid));
                    AMI("Nová baterie otázek", string.Format("javascript: _window_open('/f26/Record?f18id={0}')", pid));
                    //AMI("Nová šachovnice otázek", string.Format("javascript: _window_open('/f25/Record?f18id={0}')", pid));
                    break;
                case "x40":
                    var recX40 = Factory.MailBL.LoadMessageByPid(pid);
                    
                    AMI("Detail zprávy"+" ["+ recX40.StateAlias+"]", string.Format("javascript:_edit_full('Mail','Record',{0})",pid));
                    DIV();
                    AMI("Zkopírovat do nové zprávy", string.Format(string.Format("javascript: _window_open('/Mail/SendMail?x40id={0}')",pid)));
                    
                    if (recX40.x29ID == 101)
                    {                        
                        DIV();AMI("Stránka akce",Factory.a01EventBL.GetPageUrl(Factory.a01EventBL.Load(recX40.x40DataPID)),null,null,"_top");
                    }
                    if (recX40.x29ID == 604)
                    {
                        DIV(); AMI("Stránka úkolu", "/h04/RecPage?pid="+recX40.x40DataPID.ToString(), null, null, "_top");
                    }
                    if (recX40.x29ID == 502)
                    {
                        DIV(); AMI("Stránka osoby", "/j02/RecPage?pid=" + recX40.x40DataPID.ToString(), null, null, "_top");
                    }
                    break;
                case "o27":
                    var recO27 = Factory.o27AttachmentBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('o27',{0})", pid));
                    DIV();
                    AMI("Otevřít/Stáhnout dokument", string.Format("/FileUpload/FileDownloadInline?downloadguid={0}", recO27.o27DownloadGUID),null,null,"_blank");
                    break;
                case "b01":
                    if (flag != "recpage")
                    {
                        HEADER(Factory.b01WorkflowTemplateBL.Load(pid).b01Name);
                        AMI("Návrhář workflow", string.Format("/AdminOneWorkflow/Index?b01id={0}", pid));
                        DIV();
                    }
                    
                    AMI("Nový workflow stav", string.Format("javascript: _window_open('/b02/Record?b01id={0}')", pid));
                    
                    DIV();
                    AMI("Upravit hlavičku workflow", string.Format("javascript:_edit('b01',{0})", pid));
                    AMI("Kopírovat workflow šablonu", string.Format("javascript:_clone('b01',{0})", pid));

                    break;
                case "b02":
                    AMI("Karta záznamu", string.Format("javascript:_edit('b02',{0})", pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('b02',{0})", pid));
                    DIV();
                    AMI("Nový workflow krok", string.Format("javascript: _window_open('/b06/Record?b02id={0}')", pid));
                    break;
                case "z01":
                    AMI("Náhled tiskové sestavy", string.Format("javascript: _window_open('/x31/ReportNoContext?x31id={0}',2)", pid));
                    if (Factory.CurrentUser.TestPermission(j05PermValuEnum.AdminGlobal))
                    {
                        DIV();
                        AMI("Administrace", string.Format("javascript:_edit('x31',{0})", pid));
                    }
                    break;
                case "j90":
                case "j92":
                case "p44":
                case "k01":
                case "p86":
                    AMI("Záznam bez menu nabídky", "javascript:_notify_message('nic')");
                    break;
                case "j03":
                    var recJ03 = Factory.j03UserBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                    if (recJ03.j02ID > 0)
                    {
                        DIV();
                        AMI("Odeslat zprávu", string.Format("javascript: _window_open('/Mail/SendMail?j02id={0}&x29id=503&x40datapid={1}',2)", recJ03.j02ID, pid));
                    }
                    
                    break;
                case "h11":
                    if (Factory.CurrentUser.TestPermission(j05PermValuEnum.H11Admin))
                    {
                        AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                        AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                        DIV();
                    }                   
                    AMI("Náhled", string.Format("javascript: _window_open('/h11/Info?pid={0}',1)", pid));
                    break;
                default:
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));


                    break;
            }

            
            //if (prefix == "p28" || prefix == "j02" || prefix == "p10" || prefix == "p13" || prefix == "p26" || prefix=="p27" || prefix == "p21" || prefix == "p51" || prefix == "p41" || prefix=="p11" || prefix=="p12" || prefix=="o23")
            //{
            //    DIV();
            //    AMI("Info", string.Format("javascript:_window_open('/{0}/Index?pid={1}')", prefix, pid));
            //}
          

            return FlushResult_UL(false,true);
        }
        private void RenderAkceSeVztahem(string strMenuParentID, BO.a01Event recCurrentA01,IEnumerable<BO.a24EventRelation> lisA24)
        {
            string strName = "";
            foreach(var c in lisA24)
            {
                strName = c.a01Signature_Left + " [" + c.a10Name_Left + "]";
                switch (c.a46ID)
                {
                    case 4:
                        strName += "<img src='/Images/child.png'/>";
                        break;
                    case 3:
                        strName += "<img src='/Images/souvisejici.png'/>";
                        break;
                    case 2:
                        strName += "<img src='/Images/zavisla.png'/>";
                        break;
                    case 1:
                        strName += "<img src='/Images/duplikat.png'/>";
                        break;
                }
                var recA01 = Factory.a01EventBL.Load(c.a01ID_Left);
                AMI_NOTRA(strName, Factory.a01EventBL.GetPageUrl(recA01), strMenuParentID,null,"_top");
            }
           
           
        }



        private void AMI(string strName,string strUrl, string strParentID = null,string strID=null, string strTarget = null, string icon = null)
        {            
            _lis.Add(new BO.MenuItem() { Name = Factory.tra(strName), Url = strUrl,Target=strTarget,ID=strID,ParentID=strParentID,Icon=icon });
        }
        private void AMI_NOTRA(string strName, string strUrl, string strParentID = null, string strID = null, string strTarget = null)
        {           
            _lis.Add(new BO.MenuItem() { Name = strName, Url = strUrl, Target = strTarget, ID = strID, ParentID = strParentID });
        }
        private void DIV(string strName=null, string strParentID = null)
        {
            _lis.Add(new BO.MenuItem() { IsDivider = true, Name = BO.BAS.OM2(strName,30),ParentID=strParentID });
        }
        private void DIV_TRANS(string strName = null)
        {
            _lis.Add(new BO.MenuItem() { IsDivider = true, Name = BO.BAS.OM2(Factory.tra(strName), 30) });
        }
        private void HEADER(string strName)
        {
            _lis.Add(new BO.MenuItem() { IsHeader = true, Name = BO.BAS.OM2(strName, 100)+":" });
        }

        private string FlushResult_UL(bool bolSupportIcons,bool bolRenderUlContainer)
        {
            var sb = new System.Text.StringBuilder();
            
            if (bolRenderUlContainer)
            {
                sb.AppendLine("<ul style='border:0px;'>");
            }
            foreach (var c in _lis.Where(p => p.ParentID == null))
            {
                if (c.IsDivider == true)
                {
                    if (c.Name == null)
                    {
                        sb.Append("<li><hr></li>");  //divider
                    }
                    else
                    {
                        sb.Append("<div class='hr-text'>" + c.Name + "</div>");
                    }

                }
                else
                {
                    if (c.IsHeader)
                    {
                        sb.Append("<div style='color:silver;font-style: italic;'>" + c.Name + "</div>");
                    }
                    else
                    {
                        string strStyle = "";
                        string strImg = "<span style='margin-left:10px;'></span>";
                        if (bolSupportIcons)
                        {
                            strImg = "<span class='k-icon' style='width:30px;'></span>";
                            if (c.Icon != null)
                            {
                                strImg = string.Format("<span class='k-icon {0}' style='width:30px;'></span>", c.Icon);
                            }
                        }
                        
                        if (c.IsActive == true)
                        {
                            strStyle = " style='background-color: #ADD8E6;' id='menu_active_item'";
                        }
                        bool bolHasChilds = false;
                        if (c.ID != null && _lis.Where(p => p.ParentID == c.ID).Count() > 0)
                        {
                            bolHasChilds = true;
                            c.Name += "<span class='k-icon k-i-arrow-60-right' style='float:right;'></span>";
                        }

                        if (c.Url == null)
                        {
                            sb.Append(string.Format("<li{0}><a>{1}</a>", strStyle, c.Name));
                        }
                        else
                        {
                            if (c.Target != null) c.Target = " target='" + c.Target + "'";
                            sb.Append(string.Format("<li{0}><a class='dropdown-item px-0' href=\"{1}\"{2}>{3}{4}</a>", strStyle, c.Url, c.Target, strImg, c.Name));


                        }
                        if (bolHasChilds)
                        {
                            //podřízené nabídky -> druhá úroveň »
                            sb.Append("<ul class='cm_submenu'>");
                            foreach (var cc in _lis.Where(p => p.ParentID == c.ID))
                            {
                                if (cc.IsDivider)
                                {
                                    sb.Append("<li><hr></li>");  //divider
                                }
                                else
                                {
                                    if (cc.Target != null) cc.Target = " target='" + cc.Target + "'";
                                    sb.Append(string.Format("<li><a class='dropdown-item' href=\"{0}\"{1}>{2}</a></li>", cc.Url, cc.Target, cc.Name));
                                }

                            }
                            sb.Append("</ul>");
                        }

                        sb.Append("</li>");
                    }

                }

            }

            if (bolRenderUlContainer)
            {
                sb.Append("</ul>");
            }
            return sb.ToString();
        }
        //private string FlushResult_NAVLINKs()
        //{
        //    var sb = new System.Text.StringBuilder();

        //    foreach (var c in _lis)
        //    {
        //        if (c.Name == null)
        //        {
        //            sb.Append("<hr>");  //divider
        //        }
        //        else
        //        {
        //            if (c.Url == null)
        //            {
        //                sb.Append(string.Format("<span>{0}</span>", c.Name));
        //            }
        //            else
        //            {
        //                if (c.Target != null) c.Target = " target='" + c.Target + "'";
        //                sb.Append(string.Format("<a class='nav-link' style='color:black;' href=\"{0}\"{1}>{2}</a>", c.Url, c.Target, c.Name));
        //            }
        //        }

        //    }


        //    return sb.ToString();
        //}
    }
}