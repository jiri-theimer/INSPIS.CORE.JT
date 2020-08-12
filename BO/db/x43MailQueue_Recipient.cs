using System.ComponentModel.DataAnnotations;
namespace BO
{
    public class x43MailQueue_Recipient
    {
        [Key]
        public int x43ID { get; set; }
        public int x40ID { get; set; }
        public string x43DisplayName { get; set; }
        public string x43Email { get; set; }
        public int x43RecipientType { get; set; }
        public int x29ID { get; set; }
        public int x43DataPID { get; set; }

        public int TempA03ID { get; set; }  //kvůli INEZ
    }
}
