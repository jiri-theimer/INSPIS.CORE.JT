using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b07WorkflowMessageToStatus
    {
        [Key]
        public int b07ID { get; set; }
        public int b02ID { get; set; }
        public int b65ID { get; set; }
        public int a45ID { get; set; }
        public int j04ID { get; set; }
        public int j11ID { get; set; }

        public string b65Name { get; set; } //combo
        public string a45Name { get; set; } //combo
        public string j04Name { get; set; } //combo
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
                    return "display:flex";
                }
            }
        }
    }
}
