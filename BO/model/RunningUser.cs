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
        

        public bool j03IsMustChangePassword { get; set; }
        public int j03FontStyleFlag { get; set; }
    
        public int j03GridSelectionModeFlag { get; set; }
        public DateTime? j03LiveChatTimestamp { get; set; }
        public DateTime? j03PingTimestamp { get; set; }

        public int j03LangIndex { get; set; }
        public string j03HomePageUrl { get; set; }
        public int a04ID { get; set; }      //svázaný inspektorát
        public int a05ID { get; set; }      //svázaný region
        public string j11IDs_Cache { get; set; }    //kvůli výkonu SQL - seznam j11ID týmů s účastní uživatele

        public List<BO.StringPair> Messages4Notify { get; set; }

        private bool _WasInitTesting;
        private bool _IsGlobalAdmin;

        public void AddMessage(string strMessage,string strTemplate="error")
        {
            if (Messages4Notify == null) { Messages4Notify = new List<BO.StringPair>(); };
            Messages4Notify.Add(new BO.StringPair() { Key = strTemplate, Value = strMessage }); ;
        }

        public string getFontStyle()
        {
            switch (this.j03FontStyleFlag)
            {
                case 1:
                    return "font: 0.6875rem/1.0 var(--font-family-sans-serif)";
                case 2:
                    return "font: 0.75rem/1.0 var(--font-family-sans-serif)";
                case 3:
                    return "font: 0.85rem/1.0 var(--font-family-sans-serif)";
                case 4:
                    return "font: 1rem/1.2 var(--font-family-sans-serif);";
                default:
                    return "font: 0.75rem/1.0 var(--font-family-sans-serif)";
            }
        }

        public bool TestPermission(BO.j05PermValuEnum oneperm)
        {
            if (_WasInitTesting == false)
            {
                if (this.j04RoleValue.Substring(0, 1) == "1")   //globální admin - první oprávnění
                {
                    _IsGlobalAdmin = true;
                }
                _WasInitTesting = true;
            }
            int x = (int)oneperm;
            if (this.j04RoleValue.Substring(x-1, 1) == "1") //testuje se 1 nebo 0 ve stringu j04RoleValue na pozici x-1
            {
                return true;
            }

            return false;  //i globální admin musí mít oprávnění oneperm zaškrtnuté
            
            
           
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
                default:
                    return "/Home/Index";


            }
        }

    }
}
