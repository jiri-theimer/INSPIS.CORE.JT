using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class a57AutoEvaluation:BaseBO
    {
        [Key]
        public int a57ID { get; set; }
        public int a08ID { get; set; }
        public int a10ID { get; set; }
        public string a08Name { get; }
        public string a10Name { get; }
        public string a57Name { get; set; }
        public string a57Description { get; set; }
    }
}
