using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b05Workflow_History:BaseBO
    {
        [Key]
        public int b05ID { get; set; }
        public int a01ID { get; set; }
        public int b06ID { get; set; }
        public int j03ID_Sys { get; set; }
        public int j11ID_Nominee { get; set; }
        public int j02ID_Nominee { get; set; }
        public int a45ID_Nominee { get; set; }
        public bool b05IsNominee { get; set; }
        public int b02ID_From { get; set; }
        public int b02ID_To { get; set; }
        public bool b05IsManualStep { get; set; }
        public bool b05IsCommentOnly { get; set; }
        public string b05Comment { get; set; }
        public bool b05IsCommentRestriction { get; set; }
        public bool b05IsBatchUpdate { get; set; }
        public string b05SQL { get; set; }

        //readonly
        public string b06Name;
        public bool b06IsHistoryAllowedToAll;
        public string b02Name_From;
        public string b02Name_To;
        public string a45Name;
        public string a01Signature;
        public string j03login;
        public string Person;
        public int j02ID;
        public string StatusMoveHtml
        {
            get
            {
                if (this.b02ID_From == this.b02ID_To)
                {
                    return null;
                }
                else
                {
                    return string.Format("{0} ➝ {1}", this.b02Name_From, this.b02Name_To);
                }
            }
        }
        public string StatusMoveHtmlRed
        {
            get
            {
                if (this.b02ID_From == this.b02ID_To)
                {
                    return null;
                }
                else
                {
                    return string.Format("<span>{0} ➝ </span><span style='color:red;'>{1}</span>", this.b02Name_From,this.b02Name_To);
                }
            }
        }

        
    }
}
