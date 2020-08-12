using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class h07ToDoType:BaseBO
    {
        [Key]
        public int h07ID { get; set; }
        public string h07Name { get; set; }
        public bool h07IsDefault { get; set; }
        public bool h07IsToDo { get; set; }
        public bool h07IsCapacityPlan { get; set; }
        public string h07Description { get; set; }
        public int b65ID { get; set; }
        public string b65Name { get; set; } //combo
    }
}
