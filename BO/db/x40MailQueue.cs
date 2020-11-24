using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BO
{

    public enum x40StateFlag
    {
        NotSpecified = 0,
        InQueque = 1,
        Error = 2,
        Proceeded = 3,
        Stopped = 4,
        WaitOnConfirm = 5
    }
    public enum RecipientFlagEnum
    {
        _None=0,
        SchoolAdress=1,
        DirectorAddress=2,
        SchoolPlusDirector=3
    }
    public class x40MailQueue : BaseBO
    {
        [Key]
        public int x40ID { get; set; }
        public int j40ID { get; set; }
        public int x29ID { get; set; }
        public int p40ID { get; set; }
        public int j03ID_Creator { get; set; }
       
        public string x40MessageGuid { get; set; }
        public string x40BatchGuid { get; set; }
        public int x40MailID { get; set; }
        public string x40AttachmentsGuid { get; set; }
        public string x40EmlFolder { get; set; }
        public int x40EmlFileSize { get; set; }
        public x40StateFlag x40Status { get; set; }
        public int x40DataPID { get; set; }
        public string x40Entity { get; set; }
        public string x40SenderAddress { get; set; }
        public string x40SenderName { get; set; }
        public string x40Recipient { get; set; }
        public RecipientFlagEnum x40RecipientFlag { get; set; }
        public string x40Bcc { get; set; }
        public string x40Cc { get; set; }
        public string x40Attachments { get; set; }
        public string x40Subject { get; set; }
        public string x40Body { get; set; }
        public bool x40IsHtmlBody { get; set; }
        public DateTime? x40DatetimeProcessed { get; set; }
        public bool x40IsProcessed { get; set; }
        public string x40ErrorMessage { get; set; }
        public bool x40IsAutoNotification { get; set; }
        public string j40Name { get; set; }

        public DateTime MessageTime;

        public string StateAlias
        {
            get
            {

                if (this.x40Status == x40StateFlag.Error) return "Chyba";
                if (this.x40Status == x40StateFlag.Proceeded) return "Odesláno";

                return this.x40Status.ToString();
            }
        }
    }
}
