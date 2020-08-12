using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j24NonPersonType:BaseBO
    {
        [Key]
        public int j24ID { get; set; }
        public string j24Name { get; set; }
        public bool j24IsDriver { get; set; }
    }
}
