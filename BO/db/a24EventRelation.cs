using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class a24EventRelation:BaseBO
    {
        [Key]
        public int a24ID { get; set; }
        public int a46ID { get; set; }
        public int a01ID_Left { get; set; }
        public int a01ID_Right { get; set; }

        public string a24Description { get; set; }

        public string a46Name { get; set; } //combo

        public string a01Signature_Left;
        public string a01Signature_Right;
        public string a10Name_Left;
        public string a10Name_Right;

        public string SignaturePlusType_Left
        {
            get
            {
                return this.a01Signature_Left + " (" + this.a10Name_Left + ")";
            }
        }
        public string SignaturePlusType_Right
        {
            get
            {
                return this.a01Signature_Right + " (" + this.a10Name_Right + ")";
            }
        }
    }
}
