using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum h06TodoRoleEnum
    {
        Resitel = 1,
        BytInformovan = 2
    }
    public class h06ToDoReceiver: BaseBO
    {
        [Key]
        public int h06ID { get; set; }
        public int h04ID { get; set; }
        public int j02ID { get; set; }
        public int j11ID { get; set; }

        public h06TodoRoleEnum h06TodoRole { get; set; }

        public string j02FirstName;
        public string j02LastName;
        public string j02TitleBeforeName;
        public string j02TitleAfterName;

        public string PersonAsc
        {
            get
            {

                return (this.j02TitleBeforeName + " " + this.j02FirstName + " " + this.j02LastName + " " + this.j02TitleAfterName).Trim();
            }

        }
        public string PersonDesc
        {
            get
            {

                return (this.j02LastName + " " + this.j02FirstName + " " + this.j02TitleBeforeName).Trim();
            }

        }
    }
}
