using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class x27EvalFunction:BaseBO
    {
        [Key]
        public int x27ID { get; set; }
        public string x27Name { get; set; }
        public string x27Description { get; set; }
        public int x27Ordinal { get; set; }
        public string x27Parameters { get; set; }
        public string x27Returns { get; set; }
    }
}
