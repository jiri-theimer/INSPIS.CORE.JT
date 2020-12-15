﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryH04:baseQuery
    {
        public int h07id { get; set; }
        public int a01id { get; set; }
        public int j02id { get; set; }
        public int j02id_member { get; set; }
        public int j02id_issuer { get; set; }
        public myQueryH04()
        {
            this.Prefix = "h04";
        }

        public override List<QRow> GetRows()
        {
            if (this.h07id > 0)
            {

                this.AQ("a.h07ID=@h07id", "h07id", this.h07id);
            }
            if (this.a01id > 0)
            {
                AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
            if (this.j02id > 0)
            {
                AQ("(a.h04ID IN (select h04ID FROM h06ToDoReceiver WHERE j02ID=@j02id) OR a.j02ID_Owner=@j02id)", "j02id", this.j02id);
            }

            
            if (this.j02id_member > 0)
            {
                AQ("a.h04ID IN (select h04ID FROM h06ToDoReceiver WHERE j02ID=@j02id_member)", "j02id_member", this.j02id_member);   //je řešitel úkolu
            }
           
            if (this.j02id_issuer > 0)
            {
                AQ("a.j02ID_Owner=@j02id_issuer", "j02id_issuer", this.j02id_issuer);   //je zakladatelem úkolu
            }

            return this.InhaleRows();

        }

    }
}
