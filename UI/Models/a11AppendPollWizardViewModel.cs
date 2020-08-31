using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a11AppendPollWizardViewModel: a11AppendViewModel
    {
        public string AccessToken { get; set; }
        
        public int SelectedB65ID { get; set; }
        public string SelectedB65Name { get; set; }

        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }

        public string EmailAddress { get; set; }

        public string SelectedEmail { get; set; }
        public List<BO.StringPair> lisEmails { get; set; }
    }
}
