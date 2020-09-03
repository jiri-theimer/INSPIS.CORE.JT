using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class iSETSchoolClass
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int ClassGrade { get; set; }
        public int ClassStatus { get; set; }
    }
}
