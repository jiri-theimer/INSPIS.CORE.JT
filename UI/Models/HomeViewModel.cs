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

        public string BoxColCss { get; set; } = "col-lg-6";
        
        public WidgetsEnvironment DockStructure { get; set; }

        public BO.x56WidgetBinding recX56 { get; set; }

        public IEnumerable<BO.x55Widget> lisAllWidgets { get; set; }
        public List<BO.x55Widget> lisUserWidgets { get; set; }
        public int ColumnsPerPage { get; set; }

        public DateTime DateToday { get; set; } = DateTime.Today;

        public string FormatSnippetHeader(BO.x55Widget c, DateTime datToday)
        {
            if (datToday == DateTime.Today)
            {
                return c.x55Name;
            }
            if (c.x55Name.Contains("dnes"))
            {
                return c.x55Name.Replace("DNES", BO.BAS.ObjectDate2String(datToday));
            }
            else
            {
                return c.x55Name;
            }
        }
    }
}
