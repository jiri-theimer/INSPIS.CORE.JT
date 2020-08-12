using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j26Holiday: BaseBO
    {
        [Key]
        public int j26ID { get; set; }
        public string j26Name { get; set; }
        public DateTime j26Date { get; set; }
    }
}
