using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class StatViewModel:BaseViewModel
    {
        public int ActiveTabIndex { get; set; } = 1;
        public string f06IDs { get; set; }
        public IEnumerable<BO.f06Form> lisF06 { get; set; }
        public IEnumerable<BO.f18FormSegment> lisF18 { get; set; }
        public IEnumerable<BO.f19Question> lisF19 { get; set; }

        public string f19IDs { get; set; }
        public string CheckedIDs { get; set; }
       
        public bool IsZeroRow { get; set; }     //V prvním XLS řádku názvy otázek
        public bool IsBlankA11IDs { get; set; }     //Zahrnout i formuláře bez odpovědí

        public BO.StatValueMode ValuesMode { get; set; }
        public BO.StatGroupByMode GroupByMode { get; set; }

        public PeriodViewModel PeriodFilter { get; set; }

        public string guid { get; set; }

        public string XlsExportTempFileName { get; set; }
        public string GridGuid { get; set; }
        public string GridColumns { get; set; }
        public string GridHeaders { get; set; }
        public string GridContainerCssStyle { get; set; }

        public string SelectedJ72ID { get; set; }
        public IEnumerable<BO.j72TheGridTemplate> lisJ72 { get; set; }

        public string TreeContainerHeight { get; set; }
    }
}
