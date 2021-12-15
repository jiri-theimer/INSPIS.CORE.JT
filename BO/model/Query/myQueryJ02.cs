using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryJ02:baseQuery
    {
        public int j04id { get; set; }
        public int a01id { get; set; }
        public int a03id { get; set; }
        public int a05id { get; set; }
        public int a04id { get; set; }
        public int j11id { get; set; }
        public int h04id { get; set; }
        public bool? j02isinvitedperson { get; set; }
        public bool? a02inspector { get; set; }
        public bool without_teams_and_owner { get; set; }

        public myQueryJ02()
        {
            this.Prefix = "j02";
        }

        public override List<QRow> GetRows()
        {
            if (this.j04id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j03User WHERE j04ID=@j04id)", "j04id", this.j04id);
            }
            if (this.a01id > 0)
            {
                if (this.without_teams_and_owner)
                {
                    AQ("a.j02ID IN (select j02ID FROM a41PersonToEvent WHERE a01ID=@a01id AND a45ID<>5 AND j02ID IS NOT NULL)", "a01id", this.a01id);
                }
                else
                {
                    AQ("a.j02ID IN (select j02ID FROM a41PersonToEvent WHERE a01ID=@a01id AND j02ID IS NOT NULL) OR a.j02ID IN (select xb.j02ID FROM a41PersonToEvent xa INNER JOIN j12Team_Person xb ON xa.j11ID=xb.j11ID WHERE xa.a01ID=@a01id AND xa.j11ID IS NOT NULL)", "a01id", this.a01id);
                }
            }
            if (this.a03id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM a39InstitutionPerson WHERE a03ID=@a03id)", "a03id", this.a03id);
            }
            if (this.a05id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM a02Inspector WHERE a05ID=@a05id AND GETDATE() BETWEEN a02ValidFrom AND a02ValidUntil)", "a05id", this.a05id);
            }
            if (this.a04id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM a02Inspector WHERE a04ID=@a04id AND GETDATE() BETWEEN a02ValidFrom AND a02ValidUntil)", "a04id", this.a04id);
            }
            if (this.j11id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID=@j11id)", "j11id", this.j11id);
            }
            if (this.h04id > 0)
            {
                AQ("a.j02ID IN (select j02ID FROM h06ToDoReceiver WHERE h04ID=@h04id AND j02ID IS NOT NULL)", "h04id", this.h04id);
            }
            if (this.j02isinvitedperson != null)
            {
                AQ("a.j02IsInvitedPerson=@j02isinvitedperson", "j02isinvitedperson", this.j02isinvitedperson);
            }
            if (this.a02inspector !=null)
            {
                if (this.a02inspector==true)
                {
                    AQ("a.j02ID IN (select j02ID FROM a02Inspector)", "", null);
                }
                else
                {
                    AQ("a.j02ID NOT IN (select j02ID FROM a02Inspector)", "", null);
                }
            }
            

            if (_searchstring != null && _searchstring.Length > 2)
            {
                string sw = _searchstring;
                if (CurrentUser.FullTextSearch)
                {
                    sw = $"Contains((a.j02FullText,a.j02Email,a.j02PID,a.j02Address,a.j02Mobile),'{this.ConvertSearchString2FulltextSyntax()}')";
                    AQ("(" + sw + ")", "", null);
                }
                else
                {
                    sw = "a.j02FullText like '%'+@ss+'%' OR a.j02Email LIKE '%'+@ss+'%' OR a.j02PID LIKE '%'+@ss+'%' OR a.j02Address LIKE '%'+@ss+'%' OR a.j02Mobile LIKE '%'+@ss+'%'";
                    AQ("(" + sw + ")", "ss", _searchstring);
                }
                

            }

            return this.InhaleRows();

        }
    }
}
