using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryA03:baseQuery
    {
        public int a01id { get; set; }
        public int j02id { get; set; }
        public int a03id_founder { get; set; }
        public int a03id_supervisory { get; set; }
        public int a03id_parent { get; set; }
        public int a42id { get; set; }
        public int a06id { get; set; }
        public int a29id { get; set; }
        public myQueryA03()
        {
            this.Prefix = "a03";
        }

        public override List<QRow> GetRows()
        {
            if (this.a01id > 0)
            {

                this.AQ("a.a01ID=@a01id", "a01id", this.a01id);
            }
            if (this.j02id > 0)
            {

                this.AQ("a.a03ID IN (select a03ID FROM a39InstitutionPerson WHERE j02ID=@j02id)", "j02id", this.j02id);
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
            if (this.param1 == "parent")
            {
                AQ("a.a03ParentFlag=1", "", null);    //filtr pouze nadřízené školy
            }

            if (_searchstring !=null && _searchstring.Length > 2)
            {
                string sw = string.Format("Contains((a.a03REDIZO,a.a03Name,a.a03ICO,a.a03City,a.a03Street),'{0}')", _searchstring);
                if (_searchstring.Length == 9 && BO.BAS.InDouble(_searchstring) > 0)
                {
                    sw = string.Format("Contains((a.a03REDIZO),'{0}')", s);
                    sw += string.Format(" OR a.a03ID IN (SELECT a03ID FROM a37InstitutionDepartment WHERE a37IZO = '{0}')", _searchstring);
                }


                AQ("(" + sw + ")", "", null);
            }

            return this.InhaleRows();

        }
    }
}
