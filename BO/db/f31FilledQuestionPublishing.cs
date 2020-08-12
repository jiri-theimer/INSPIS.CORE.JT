
using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class f31FilledQuestionPublishing:BaseBO
    {
        [Key]
        public int f31ID { get; set; }
        public int a11ID { get; set; }
        public int f19ID { get; set; }
        public int f25ID { get; set; }
        public int f26ID { get; set; }
        public bool f31IsPublished { get; set; }
    }
}
