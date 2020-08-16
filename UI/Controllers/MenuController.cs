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
            AMI_NOTRA(string.Format("<img src='{0}'/>",Factory.App.LogoImage), null);

            if (Factory.App.Implementation == "HD")
            {
                AMI_NOTRA("InspIS DATA", "https://inspis.csicr.cz/app");
            }
            else
            {
                AMI_NOTRA("InspIS HELPDESK", "https://helpdesk.csicr.cz/app");
            }
            AMI_NOTRA("InspIS E-LEARNING", "https://elearning.csicr.cz");
            AMI_NOTRA("InspIS PORTAL", "https://portal.csicr.cz");
            AMI_NOTRA("InspIS SET", "https://set.csicr.cz");
            AMI_NOTRA("InspIS ŠVP", "https://svp.csicr.cz/app");
            

            DIV();
            AMI("Odhlásit se", "/Home/logout");

            return FlushResult_NAVLINKs();
        }
        public string CurrentUserMenu()
        {                
            AMI("Aktuální stránku uložit jako domovskou", "javascript:_save_as_home_page()");
            if (Factory.CurrentUser.j03HomePageUrl != null)
            {
                AMI("Vyčistit odkaz na domovskou stránku", "javascript:_clear_home_page()");                
                AMI("Tovární HOME stránka", "/Home/Index");
            }
            DIV();
            AMI("Můj profil", "/Home/MyProfile");
            AMI("Odeslat zprávu", "javascript:_sendmail()");
            AMI("Změnit přístupové heslo", "/Home/ChangePassword");
            AMI("O aplikaci", "/Home/About");
            DIV();
            AMI("Odhlásit se", "/Home/logout");

           
            return FlushResult_NAVLINKs();
        }
        public string CurrentUserLangIndex()
        {
            for (int i = 0; i <= 2; i++)
            {
                string s = "Česky";
                if (i == 1) s = "English";
                if (i == 2) s = "Українська";                
                if (Factory.CurrentUser.j03LangIndex == i) s += "&#10004;";
                AMI_NOTRA(s, string.Format("javascript: save_langindex_menu({0})", i));
            }
            return FlushResult_NAVLINKs();            
        }
        public string CurrentUserFontMenu()
        {
            
            for (int i = 1; i <= 4; i++)
            {
                string s = Factory.tra("Malé písmo");
                if (i == 2) s = Factory.tra("Výchozí velikost písma");
                if (i == 3) s = Factory.tra("Větší");
                if (i == 4) s = Factory.tra("Velké");
                if (Factory.CurrentUser.j03FontStyleFlag == i) s += "&#10004;";
                AMI_NOTRA(s, string.Format("javascript: save_fontstyle_menu({0})",i));
                
            }
          
            return FlushResult_NAVLINKs();
        }
        public string AdminMenu()
        {
            AMI("Uživatelé", "/Admin/Users");
            AMI("Číselníky", "/Admin/Ciselniky");
            if (TUP(BO.PermValueEnum.AdminGlobal_Ciselniky))
            {
                //AMI("Číselníky", "/Admin/Index");
            }
          
            if (TUP(BO.PermValueEnum.FormDesigner))
            {
                AMI("Formuláře", "/Admin/Forms");
                AMI("Návrhář formuláře", "/AdminOneForm/Index");
            }
            if (TUP(BO.PermValueEnum.WorkflowDesigner))
            {
                AMI("Workflow", "/Admin/Workflow");
                AMI("Návrhář workflow", "/AdminOneWorkflow/Index");
            }
            //if (TUP(BO.PermValueEnum.H11Admin))
            //{
            //    AMI("Nástěnka", "/Admin/NoticeBoard");
            //}

            return FlushResult_NAVLINKs();
        }
        public string AdminWorkflow(string prefix)
        {
            AMI("Workflow šablony", "/Admin/Workflow?prefix=b01");
            AMI("Šablony notifikačních zpráv", "/Admin/Workflow?prefix=b65");

            handle_selected_item(prefix);

            return FlushResult_UL();
        }
        public string AdminForms(string prefix)
        {            
            AMI("Typy formulářů", url_forms("f12"));
            AMI("Šablony formulářů", url_forms("f06"));
            AMI("PORTAL boxy", url_forms("f29"));
            AMI("Klastry otázek", url_forms("f44"));
            AMI("EVALuation funkce", url_forms("x27"));
            AMI("Jednotky odpovědí", url_forms("f21"));
            AMI("Šablony jednotek odpovědí", url_forms("f22"));

            handle_selected_item(prefix);

            return FlushResult_UL();
        }
        public string AdminUsers(string prefix)
        {
            DIV_TRANS("Správa uživatelů");
            AMI("Uživatelské účty", url_users("j03"));
            AMI("Aplikační role", url_users("j04"));
            DIV_TRANS("Osobní profily");
            AMI("Osobní profily", url_users("j02"));
            AMI("Týmy osob", url_users("j11"));
            DIV_TRANS("Provoz");
            AMI("PING Log", url_users("j92"));
            AMI("Kdo je právě online (+-2 minuty)", url_users("j92"));
            AMI("LOGIN Log", url_users("j90"));
            AMI("WORKFLOW Log", url_users("b05"));
            DIV_TRANS("Pošta");
            AMI("Poštovní účty", url_users("j40"));
            AMI("OUTBOX", url_users("x40"));

            handle_selected_item(prefix);

            return FlushResult_UL();
        }
        public string AdminCiselniky(string prefix)
        {
            DIV_TRANS("Kategorie");
            AMI("Kategorie (Skupiny položek)", url_ciselniky("o53"));
            AMI("Položky kategorií", url_ciselniky("o51"));
            
            DIV("Inspektoráty");
            AMI("Osoby v inspektorátech", url_ciselniky("a02"));
            AMI("Inspektoráty", url_ciselniky("a04"));
            AMI("Kraje", url_ciselniky("a05"));
            DIV_TRANS("Nepersonální zdroje");
            AMI("Zdroje", url_ciselniky("j23"));
            AMI("Typy zdrojů", url_ciselniky("j24"));
            AMI("Důvody rezervace", url_ciselniky("j25"));
            DIV_TRANS("Instituce");
            AMI("Pojmenované seznamy institucí", url_ciselniky("a29"));
            AMI("Typy zřizovatelů", url_ciselniky("a09"));
            AMI("Typy činností", url_ciselniky("a17"));
            AMI("Kódy vzdělávacích oborů", url_ciselniky("a18"));
            AMI("Školní informační systémy (SIS)", url_ciselniky("a70"));
            DIV_TRANS("Akce");
            AMI("Typy akcí", url_ciselniky("a10"));
            AMI("Témata akcí", url_ciselniky("a08"));



            DIV_TRANS("Úkoly/Lhůty");
            AMI("Typy úkolů a lhůt", url_ciselniky("h07"));
            DIV_TRANS("Pevné tiskové sestavy");
            AMI("Report šablony", url_ciselniky("x31"));
            AMI("Report kategorie", url_ciselniky("x32"));

            DIV_TRANS("Ostatní");
            AMI("Typy příloh", url_ciselniky("o13"));
            AMI("Šablony notifikačních zpráv", url_ciselniky("b65"));
            AMI("Články pro nástěnku", url_ciselniky("h11"));
            AMI("Dny svátků", url_ciselniky("j26"));
            AMI("AutoComplete položky", url_ciselniky("o15"));
            AMI("Uživatelská nápověda", url_ciselniky("x51"));
            AMI("Aplikační překlad", url_ciselniky("x91"));

            handle_selected_item(prefix);

            return FlushResult_UL();
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

        public string A01Menu()
        {
            if (Factory.j04UserRoleBL.IsA01Create(Factory.CurrentUser.j04ID)==true)
            {
                AMI("Nový", "/a01Create/Index");
                DIV();
            }
            AMI("Stránka", "/a01/RecPage");
            if (TUP(BO.PermValueEnum.A01Grid))
            {
                if (Factory.CBL.LoadUserParamBool("grid-a01-show11", true))
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
                DIV();
                AMI("INEZ: Grid", "/TheGrid/MasterView?prefix=a42");
                AMI("INEZ: Nový", "/a42/CreatePre");
                DIV();
                AMI("Časové kapacity inspektorů", "/a35/TimeLine","_blank");
                AMI("Rezervace nepersonálních zdrojů", "/a38/TimeLine", "_blank");                
                
            }            

            return FlushResult_NAVLINKs();
        }
        public string A03Menu()
        {
            if (TUP(BO.PermValueEnum.A03Admin)==true)
            {
                AMI("Nový", "javascript:_edit('a03',0,'Založit instituci')");
                DIV();
            }
            AMI("Stránka", "/a03/RecPage");
            if (Factory.CBL.LoadUserParamBool("grid-a03-show11", true))
            {
                AMI("Grid 1+1", "/TheGrid/MasterView?prefix=a03");
            }
            else
            {
                AMI("Grid 1", "/TheGrid/FlatView?prefix=a03");
            }           
            
            return FlushResult_NAVLINKs();
        }
        public string J02Menu()
        {
            
            AMI("Stránka", "/j02/RecPage");
            if (Factory.CBL.LoadUserParamBool("grid-j02-show11", true))
            {
                AMI("Grid 1+1", "/TheGrid/MasterView?prefix=j02");
            }
            else
            {
                AMI("Grid 1", "/TheGrid/FlatView?prefix=j02");
            }           

            return FlushResult_NAVLINKs();
        }
        public string H04Menu()
        {
            AMI("Nový", "javascript:_edit('h04',0,'Vytvořit úkol')");
            DIV();
            AMI("Stránka", "/h04/ThePage");            
            AMI("Grid 1", "/TheGrid/FlatView?prefix=h04");

            return FlushResult_NAVLINKs();
        }
        public string X31Menu()
        {
            if (TUP(BO.PermValueEnum.Menu_Reports))
            {
                AMI("Pevné tiskové sestavy", "/Home/About");
            }
            if (TUP(BO.PermValueEnum.Menu_Analyze))
            {
                AMI("Statistiky", "/Home/About");
            }

            return FlushResult_NAVLINKs();
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
                    var recA10 = Factory.a10EventTypeBL.Load(recA01.a10ID);
                    var permA01 = Factory.a01EventBL.InhalePermission(recA01);                    
                    AMI("Posunout/Doplnit", string.Format("javascript: _window_open('/workflow/Dialog?pid={0}')", pid));
                    AMI("Nahrát přílohu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    DIV();
                    if (permA01.PermValue == a01EventPermissionENUM.ShareTeam_Leader || permA01.PermValue == a01EventPermissionENUM.ShareTeam_Owner || permA01.PermValue == a01EventPermissionENUM.FullAccess)
                    {
                        if (recA10.a10IsUse_A41 && recA01.a01ParentID == 0)
                        {
                            AMI("Přidat účastníky akce", string.Format("javascript: _window_open('/a41/Append?a01id={0}')", pid));
                        }
                        if (recA10.a10IsUse_ReChangeForms)
                        {
                            AMI("Přidat formuláře", string.Format("javascript: _window_open('/a11/Append?a01id={0}')", pid));
                            AMI("Přidat skupinu formulářů", string.Format("javascript: _window_open('/a25/Record?a01id={0}')", pid));
                        }
                        if (recA10.a10IsUse_Period && recA01.a01ParentID == 0)
                        {
                            DIV();
                            AMI("Upravit časový plán akce", string.Format("javascript: _window_open('/a35/CapacityEdit?pid={0}')", pid));
                            AMI("Časové kapacity inspektorů", string.Format("javascript: _window_open('/a35/TimeLine?a05id={0}',2)", recA01.a05ID));
                            AMI("Rezervace nepersonálního zdroje", string.Format("javascript: _window_open('/a38/AppendToA01?pid={0}',2)", pid));                            
                        }
                    }
                    
                    if (recA10.a10IsUse_Poll)
                    {
                        DIV();
                        AMI("Anketní formuláře", string.Format("javascript: _window_open('/a01/Record?pid=0&a03id={0}')", pid));
                        AMI("Založit+notifikovat anketu", string.Format("javascript: _window_open('/a01/Record?pid=0&a03id={0}')", pid));                        
                    }

                    DIV();
                    AMI("Nový úkol (lhůta)", string.Format("javascript: _window_open('/h04/Record?pid=0&a01id={0}')", pid));
                    AMI("Vyhledat související akci", string.Format("javascript: _window_open('/h04/Record?pid=0&a01id={0}')", pid));

                    AMI("Kopírovat akci", string.Format("javascript: _window_open('/h04/Record?pid=0&a01id={0}')", pid));
                    AMI("Upravit základní vlastnosti (kartu)", string.Format("javascript: _window_open('/a01/Record?pid={0}')", pid));
                    AMI("Nenávratně odstranit akci", string.Format("javascript: _window_open('/a01/Record?pid={0}')", pid));
                    DIV();
                    
                    DIV();
                    AMI("Stránka akce", string.Format("javascript:_location_replace_top('/a01/RecPage?pid={0}')", recA01.pid));
                    if (recA01.isclosed == true)
                    {
                        AMI_NOTRA("<kbd>" + Factory.tra("Záznam je v archivu.") + "</kbd>", "");
                    }
                    break;
                case "a03":
                    var recA03 = Factory.a03InstitutionBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                    DIV();
                    AMI("Přidat kontaktní osobu", string.Format("javascript: _window_open('/a39/Record?pid=0&a03id={0}')", pid));
                    if (recA03.a06ID == 1)
                    {
                        AMI("Přidat činnost školy", string.Format("javascript: _window_open('/a37/Record?pid=0&a03id={0}')", pid));
                        AMI("Přidat vzdělávací obor", string.Format("javascript: _window_open('/a19/Record?pid=0&a03id={0}')", pid));
                    }
                    DIV();
                    AMI("Stránka instituce", string.Format("javascript:_location_replace_top('/a03/RecPage?pid={0}')", recA03.pid));
                    if (recA03.isclosed == true)
                    {
                        AMI_NOTRA("<kbd>"+Factory.tra("Záznam je v archivu.")+"</kbd>", "");
                    }
                    break;
                case "a11":
                    var recA11 = Factory.a11EventFormBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    DIV();
                    AMI("Otestovat vyplnění formuláře", "");
                    AMI("Zobrazit historii přístupů k šifrovaným otázkám", "");
                    AMI("Vyčistit data ve formuláři (nenávratně)", "");
                    break;
                case "a41":
                    var recA41 = Factory.a41PersonToEventBL.Load(pid);
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    DIV();
                    AMI_NOTRA(string.Format("{0}: Osobní stránka", recA41.PersonAsc), string.Format("/j02/RecPage?pid={0}", recA41.j02ID),"_top");
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
                    
                    AMI_NOTRA(string.Format(Factory.tra("Stránka akce {0}"), Factory.a01EventBL.Load(recH04.a01ID).a01Signature), string.Format("javascript:_location_replace_top('/a01/RecPage?pid={0}')", recH04.a01ID));
                    break;
                case "f06":
                    HEADER(Factory.f06FormBL.Load(pid).f06Name);
                    AMI("Návrhář formuláře", string.Format("/AdminOneForm/Index?f06id={0}", pid));
                    DIV();
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
                    AMI("Nová šachovnice otázek", string.Format("javascript: _window_open('/f25/Record?f18id={0}')", pid));
                    break;
                case "x40":
                    AMI("Detail odeslané zprávy", string.Format("javascript:_edit_full('Mail','Record',{0})",pid));
                    DIV();
                    AMI("Zkopírovat do nové zprávy", string.Format(string.Format("javascript: _window_open('/Mail/SendMail?x40id={0}')",pid)));
                    break;
                case "o27":
                    var recO27 = Factory.o27AttachmentBL.Load(pid);
                    
                    break;
                case "b01":
                    HEADER(Factory.b01WorkflowTemplateBL.Load(pid).b01Name);
                    AMI("Návrhář workflow", string.Format("/AdminOneWorkflow/Index?b01id={0}", pid));
                    DIV();
                    AMI("Nový workflow stav", string.Format("javascript: _window_open('/b02/Record?b01id={0}')", pid));
                    //AMI("Nový workflow krok", string.Format("javascript: _window_open('/b06/Record?b01id={0}')", pid));
                    DIV();
                    AMI("Upravit hlavičku workflow", string.Format("javascript:_edit('b01',{0})", pid));
                    AMI("Kopírovat workflow šablonu", string.Format("javascript:_clone('b01',{0})", pid));

                    break;

                case "j90":
                case "j92":
                case "p44":
                    AMI("Záznam bez menu nabídky", "javascript:_notify_message('nic')");
                    break; 
                default:
                    AMI("Karta záznamu", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                    AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));


                    break;
            }

            
            if (prefix == "p28" || prefix == "j02" || prefix == "p10" || prefix == "p13" || prefix == "p26" || prefix=="p27" || prefix == "p21" || prefix == "p51" || prefix == "p41" || prefix=="p11" || prefix=="p12" || prefix=="o23")
            {
                DIV();
                AMI("Info", string.Format("javascript:_window_open('/{0}/Index?pid={1}')", prefix, pid));
            }
          

            return FlushResult_UL();
        }


        private void AMI(string strName,string strUrl,string strTarget=null)
        {            
            _lis.Add(new BO.MenuItem() { Name = Factory.tra(strName), Url = strUrl,Target=strTarget });
        }
        private void AMI_NOTRA(string strName, string strUrl, string strTarget = null)
        {           
            _lis.Add(new BO.MenuItem() { Name = strName, Url = strUrl, Target = strTarget });
        }
        private void DIV(string strName=null)
        {
            _lis.Add(new BO.MenuItem() { IsDivider = true, Name = BO.BAS.OM2(strName,30) });
        }
        private void DIV_TRANS(string strName = null)
        {
            _lis.Add(new BO.MenuItem() { IsDivider = true, Name = BO.BAS.OM2(Factory.tra(strName), 30) });
        }
        private void HEADER(string strName)
        {
            _lis.Add(new BO.MenuItem() { IsHeader = true, Name = BO.BAS.OM2(strName, 100)+":" });
        }

        private string FlushResult_UL()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<ul style='border:0px;'>");
            foreach(var c in _lis)
            {                
                if (c.IsDivider==true)
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
                        if (c.IsActive == true)
                        {
                            strStyle = " style='background-color: #ADD8E6;' id='menu_active_item'";
                        }
                        if (c.Url == null)
                        {
                            sb.Append(string.Format("<li{0}>{1}</li>", strStyle, c.Name));
                        }
                        else
                        {
                            if (c.Target != null) c.Target = " target='" + c.Target + "'";
                            sb.Append(string.Format("<li{0}><a href=\"{1}\"{2}>{3}</a></li>", strStyle, c.Url,c.Target, c.Name));
                        }
                    }
                    
                }                                

            }

            sb.AppendLine("</ul>");

            return sb.ToString();
        }
        private string FlushResult_NAVLINKs()
        {
            var sb = new System.Text.StringBuilder();

            foreach (var c in _lis)
            {
                if (c.Name == null)
                {
                    sb.Append("<hr>");  //divider
                }
                else
                {
                    if (c.Url == null)
                    {
                        sb.Append(string.Format("<span>{0}</span>", c.Name));
                    }
                    else
                    {
                        if (c.Target != null) c.Target = " target='" + c.Target + "'";
                        sb.Append(string.Format("<a class='nav-link' style='color:black;' href=\"{0}\"{1}>{2}</a>", c.Url, c.Target, c.Name));
                    }
                }

            }


            return sb.ToString();
        }
    }
}