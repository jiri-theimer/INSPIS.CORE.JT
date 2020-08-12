using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{

    public enum j04RelationFlagEnum
    {
        NoRelation = 1,     //Aplikační role bez omezení uživatele podle jeho vztahu k instituci nebo k inspektorátu
        A03 = 2,            //Aplikační role s omezením na příslušnost uživatele k instituci
        A05 = 3             //Aplikační role s omezením na příslušnost osoby uživatele ke kraji podle inspektorátu
    }
    public enum j04PortalFaceFlagEnum       //Vztah role k PORTÁLu
    {
        CSI = 1,        //ČŠI uživatel
        School = 2,     //škola
        Founder = 3,    //zřizovatel
        Anonymous = 4   //veřejnost
    }
    public class j04UserRole : BaseBO
    {
        [Key]
        public int j04ID { get; set; }        
        public string j04Name { get; set; }
        public string j04RoleValue { get; set; }
        public string j04Aspx_PersonalPage { get; set; }
        public string j04Aspx_PersonalPage_Mobile { get; set; }

        public j04RelationFlagEnum j04RelationFlag { get; set; }
        public j04PortalFaceFlagEnum j04PortalFaceFlag { get; set; }

        public bool j04IsAllowedAllEventTypes { get; set; }     //true: Možnost zakládat i číst všechny typy akcí
        public bool j04IsAllowInSchoolAdmin { get; set; }       //true: Role dostupná ve správě školních účtů
        public int j04ElearningDuration { get; set; }
        public bool j04IsElearningNeeded { get; set; }

    }
}
