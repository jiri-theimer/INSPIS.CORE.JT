using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j90LoginAccessLog
    {
        [Key]
        public int j90ID;
        public int j03ID;
        public DateTime j90Date;
        public string j90ClientBrowser;
        public string j90Platform;
        public string j90AppClient;

        public string j90BrowserFamily;
        public string j90BrowserOS;
        public string j90BrowserDeviceType;
        public string j90BrowserDeviceFamily;
        public int j90BrowserAvailWidth;
        public int j90BrowserAvailHeight;
        public int j90BrowserInnerWidth;
        public int j90BrowserInnerHeight;        
        public string j90LocationHost;
        public string j90LoginMessage;
        public string j90LoginName;
        public int j90CookieExpiresInHours;

    }
}
