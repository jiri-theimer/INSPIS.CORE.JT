using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b65WorkflowMessage:BaseBO
    {
        [Key]
        public int b65ID { get; set; }
        public int x29ID { get; set; }
        public string b65Name { get; set; }
        public string b65MessageSubject { get; set; }
        public string b65MessageBody { get; set; }

        public string x29Name { get; set; } //combo
    }
}
