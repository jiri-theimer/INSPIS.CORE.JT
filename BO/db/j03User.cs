using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum j03AdminRoleValueFlagEnum
    {
        _none=0,
        uzivatel_er = 1,
        uzivatel_ro = 2,
        inspektorat_er = 3,
        inspektorat_ro = 4,
        neper_er = 5,
        neper_ro = 6,
        instituce_er = 7,
        instituce_ro = 8,
        akce_er = 9,
        akce_ro = 10,
        formular_er = 11,
        formular_ro = 12,
        priloha_er = 13,
        priloha_ro = 14,
        ukol_er = 15,
        ukol_ro = 16,
        prazdne_er = 17,
        prazdne_ro = 18,
        sestava_er = 19,
        sestava_ro = 20,
        ostatni_er = 21,
        ostatni_ro = 22
    }
    public class j03User : BaseBO
    {
        [Key]
        public int j03ID { get; set; }
        public int j02ID { get; set; }

        public string j03Login { get; set; }

        public bool j03IsDomainAccount { get; set; }
        public bool j03IsSystemAccount { get; set; }
        public bool j03IsMustChangePassword { get; set; }
        public int j04ID { get; set; }
        public string j03PasswordHash { get; set; } //pro CORE verzi hashované heslo+login
        public int j03AccessFailedCount { get; set; }
        public int j03ModalDialogFlag { get; set; }
        public int j03FontStyleFlag { get; set; }
        public DateTime? j03LiveChatTimestamp { get; set; }   //na 20 minut zapnutý live-chat
        public bool j03IsMainLogoVisible { get; set; }  //zda je zapnutý pruh s logem nad hlavním menu
        public bool j03IsDebugLog { get; set; } //zda je u uživatele zapnuté logování jeho SQL dotazů
        public DateTime? j03PingTimestamp { get; set; }

        public int j03GridSelectionModeFlag { get; set; }

        public int j03LangIndex { get; set; }
        public string j03HomePageUrl { get; set; }
        public string j03AdminRoleValue { get; set; }   //rozklad oprávnění pro číselníky
        //readonly:
        public string fullname_desc;
        public string j02Email;
        public string j04Name { get; set; } //kvůli combo


    }
}
