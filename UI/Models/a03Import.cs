using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a03Import: BaseViewModel
    {
        public BO.j75ImportTemplate Rec { get; set; }
        public string FileFullPath { get; set; }

        public string SelectedSheet { get; set; }

        public List<BO.StringPair> Sheets { get; set; }

        public List<ImportMappingColumn> MapCols { get; set; }
        public int HeadersRow { get; set; }

        public string SelectedJ75ID { get; set; }
        public IEnumerable<BO.j75ImportTemplate> lisJ75 { get; set; }

        public int SelectedA06ID { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }

    }
}
