using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA03:baseQuery
    {
        
        public int j02id { get; set; }
        public int j02id_employee_only { get; set; } //osoba zaměstnanec pouze
        public int j02id_contact_only { get; set; } //osoba kontaktní pouze
        public int a03id_founder { get; set; }
        public int a03id_supervisory { get; set; }
        public int a03id_parent { get; set; }
        public int a42id { get; set; }
        public int a06id { get; set; }
        public int a29id { get; set; }
        public int a03parentflag { get; set; } = -1;
        public myQueryA03()
        {
            this.Prefix = "a03";
        }

        public override List<QRow> GetRows()
        {
            if (this.MyRecordsDisponible && this.CurrentUser != null)
            {
                AQ(GetDisponibleWhere(), null, null);
            }

            if (this.j02id > 0)
            {
                this.AQ("a.a03ID IN (select a03ID FROM a39InstitutionPerson WHERE j02ID=@j02id)", "j02id", this.j02id);
            }
            if (this.j02id_employee_only > 0)
            {
                this.AQ("a.a03ID IN (select a03ID FROM a39InstitutionPerson WHERE a39RelationFlag=2 AND j02ID=@j02id_employee_only)", "j02id_employee_only", this.j02id_employee_only);
            }
            if (this.j02id_contact_only > 0)
            {
                this.AQ("a.a03ID IN (select a03ID FROM a39InstitutionPerson WHERE a39RelationFlag=1 AND j02ID=@j02id_contact_only)", "j02id_contact_only", this.j02id_contact_only);
            }
            if (this.a03id_founder > 0)
            {
                AQ("a.a03ID_Founder=@a03id_founder", "a03id_founder", this.a03id_founder);
            }
            if (this.a03id_supervisory > 0)
            {
                AQ("a.a03ID_Supervisory=@a03id_supervisory", "a03id_supervisory", this.a03id_supervisory);
            }
            if (this.a03id_parent > 0)
            {
                AQ("a.a03ID_Parent=@a03id_parent", "a03id_parent", this.a03id_parent);
            }
            if (this.a42id > 0)
            {
                AQ( "a.a03ID IN (select a03ID FROM a01Event WHERE a42ID=@a42id)", "a42id", this.a42id);
            }
            if (this.a06id > 0)
            {
                AQ("a.a06ID=@a06id", "a06id", this.a06id);
            }
            if (this.a29id > 0)
            {
                AQ("a.a03ID IN (select a03ID FROM a43InstitutionToList WHERE a29ID=@a29id)", "a29id", this.a29id);
            }
            if (this.a03parentflag>-1)
            {
                AQ("a.a03ParentFlag=@parentflag", "@parentflag", this.a03parentflag);    //filtr pouze nadřízené školy
            }

            if (_searchstring !=null && _searchstring.Length > 2)
            {
                string sw = "";
                if (this.CurrentUser.FullTextSearch)
                {
                    sw = string.Format("Contains((a.a03REDIZO,a.a03Name,a.a03ICO,a.a03City,a.a03Street),'\"{0}*\"')", _searchstring);
                    if (_searchstring.Length == 9 && BO.BAS.InDouble(_searchstring) > 0)
                    {
                        sw = string.Format("Contains((a.a03REDIZO),'{0}')", _searchstring);
                        sw += string.Format(" OR a.a03ID IN (SELECT a03ID FROM a37InstitutionDepartment WHERE a37IZO = '{0}')", _searchstring);
                    }
                    AQ("(" + sw + ")", "", null);
                }
                else
                {
                    sw = "a.a03Name like '%'+@ss+'%' OR a.a03Email LIKE '%'+@ss+'%' OR a.a03City LIKE '%'+@ss+'%' OR a.a03Street LIKE '%'+@ss+'%'";
                    if (BO.BAS.InDouble(_searchstring) > 0)
                    {                        
                        sw += " OR a.a03ICO like @ss+'%' OR a.a03REDIZO like '%'+@ss+'%'";
                    }
                    if (_searchstring.Length == 9 && BO.BAS.InDouble(_searchstring) > 0)
                    {
                        sw += " OR a.a03ID IN (SELECT a03ID FROM a37InstitutionDepartment WHERE a37IZO=@ss)";
                    }
                    AQ("(" + sw + ")", "ss", _searchstring);
                    
                }
                    
                


                
            }

            return this.InhaleRows();

        }

        private string GetDisponibleWhere()
        {
            //výběr uživateli dostupných institucí
            string sw = "";            
           
            switch (this.CurrentUser.j04RelationFlag)
            {
                case BO.j04RelationFlagEnum.A03:    //omezení akcí pouze na svázané instituce
                    sw = "a.a03ID IN (SELECT a03ID FROM a39InstitutionPerson WHERE j02ID=" + this.CurrentUser.j02ID.ToString() + ")";
                    
                    break;
                case BO.j04RelationFlagEnum.A05:    //omezení  pouze na kraj instituce
                    sw = "(";
                    sw += "a.a05ID=" + this.CurrentUser.a05ID.ToString();
                   
                    
                    //navíc přístup ke všem akcím škol, kde má daný inspektor minimálně jednu otevřenou akci (a01IsClosed=0)
                    sw += " OR a.a03ID IN (SELECT qb.a03ID FROM a41PersonToEvent qa INNER JOIN a01Event qb ON qa.a01ID=qb.a01ID WHERE qb.a01IsClosed=0 AND qa.j02ID=" + this.CurrentUser.j02ID.ToString() + ")";

                    sw += ")";

                    break;
                case BO.j04RelationFlagEnum.NoRelation: //role bez vazby na region nebo školu - potenciálně neomezený přístup k institucím
                   
                    break;

            }
           
            //System.IO.File.WriteAllText("\\hovado.txt", sw);
            return sw;
        }
    }
}
