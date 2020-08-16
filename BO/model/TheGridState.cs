

namespace BO
{
    public class TheGridState:j72TheGridTemplate
    {
        public int j75ID { get; set; }
        public string j75SortDataField { get; set; }
        public string j75SortOrder { get; set; }
        public int j75PageSize { get; set; } = 100;
        public int j75CurrentPagerIndex { get; set; }
        public int j75CurrentRecordPid { get; set; }

        public string j75Filter { get; set; }
        public string j75ColumnsGridWidth { get; set; }
        public string j75ColumnsReportWidth { get; set; }
        public int j75HeightPanel1 { get; set; }
    }
}
