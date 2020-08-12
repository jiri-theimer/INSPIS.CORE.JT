using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f21ReplyUnit:BaseBO
    {
        [Key]
        public int f21ID { get; set; }
        public string f21Name { get; set; }
        public string f21Description { get; set; }
        public string f21UC { get; set; }
        public string f21ExportValue { get; set; }
        public string f21MinValue { get; set; }
        public string f21MaxValue { get; set; }
        public bool f21IsCommentAllowed { get; set; }
        public bool f21IsNegation { get; set; }
        public int f21Ordinal { get; set; }
        public int f21SystemFlag { get; set; }
    }
}
