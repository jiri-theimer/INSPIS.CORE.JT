using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum x31ReportFormatEnum
    {
        Telerik = 1,
        DOC = 2,
        XLS = 3,
        MSREPORTING = 4
    }
    public class x31Report : BaseBO
    {
        [Key]
        public int x31ID { get; set; }
        public int x29ID { get; set; }
        public string x31Name { get; set; }
        public string x31Description { get; set; }
        public string x31PID { get; set; }

        public x31ReportFormatEnum x31ReportFormat { get; set; }

        public bool x31Is4SingleRecord { get; set; }

        public string x31MSReporting_ReportServerUrl { get; set; }
        public string x31MSReporting_ReportPath { get; set; }

        public string x29Name { get; set; } //combo
        public string x31DocSqlSource { get; set; }
        public string x31Translate { get; set; }
    }
}
