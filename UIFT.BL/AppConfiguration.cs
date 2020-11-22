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

        public string ThemeCss { get; set; }

        public Auth Authentication { get; set; }

        public string BaseURL_EPIS1 { get; set; }

        public string FT_CheckboxAnswerTrueValue { get; set; }

        public string FT_CheckboxAnswerFalseValue { get; set; }

        public string UIFT_DateFormat { get; set; }

        public string UIFT_DateTimeFormat { get; set; }

        public class Auth
        {
            public string KeyPath { get; set; }

            public string AppName { get; set; }

            public string CookieName { get; set; }

            public string Domain { get; set; }
        }
    }
}
