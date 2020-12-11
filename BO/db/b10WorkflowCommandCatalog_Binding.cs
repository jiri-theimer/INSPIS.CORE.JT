using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum b10TargetScopeEnum
    {
        ThisAndSlaves = 0,
        ThisOnly = 1,
        SlavesOnly = 2,
        ParentOnly = 3
    }
    //public enum b10TargetOperationEnum
    //{
    //    None=0,
    //    a01DateFrom = 1,
    //    a01DateUntil=2,
    //    b02ID =3,
    //}
    public class b10WorkflowCommandCatalog_Binding:BaseBO
    {
        [Key]
        public int b10ID { get; set; }
        public int b09ID { get; set; }
        public int b06ID { get; set; }
        public int b02ID { get; set; }
        
        public b10TargetScopeEnum b10TargetScopeFlag { get; set; }
        //public b10TargetOperationEnum b10TargetOperationFlag { get; set; }
        public int a10ID_TargetUpdate { get; set; }
        public string a10Name_TargetUpdate { get; set; }    //combo
        public int b02ID_TargetUpdate { get; set; }
        public string b02Name_TargetUpdate { get; set; }    //combo

        public string b10Parameter1 { get; set; }
        public string b10Parameter2 { get; set; }
        public string b10Parameter3 { get; set; }

        public string b09Name { get; set; } //combo
        public string b09Ident { get; set; }    //combo
        public string b09SQL;
        public int b09ParametersCount { get; set; }
        
        public bool IsUpdateStatusCommand { get     //true = Nabízet pro tento příkaz možnost nahodit jiný stav akce
            {
                if (this.b09Ident !=null && this.b09Ident.ToLower() == "b02id")
                {
                    return true;
                }
                return false;
            }
        }

        

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
