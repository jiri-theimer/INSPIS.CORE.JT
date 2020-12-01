using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum x55TypeFlagEnum
    {
        DynamicHtml = 1,
        StaticHtml = 2,
        ExternalPage = 3,
        BuildIn = 4
    }
    public class x55Widget : BaseBO
    {
        [Key]
        public int x55ID { get; set; }
        public x55TypeFlagEnum x55TypeFlag { get; set; }
        public bool x55IsSystem { get; set; }
        public string x55Name { get; set; }
        public string x55Code { get; set; }
        public int x55Ordinal { get; set; }
        public string x55Content { get; set; }
        public string x55Description { get; set; }
        public string x55TableSql { get; set; }
        public string x55TableColHeaders { get; set; }
        public string x55TableColTypes { get; set; }
        public string x55Image { get; set; }

    }
}
