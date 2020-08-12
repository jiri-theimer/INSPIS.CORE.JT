using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j25NonPersonPlanReason : BaseBO
    {
        [Key]
        public int j25ID { get; set; }
        public string j25Name { get; set; }
        
    }
}
