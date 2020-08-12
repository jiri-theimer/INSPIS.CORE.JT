using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum MailUsageFlag
    {
        SmtpPrivate=1,
        SmtpGlobal=2,
        ImapPrivate=3,
        ImapGlobal=4
    }
    public class j40MailAccount:BaseBO
    {
        [Key]
        public int j40ID { get; set; }
        public int j02ID { get; set; }
        public MailUsageFlag j40UsageFlag { get; set; }
        public string j40SmtpHost { get; set; }
        public int j40SmtpPort { get; set; }
        public string j40SmtpName { get; set; }
        public string j40SmtpEmail { get; set; }
        public bool j40SmtpUsePersonalReply { get; set; } = true;
        public string j40SmtpLogin { get; set; }
        public string j40SmtpPassword { get; set; }
        public bool j40SmtpUseDefaultCredentials { get; set; }
        public bool j40SmtpEnableSsl { get; set; }
        public string j40ImapHost { get; set; }
        public string j40ImapLogin { get; set; }
        public string j40ImapPassword { get; set; }
        public int j40ImapPort { get; set; }

        public string Person { get; set; } //get+set: kvůli mycombo
    }
}
