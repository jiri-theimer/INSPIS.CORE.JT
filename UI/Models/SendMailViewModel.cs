using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class SendMailViewModel:BaseViewModel
    {
        public BO.x40MailQueue Rec { get; set; }

        public string UploadGuid { get; set; }

        public int ActiveTabIndex { get; set; } = 1;
    }
}
