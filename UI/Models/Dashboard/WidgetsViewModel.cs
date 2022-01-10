using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Dashboard
{
    public class WidgetsViewModel: BaseViewModel
    {
        public BO.j02Person Rec { get; set; }
        public string Skin { get; set; }
        public bool IsSubform { get; set; }     //true: jedná se o podformulář
        public string BoxColCss { get; set; } = "col-lg-6";

        public WidgetsEnvironment DockStructure { get; set; }

        public BO.x56WidgetBinding recX56 { get; set; }

        public IEnumerable<BO.x55Widget> lisAllWidgets { get; set; }
        public List<BO.x55Widget> lisUserWidgets { get; set; }
        public int ColumnsPerPage { get; set; }
        public bool IsCharts { get; set; }
        public bool IsDataTables { get; set; }
        public string DataTables_Localisation { get; set; }

        public bool IsPdfButtons { get; set; }
        public bool IsExportButtons { get; set; }
        public bool IsPrintButton { get; set; }

    }
}
