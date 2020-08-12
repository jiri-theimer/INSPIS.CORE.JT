using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b13WorkflowRequiredFormsToStep
    {
        [Key]
        public int b13ID { get; set; }
        public int b06ID { get; set; }
        public int f06ID { get; set; }
        public string f06Name { get; set; }

        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:flex";
                }
            }
        }
    }
}
