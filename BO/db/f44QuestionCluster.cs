using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f44QuestionCluster:BaseBO
    {
        [Key]
        public int f44ID { get; set; }
        public string f44Code { get; set; }
        public string f44Name { get; set; }
        public int f44Ordinal { get; set; }
    }
}
