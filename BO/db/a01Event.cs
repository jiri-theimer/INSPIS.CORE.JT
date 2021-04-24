using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum a01EventPermissionENUM
    {
        NoAccess = 0,
        ReadOnlyAccess = 1,
        FullAccess = 3,
        ShareTeam_Member = 11,
        ShareTeam_Leader = 12,
        ShareTeam_Owner = 15,
        ShareTeam_InvitedPerson = 16,
        HD_Requestor = 13
    }
    

    public class a01Event : BaseBO
    {
        [Key]
        public int a01ID { get; set; }

        public int a03ID { get; set; }
        public int a10ID { get; set; }
        public int a42ID { get; set; }
        public int a57ID { get; set; }
        public int a08ID { get; set; }
        public int b02ID { get; set; }
        public int a01ParentID { get; set; }

        public int j03ID_Creator { get; set; }
        public int j02ID_Issuer { get; set; }
        public int a61ID { get; set; }
        public int a22ID { get; set; }
        public int a70ID { get; set; }
        public int a46ID { get; set; }
        public string a01Name { get; set; }
        public string a01Description { get; set; }
        public string a01CaseCode { get; set; }
        public string a01CaseCodePID { get; set; }

        public bool a01IsTemporary { get; set; }
        public bool a01IsClosed { get; set; }
        public bool a01IsAllFormsClosed { get; set; }
        public DateTime? a01DateClosed { get; set; }

        public DateTime? a01DateFrom { get; set; }
        public DateTime? a01DateUntil { get; set; }


        public string TagHtml;

        //readonly:
        public string a01Signature;
        public int a01SigID;
        public string a03Name { get; set; } //kvůli combo
        public string a03REDIZO;
        public string a06Name;
        public string a10Name;
        public string a10ViewUrl_Page;
        public string a10CoreFlag;
        public string a08Name { get; set; } //kvůli combo
        public int a05ID;
        public string a05Name;
        public string a09Name;

        public string b02Name;
        public string b02Color;
        public string b02Ident;
        public int b01ID;

        public string a01LeaderInLine;
        public string a01MemberInLine;
        public string a01InstitutionPlainText;

        public int a01ChildsCount;
        
        
    }
}
