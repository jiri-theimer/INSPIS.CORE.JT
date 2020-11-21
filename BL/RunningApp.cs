﻿using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.Extensions.Logging;

namespace BL
{
    public class RunningApp
    {

        public string ConnectString { get; set; }
        public string UploadFolder { get; set; }
        public string TempFolder { get; set; }
        public string LogFolder { get; set; }
        public string ReportFolder { get; set; }

        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string AppBuild { get; set; }
        public string LogoImage { get; set; }
        public string LoginImage { get; set; }
        public string TranslatorMode { get; set; }

        public int DefaultLangIndex { get; set; }

        public string Implementation { get; set; }  //HD nebo prázdno

        public string UiftUrl { get; set; } //url pro spouštění UIFT

        public string Terminology_Akce
        {
            get
            {
                if (this.Implementation == "HD")
                {
                    return "Požadavky";
                }
                return "Akce";
            }
        }

        public bool PasswordRequireDigit { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public int PasswordMinLength { get; set; }
        public int PasswordMaxLength { get; set; }

    }
}
