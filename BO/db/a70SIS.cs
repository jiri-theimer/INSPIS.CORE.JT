using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum a70ScopFlagENUM
    {
        TestSchoolOnly=1,
        RealSchoolOnly=2,
        TestAndRealSchool=3,
        GlobalSystem=4
    }
    public class a70SIS:BaseBO
    {
        [Key]
        public int a70ID { get; set; }
        public string a70Name { get; set; }
        public string a70Code { get; set; }
        public string a70Description { get; set; }
        public a70ScopFlagENUM a70ScopFlag { get; set; }

        public string a70WsLogin { get; set; }

        public string ScopeFlagAlias;


    }
}
