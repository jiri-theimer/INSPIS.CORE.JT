using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BO
{
    public class myQueryA01 : baseQuery
    {
        public int a03id { get; set; }

        public int a01parentid { get; set; }
        public bool? a01IsTemporary;
        public bool? a01IsClosed;
        public int b02id { get; set; }
        public int a01id { get; set; }
        public int a42id { get; set; }
        public int j02id { get; set; }
        public int j02id_leader { get; set; }
        public int j02id_member { get; set; }
        public int j02id_invited { get; set; }
        public int j02id_involved { get; set; }
        public int j02id_issuer { get; set; }
        public int a10id { get; set; }
        public int a08id { get; set; }
        public myQueryA01()
        {
            this.Prefix = "a01";
        }

        public override List<QRow> GetRows()
        {
            if (this.global_d1 != null)
            {
                //AQ("a.a01DateFrom>=@gd1", "gd1", this.global_d1);
                //AQ("a.a01DateUntil<=@gd2", "gd2", this.global_d2);
                AQ("a.a01DateFrom >= @gd1 AND (a.a01DateUntil <= @gd2 OR a.a01DateUntil=CONVERT(DATETIME,'01.01.3000',104))", "gd1", this.global_d1,"AND",null,null,"gd2",this.global_d2);
            }
            if (this.MyRecordsDisponible && this.CurrentUser != null)
            {
                if (!(this.CurrentUser.j04IsAllowedAllEventTypes && this.CurrentUser.j04RelationFlag == BO.j04RelationFlagEnum.NoRelation))
                {
                    AQ(GetDisponibleWhere(), null, null);
                }
            }
            if (this.b02id > 0)
            {
                AQ("a.b02ID=@b02id", "b02id", this.b02id);
            }
            if (this.a03id > 0)
            {
                AQ("a.a03ID=@a03id", "a03id", this.a03id);
            }
            if (this.a01id > 0) //související akce
            {
                AQ("(a.a01ID IN (select a01ID_Left FROM a24EventRelation WHERE a01ID_Left=@a01id OR a01ID_Right=@a01id) OR a.a01ID IN (select a01ID_Right FROM a24EventRelation WHERE a01ID_Left=@a01id OR a01ID_Right=@a01id)) AND a.a01ID<>@a01id", "a01id", this.a01id);
            }

            if (this.a01parentid > 0)
            {
                AQ("a.a01ParentID=@a01parentid", "a01parentid", this.a01parentid);   //podřízené akce
            }
            if (this.a01IsTemporary != null)
            {
                AQ("a.a01IsTemporary=@istemp", "istemp", this.a01IsTemporary);   //temp akce
            }
            if (this.a01IsClosed != null)
            {
                AQ("a.a01IsClosed=@isclosed", "isclosed", this.a01IsClosed);     //uzavřená akce
            }
            if (this.a42id > 0)
            {
                AQ("a.a42ID=@a42id", "a42id", this.a42id);
            }
            if (this.j02id > 0)
            {
                AQ("a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE j02ID=@j02id)", "j02id", this.j02id);   //je účastníkem akce
            }

            if (this.j02id_leader > 0)
            {
                AQ("a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=2 AND j02ID=@j02id_leader)", "j02id_leader", this.j02id_leader);   //je vedoucím akce
            }
            if (this.j02id_member > 0)
            {
                AQ("a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=1 AND j02ID=@j02id_member)", "j02id_member", this.j02id_member);   //je člen akce               
            }
            if (this.j02id_invited > 0)
            {
                AQ("a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=6 AND j02ID=@j02id_invited)", "j02id_invited", this.j02id_invited);   //je přizvaná osoba
            }
            if (this.j02id_involved > 0)
            {
               AQ("a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE j02ID=@j02id_involved)", "j02id_involved", this.j02id_involved);   //je zapojen do akce
            }
            if (this.j02id_issuer > 0)
            {
                AQ("a.j02ID_Issuer=@j02id_issuer", "j02id_issuer", this.j02id_issuer);   //je zakladatelem akce                
            }
            if (this.a10id > 0)
            {
                AQ("a.a10ID=@a10id", "a10id", this.a10id);
            }
            if (this.a08id > 0)
            {
                AQ("a.a08ID=@a08id", "a08id", this.a08id);
            }

            switch (this.param1)
            {
                case "j02IsInvitedPerson":
                    AQ("a.j02IsInvitedPerson=1", "", null);    //filtr přizvaných osob
                    break;
                case "a02Inspector":
                    AQ("a.j02ID IN (select j02ID FROM a02Inspector)", "", null);    //filtr přizvaných osob
                    break;
            }


            if (_searchstring != null && _searchstring.Length > 2)
            {                
                string sw = string.Format("Contains((a.a01Signature,a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01InstitutionPlainText,a.a01InstitutionPlainTextRedizo),'{0}')", _searchstring);
                AQ("(" + sw + ")", "", null);
                //pro HD:
                //sw = "Contains((a.a01Signature,a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01Description,a.a01InstitutionPlainText,a.a01InstitutionPlainTextRedizo),@expr)";
                //sw += " OR a.a01ID IN (select a01ID FROM b05Workflow_History WHERE Contains((b05Comment),@expr))";
            }

            return this.InhaleRows();

        }



        private string GetDisponibleWhere()
        {
            //výběr uživateli dostupných akcí podle aplikační role a účasti v akcích
            string sw = "(";
            sw += "a.a01ID IN (SELECT a01ID FROM a41PersonToEvent WHERE j02ID = " + this.CurrentUser.j02ID.ToString() + ")";
            //na týmy si nehrají: sw += " OR a.a01ID IN (SELECT xa.a01ID FROM a41PersonToEvent xa INNER JOIN j12Team_Person xb on xa.j11ID=xb.j11ID WHERE xa.j11ID IS NOT NULL AND (xb.j02ID=" + mq.CurrentUser.j02ID.ToString() + " OR xb.j04ID=" + mq.CurrentUser.j04ID.ToString() + "))";
            switch (this.CurrentUser.j04RelationFlag)
            {
                case BO.j04RelationFlagEnum.A03:    //omezení akcí pouze na svázané instituce
                    sw += " OR (a.a03ID IN (SELECT a03ID FROM a39InstitutionPerson WHERE j02ID=" + this.CurrentUser.j02ID.ToString() + ")";
                    if (!this.CurrentUser.j04IsAllowedAllEventTypes)
                    {
                        if (!string.IsNullOrEmpty(this.CurrentUser.a10IDs))
                        {
                            sw += " AND a.a10ID IN (" + this.CurrentUser.a10IDs + ")";
                        }
                        else
                        {
                            sw += " AND a.a10ID IN (SELECT a10ID FROM j08UserRole_EventType WHERE (j08IsAllowedRead=1 OR j08IsLeader=1 OR j08IsMember=1) AND j04ID=" + this.CurrentUser.j04ID.ToString() + ")";
                        }

                    }
                    sw += ")";
                    break;
                case BO.j04RelationFlagEnum.A05:    //omezení akcí pouze na kraj instituce
                    sw += " OR (a01_a03.a05ID=" + this.CurrentUser.a05ID.ToString();
                    if (!this.CurrentUser.j04IsAllowedAllEventTypes)
                    {
                        if (!string.IsNullOrEmpty(this.CurrentUser.a10IDs))
                        {
                            sw += " AND a.a10ID IN (" + this.CurrentUser.a10IDs + ")";
                        }
                        else
                        {
                            sw += " AND a.a10ID IN (SELECT a10ID FROM j08UserRole_EventType WHERE (j08IsAllowedRead=1 OR j08IsLeader=1 OR j08IsMember=1) AND j04ID=" + this.CurrentUser.j04ID.ToString() + ")";
                        }
                    }
                    sw += ")";
                    //navíc přístup ke všem akcím škol, kde má daný inspektor minimálně jednu otevřenou akci (a01IsClosed=0)
                    sw += " OR a.a03ID IN (SELECT qb.a03ID FROM a41PersonToEvent qa INNER JOIN a01Event qb ON qa.a01ID=qb.a01ID WHERE qb.a01IsClosed=0 AND qa.j02ID=" + this.CurrentUser.j02ID.ToString() + ")";
                    //s týmy nepracují v akcích: sw += " OR a.a03ID IN (SELECT qb.a03ID FROM a01Event qb INNER JOIN a41PersonToEvent qa ON qb.a01ID=qa.a01ID INNER JOIN j12Team_Person qc on qa.j11ID=qc.j11ID WHERE qb.a01IsClosed=0 AND qa.j11ID IS NOT NULL AND (qc.j02ID="+mq.CurrentUser.j02ID.ToString()+" OR qc.j04ID="+mq.CurrentUser.j04ID.ToString()+"))";

                    break;
                case BO.j04RelationFlagEnum.NoRelation: //role bez vazby na region nebo školu - potenciálně neomezený přístup k akcím
                    if (!this.CurrentUser.j04IsAllowedAllEventTypes)
                    {//nemá právo vidět všechny akce v systému
                        if (!string.IsNullOrEmpty(this.CurrentUser.a10IDs))
                        {
                            sw += " OR a.a10ID IN (" + this.CurrentUser.a10IDs + ")";
                        }
                        else
                        {
                            sw += " OR a.a10ID IN (SELECT a10ID FROM j08UserRole_EventType WHERE (j08IsAllowedRead=1 OR j08IsLeader=1 OR j08IsMember=1) AND j04ID=" + this.CurrentUser.j04ID.ToString() + ")";
                        }

                    }
                    break;

            }
            sw += ")";
            //System.IO.File.WriteAllText("\\hovado.txt", sw);
            return sw;
        }
    }
}
