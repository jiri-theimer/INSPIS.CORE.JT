using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class k02TeacherSchool:BaseBO
    {
        [Key]
        public int k02ID { get; set; }
        public int k01ID { get; set; }
        public int a03ID { get; set; }
    }
}
