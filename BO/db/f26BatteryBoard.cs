using System.ComponentModel.DataAnnotations;
namespace BO
{
    public class f26BatteryBoard:BaseBO
    {
        [Key]
        public int f26ID { get; set; }
        public int f18ID { get; set; }
        public string f26Name { get; set; }
        public int f26Ordinal { get; set; }
        public string f26ColumnHeaders { get; set; }
        public string f26SupportingText { get; set; }
        public string f26Hint { get; set; }
        public string f26UC { get; set; }
        public string f26Description { get; set; }

        public string f18Name { get; set; } //combo
        public int f06ID;
    }
}
