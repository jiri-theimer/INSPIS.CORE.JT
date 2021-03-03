using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class RunningUser
    {
        public int pid { get; set; }
        public int j02ID { get; set; }
       
        public bool isclosed { get; set; }
        public string j03Login { get; set; }
        public string j02Email { get; set; }
        
        public string FullName { get; set; }
        public string j04Name { get; set; }
        public int j04ID { get; set; }
        public string j04RoleValue { get;set;}
        public string j03AdminRoleValue { get; set; }
        public bool j04IsAllowedAllEventTypes { get; set; }
        public j04RelationFlagEnum j04RelationFlag { get; set; }

        public bool j03IsMustChangePassword { get; set; }
        public int j03FontStyleFlag { get; set; }
    
        public int j03GridSelectionModeFlag { get; set; }
        public DateTime? j03LiveChatTimestamp { get; set; }
        public DateTime? j03PingTimestamp { get; set; }

        public int j03LangIndex { get; set; }
        public bool j03IsMainLogoVisible { get; set; }  //zda je zapnutý pruh s logem nad hlavním menu
        public bool j03IsDebugLog { get; set; }     //zda je zapnuté logování uživatelovo SQL dotazů
        public string j03HomePageUrl { get; set; }
        public string j04ViewUrl_Page { get; set; }
        public int a04ID { get; set; }      //svázaný inspektorát
        public int a05ID { get; set; }      //svázaný region
        public string j11IDs_Cache { get; set; }    //kvůli výkonu SQL - seznam j11ID týmů s účastní uživatele
        public string a10IDs { get; set; }      //výčet všech povolených typů akcí z tabulky j08

        public string AppImplementation { get; set; }   //název implementace předaný z RunningApp
        public bool FullTextSearch { get; set; }    //zda používat FULLTEXT search předaný z RunningApp
        public List<BO.StringPair> Messages4Notify { get; set; }

        private bool _WasInitTesting;
        private bool _IsGlobalAdmin;

        public void AddMessage(string strMessage,string strTemplate="error")
        {
            if (Messages4Notify == null) { Messages4Notify = new List<BO.StringPair>(); };
            Messages4Notify.Add(new BO.StringPair() { Key = strTemplate, Value = strMessage }); ;
        }

       
        public string getFontSizeCss()
        {

            switch (this.j03FontStyleFlag)
            {
                case 1:
                    return "fontsize1.css";     //malé písmo               
                case 2:
                    return "fontsize2.css";       //výchozí písmo                
                case 3:
                    return "fontsize3.css";        //větší písmo       
                case 4:
                    return "fontsize4.css";         //největší písmo
                default:
                    return "fontsize2.css";         //výchozí písmo
            }
           
        }

        public bool TestPermission(BO.j05PermValuEnum oneperm, string strRoleValue=null)
        {
            if (strRoleValue == null)
            {
                strRoleValue = this.j04RoleValue;
            }
            if (_WasInitTesting == false)
            {
                if (strRoleValue.Substring(0, 1) == "1")   //globální admin - první oprávnění
                {
                    _IsGlobalAdmin = true;
                }
                _WasInitTesting = true;
            }
            int x = (int)oneperm;
            if (strRoleValue.Substring(x-1, 1) == "1") //testuje se 1 nebo 0 ve stringu j04RoleValue na pozici x-1
            {
                return true;
            }

            return false;  //i globální admin musí mít oprávnění oneperm zaškrtnuté
                                   
        }
        public bool TestPermCiselniky(BO.j03AdminRoleValueFlagEnum oneperm_re, BO.j03AdminRoleValueFlagEnum oneperm_ro)
        {
            if (this.j03AdminRoleValue == null || TestPermission(j05PermValuEnum.AdminGlobal_Ciselniky)==false)
            {
                return false;
            }
            int x = (int)oneperm_re;            
            if (x>0 && this.j03AdminRoleValue.Substring(x - 1, 1) == "1") //testuje se 1 nebo 0 ve stringu j03AdminRoleValue na pozici x-1
            {
                return true;
            }

            x = (int)oneperm_ro;
            if (x>0 && this.j03AdminRoleValue.Substring(x - 1, 1) == "1") //testuje se 1 nebo 0 ve stringu j03AdminRoleValue na pozici x-1
            {
                return true;
            }

            return false;
        }
        public bool HasAdminMenu()
        {
        
            if (_IsGlobalAdmin || this.j04RoleValue.Substring(0, 3).Contains("1")) //    AdminGlobal = 1, FormDesigner = 2, WorkflowDesigner = 3,
            {
                return true;
            }
            if (TestPermission(j05PermValuEnum.H11Admin)) return true;
            if (TestPermission(j05PermValuEnum.AdminGlobal_Ciselniky)) return true;

            return false;


        }
        public string LangName
        {
            get
            {
                switch (this.j03LangIndex)
                {
                    case 1:
                        return "EN";
                    case 2:
                        return "UA";
                    default:
                        return "CZ";
                }
            }
        }

        public string getHomePageUrl()
        {
            
            switch (this.j03HomePageUrl)
            {
                case "j02_framework_inspector.aspx":
                    return "/Dashboard/Inspector";                    
                case "j02_framework_institution_general.aspx":
                    return "/Dashboard/School";
                case "j02_framework_administrator.aspx":
                    return "/Dashboard/Widgets?skin=index";
                case null:
                    return "/Dashboard/Widgets";
                default:
                    return this.j03HomePageUrl;


            }
        }

    }
}
