using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b12WorkflowReceiverToHistory
    {
        [Key]
        public int b12ID { get; set; }
        public int b06ID { get; set; }
        public int a45ID { get; set; }
        public int b06ID_NomineeSource { get; set; }
        public int j04ID { get; set; }
    }
}
