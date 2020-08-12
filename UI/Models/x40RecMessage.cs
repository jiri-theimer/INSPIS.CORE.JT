using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;

namespace UI.Models
{
    public class x40RecMessage:BaseViewModel
    {
        public BO.x40MailQueue Rec { get; set; }

        public MimeMessage MimeMessage { get; set; }

        public List<BO.StringPair> MimeAttachments { get; set; }
    }
}
