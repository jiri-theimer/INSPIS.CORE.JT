using System;
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
        public string AppRootFolder { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string AppBuild { get; set; }
        public bool FulltextSearch { get; set; }
        public string LogoImageSuffix { get; set; }
        public string CssCustomSkin { get; set; }
       public string UserUrl { get; set; }
        public string TranslatorMode { get; set; }

        public int DefaultLangIndex { get; set; }
        public bool LangChooseIsStopped { get; set; }   //true: bez nabídky volby jazyku

        public string Implementation { get; set; }  //UA/HD/Default

        public string UiftUrl { get; set; } //url pro spouštění UIFT
        public string RobotUser { get; set; }   //pod jakým uživatelským loginem běží robot na pozadí
        public bool RobotIsStopped { get; set; }    //true: běh robota je zastaven
       
        public bool PasswordRequireDigit { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public int PasswordMinLength { get; set; }
        public int PasswordMaxLength { get; set; }
        public bool PipeIsMembershipProvider { get; set; }
        public string PipeBaseUrl { get; set; }     //příklad: https://tinspiscore.csicr.cz/pipe
        public string GinisExportDocTypes { get; set; }

    }
}
