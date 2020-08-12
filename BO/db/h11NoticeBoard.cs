using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class h11NoticeBoard:BaseBO
    {
        [Key]
        public int h11ID { get; set; }
        public int o11ID { get; set; }
        public int j02ID_Creator { get; set; }
        
        public string h11Name { get; set; }
        public bool h11IsPublic { get; set; }
    }
}
