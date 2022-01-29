using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class QueryBuilderViewModel:BaseViewModel
    {
        public int SelectedJ76ID { get; set; }
        public string Entity { get; set; }
        public bool IsCreate { get; set; }
        public IEnumerable<BO.j76NamedQuery> lisJ76 { get; set; }
        public BO.j76NamedQuery Rec { get; set; }
        public List<BO.j73TheGridQuery> lisJ73 { get; set; }

        public List<BO.TheQueryField> lisQueryFields { get; set; }

        public bool HasOwnerPermissions { get; set; }

        public List<BO.ThePeriod> lisPeriods { get; set; }
    }
}
