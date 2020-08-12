using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class b01WorkflowTemplate:BaseBO
    {
        [Key]
        public int b01ID { get; set; }
        public string b01Name { get; set; }
        public string b01Ident { get; set; }
        public string b01UC { get; set; }
        
    }
}
