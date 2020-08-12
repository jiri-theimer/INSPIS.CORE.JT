using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a08Theme:BaseBO
    {
        [Key]
        public int a08ID { get; set; }

        public string a08Name { get; set; }
        public string a08Ident { get; set; }
        public string a08Description { get; set; }

        public string f06Name;

    }
}
