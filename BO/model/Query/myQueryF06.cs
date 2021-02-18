using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQueryF06:baseQuery
    {
        public int a01id { get; set; }
        public int b06id { get; set; }
        public int a08id { get; set; }
        public int f12id { get; set; }
        public bool? ispollforms { get; set; }  //true: formuláře použitelné jako anketní
        public myQueryF06()
        {
            this.Prefix = "f06";
        }

        public override List<QRow> GetRows()
        {
            if (this.a01id > 0)
            {

                AQ("a.f06ID IN (select f06ID FROM a11EventForm WHERE a01ID=@a01id)", "a01id", this.a01id);
            }
            if (this.b06id > 0)
            {

                AQ("a.f06ID IN (select f06ID FROM b13WorkflowRequiredFormsToStep WHERE b06ID=@b06id)", "b06id", this.b06id);
            }
            if (this.a08id > 0)
            {
                AQ("a.f06ID IN (select f06ID FROM a12ThemeForm WHERE a08ID=@a08id)", "a08id", this.a08id);
            }
            if (this.f12id > 0)
            {
                AQ("a.f12ID=@f12id", "f12id", this.f12id);
            }
            if (this.ispollforms==true)
            {
                AQ("ISNULL(a.f06BindScopeQuery,0) IN (0,2)", "", null);    //formuláře použitelné jako anketní
            }

            if (_searchstring != null && _searchstring.Length > 2)
            {
                AQ("(a.f06Name LIKE '%'+@expr+'%' OR a.f06Description LIKE '%'+@expr+'%')", "expr", _searchstring);
            }

            return this.InhaleRows();

        }
    }
}
