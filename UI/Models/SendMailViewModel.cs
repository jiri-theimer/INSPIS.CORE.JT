using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class SendMailViewModel:BaseViewModel
    {
        public BO.x40MailQueue Rec { get; set; }
        public int b65ID { get; set; }
        public string b65Name { get; set; } //combo

        public string UploadGuid { get; set; }
        
        public int ActiveTabIndex { get; set; } = 1;

        public int FilterA03ID { get; set; }
        public string FilterA03Name { get; set; }
        public int FilterA29ID { get; set; }
        public string FilterA29Name { get; set; }
        public string a03IDs { get; set; }

        public bool IsTest { get; set; }
    }
}
