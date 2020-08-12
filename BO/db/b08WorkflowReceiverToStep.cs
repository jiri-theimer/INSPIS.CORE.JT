using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b08WorkflowReceiverToStep
    {
        [Key]
        public int b08ID { get; set; }
        public int b06ID { get; set; }
       
        public int a45ID { get; set; }
        
        public int j11ID { get; set; }
        public int j04ID { get; set; }
        public int b06ID_NomineeSource { get; set; }



        public string a45Name;
        public string j04Name;
        public string j11Name;

       
    }
}
