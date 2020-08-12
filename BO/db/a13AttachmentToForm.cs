using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a13AttachmentToForm:BaseBO
    {
        [Key]
        public int a13ID { get; set; }
        public int f06ID { get; set; }
        public int o13ID { get; set; }
        public bool a13IsEditable { get; set; }
        public bool a13IsRequired { get; set; }
        public int a13Ordinal { get; set; }

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

        public string o13Name { get; set; } //combo

    }
}
