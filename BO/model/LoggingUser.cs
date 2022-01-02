using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Reflection.Metadata;

namespace BO
{
    public class LoggingUser
    {
        
        public string Login { get; set; }        
        public string Password { get; set; }
        public int CookieExpiresInHours { get; set; } = 1;
        public int LangIndex { get; set; }
        public bool IsChangedLangIndex { get; set; }

        public string Message { get; set; }
        public string AppClient { get; set; }

        public string Browser_UserAgent { get; set; }
        public int Browser_AvailWidth { get; set; }
        public int Browser_AvailHeight { get; set; }
        public int Browser_InnerWidth { get; set; }
        public int Browser_InnerHeight { get; set; }
        public string Browser_DeviceType { get; set; }
        public string Browser_Host { get; set; }

        public string ReturnUrl { get; set; }

        
    }
}
