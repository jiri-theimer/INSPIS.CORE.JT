using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models.Dashboard;

namespace UI.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public string Pozdrav { get; set; }
        public string Pandulak1 { get; set; }
        public string Pandulak2 { get; set; }

        public string BoxColClass { get; set; } = "col-lg-6";
        
        public DockStructure DockStructure { get; set; }

        public BO.x56WidgetBinding recX56 { get; set; }

        public IEnumerable<BO.x55Widget> lisAllWidgets { get; set; }
        public List<BO.x55Widget> lisUserWidgets { get; set; }
        public int ColumnsPerPage { get; set; } = 2;
           

    }
}
