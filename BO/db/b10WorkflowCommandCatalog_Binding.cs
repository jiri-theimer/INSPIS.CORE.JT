using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b10WorkflowCommandCatalog_Binding
    {
        [Key]
        public int b10ID { get; set; }
        public int b09ID { get; set; }
        public int b06ID { get; set; }
        public int b02ID { get; set; }
        public string b10Parameter1 { get; set; }
        public string b10Parameter2 { get; set; }
        public string b10Parameter3 { get; set; }

        public string b09Name { get; set; } //combo
        public int b09ParametersCount { get; set; }

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
                    return "display:table-row";
                }
            }
        }
    }
}
