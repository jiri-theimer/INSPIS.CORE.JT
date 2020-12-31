using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridDesignerViewModel:BaseViewModel
    {
        public BO.j72TheGridTemplate Rec { get; set; }

        public List<BO.TheGridColumn> SelectedColumns;


        public List<BO.EntityRelation> Relations;
        public List<BO.TheGridColumn> AllColumns;

        public List<UI.Models.kendoTreeItem> treeNodes { get; set; }

        public List<BO.j73TheGridQuery> lisJ73 { get; set; }

        public List<BO.TheQueryField> lisQueryFields { get; set; }

        public List<BO.ThePeriod> lisPeriods { get; set; }

        public bool HasOwnerPermissions { get; set; }

        public string j04IDs { get; set; }
        public string j04Names { get; set; }

        public string j11IDs { get; set; }
        public string j11Names { get; set; }

    }
}
