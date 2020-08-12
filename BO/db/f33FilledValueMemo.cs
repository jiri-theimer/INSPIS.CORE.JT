using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f33FilledValueMemo
    {
        [Key]
        public int f33ID { get; set; }
        public int f32ID { get; set; }
        public string f33Value { get; set; }
    }
}
