using System.ComponentModel.DataAnnotations;



namespace BO
{
    public class f29PortalQuestionTab:BaseBO
    {
        [Key]
        public int f29ID { get; set; }
        public string f29Name { get; set; }
        public string f29Description { get; set; }
        public int f29Ordinal { get; set; }
        public bool f29IsSeparatePortalBox { get; set; }
        
    }
}
