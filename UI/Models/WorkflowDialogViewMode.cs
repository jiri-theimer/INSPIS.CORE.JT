using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class WorkflowDialogViewMode:BaseViewModel
    {
        public BO.a01Event RecA01 { get; set; }
        public BO.b06WorkflowStep RecB06 { get; set; }
        public BO.a01EventPermission PermA01 { get; set; }
        public IEnumerable<BO.a41PersonToEvent> lisA41 { get; set; }
        public IEnumerable<BO.p85Tempbox> lisTemp { get; set; }

        public int pid { get; set; }

        public List<BO.b06WorkflowStep> lisB06 { get; set; }

        public int SelectedB06ID { get; set; }

        public string Comment { get; set; }
        public string UploadGuid { get; set; }
        public string TempGuid { get; set; }

        public int SelectedNomineeJ02ID { get; set; }
        public string SelectedNomineePerson { get; set; }

        public int SelectedNomineeJ11ID { get; set; }
        public string SelectedNomineeTeam { get; set; }

        public string FormValidationErrorMessage { get; set; }

        public List<BO.a45EventRole> lisCommentTo { get; set; }

    }
}
