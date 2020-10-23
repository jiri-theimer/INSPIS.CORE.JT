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

    }
}
