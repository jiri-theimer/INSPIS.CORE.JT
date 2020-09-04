using System;

namespace UIFT
{
    public class AppConfiguration
    {
        public string GA { get; set; }

        public string UploadFolder { get; set; }

        public string TempFolder { get; set; }

        public string LogFolder { get; set; }

        public int MaxFileUploadSize { get; set; }

        public string BaseUrl { get; set; }

        public int DefaultLanguage { get; set; }

        public Auth Authentication { get; set; }

        public class Auth
        {
            public string KeyPath { get; set; }

            public string AppName { get; set; }

            public string CookieName { get; set; }

            public string Domain { get; set; }
        }
    }
}
