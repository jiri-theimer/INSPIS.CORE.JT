using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a12ThemeForm:BaseBO
    {
        [Key]
        public int a12ID { get; set; }
        public int a08ID { get; set; }
        public int f06ID { get; set; }
        public bool a12IsRequired { get; set; }
        public int a12Ordinal { get; set; }

        public string f06Name { get; set; } //combo
        public BO.f06BindScopeQueryENUM f06BindScopeQuery;

        public string CssClass
        {
            get
            {
                if (this.a12IsRequired)
                {
                    return "required";
                }
                else
                {
                    return null;
                }
            }
        }
        public int TempCount { get; set; }  //používá se při generování INEZ pro počet anketních formulářů

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
