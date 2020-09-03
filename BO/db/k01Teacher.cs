using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class k01Teacher:BaseBO
    {
        [Key]
        public int k01ID { get; set; }
        public int j02ID { get; set; }

        public string k01PID { get; set; }

        public string k01FirstName { get; set; }
        public string k01LastName { get; set; }
        public string k01TitleBeforeName { get; set; }
        public string k01TitleAfterName { get; set; }

    }
}
