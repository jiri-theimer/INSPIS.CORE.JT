using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAParser;


namespace UI.Models
{
    public class MyProfileViewModel:BaseViewModel
    {
        public BO.j02Person RecJ02;
        public BO.j03User RecJ03;
        public BO.RunningUser CurrentUser;


        public string EmailAddres { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public bool IsGridClipboard { get; set; }
        public string userAgent { get; set; }
        public ClientInfo client_info { get; set; }

        
    }
}
