using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f25ChessBoard:BaseBO
    {
        [Key]
        public int f25ID { get; set; }
        public int f18ID { get; set; }
        public int f19ID_TemplateID { get; set; }
        public string f25Name { get; set; }
        public int f25ColumnCount { get; set; }
        public int f25RowCount { get; set; }
        public string f25RowHeaders { get; set; }
        public string f25ColumnHeaders { get; set; }
        public string f25UC { get; set; }
        public string f25Description { get; set; }

        public string f18Name { get; set; } //combo
    }
}
