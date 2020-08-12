using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b03WorkflowReceiverToStatus
    {
        [Key]
        public int b03ID { get; set; }
        public int b02ID { get; set; }
        public int a45ID { get; set; }
        public int j11ID { get; set; }
        public string a45Name { get; set; } //combo
        public string j11Name { get; set; } //combo

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
