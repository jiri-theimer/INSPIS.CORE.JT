﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryF21:baseQuery
    {
        public int f22id { get; set; }
        public int f19id { get; set; }
        public int f19id_onlyin { get; set; }   //pouze jednotky, které se používají v otázce f19id_onlyin
        public bool searchunits { get; set; }

        public myQueryF21()
        {
            this.Prefix = "f21";
        }

        public override List<QRow> GetRows()
        {
            if (this.f22id > 0)
            {
                AQ("a.f21ID IN (SELECT f21ID FROM f43ReplyUnitToSet WHERE f22ID=@f22id)", "f22id", this.f22id);
            }
            if (this.f19id > 0)
            {
                AQ("a.f21ID IN (SELECT f21ID FROM f20ReplyUnitToQuestion WHERE f19ID=@f19id)", "f19id", this.f19id);
            }
            if (this.f19id_onlyin > 0)
            {
                AQ("a.f21ID IN (SELECT f21ID FROM f20ReplyUnitToQuestion WHERE f19ID=@f19id_onlyin) AND a.f21ID NOT IN (select f21ID FROM f20ReplyUnitToQuestion WHERE f19ID<>@f19id_onlyin)", "f19id_onlyin", this.f19id_onlyin);
            }
            if (this.searchunits)
            {
                AQ("ISNULL(a.f21Name,'') NOT IN ('textbox','checkbox','fileupload','')", "", null);    //filtr neprázdných jednotek odpovědi
            }

            if (_searchstring != null && _searchstring.Length > 2)
            {
                AQ("(a.f21Name LIKE '%'+@expr+'%' OR a.f21Description LIKE '%'+@expr+'%' OR a.f21ExportValue LIKE '%'+@expr+'%')", "expr", _searchstring);

            }

            return this.InhaleRows();

        }
    }
}
