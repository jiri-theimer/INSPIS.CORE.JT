using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b09WorkflowCommandCatalog
    {
        [Key]
        public int b09ID { get; set; }
        public string b09Name { get; set; }
        public string b09Ident { get; set; }
        public int b09Order { get; set; }
        public int b09ParametersCount { get; set; }
        public string b09Hint { get; set; }
        public string b09SQL { get; set; }
    }
}
