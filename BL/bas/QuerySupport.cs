using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL.bas
{
    public class QuerySupport
    {
        public string getFiltrAlias(string prefix, IEnumerable<BO.j73TheGridQuery> lisJ73,BL.Factory f)
        {
            if (lisJ73.Count() == 0) return "";
            var lisFields = new BL.TheQueryFieldProvider(prefix).getPallete();

            var lis = new List<string>();

            foreach (var c in lisJ73)
            {
                string ss = "";
                BO.TheQueryField cField = null;
                if (c.j73BracketLeft != null)
                {
                    ss += "(";
                }
                if (c.j73Op == "OR")
                {
                    ss += " OR ";
                }
                if (lisFields.Where(p => p.Field == c.j73Column).Count() > 0)
                {
                    cField = lisFields.Where(p => p.Field == c.j73Column).First();
                    string s = cField.Header;
                    if (f.CurrentUser.j03LangIndex > 0)
                    {
                        s = f.tra(s);
                    }
                    ss = "[" + s + "] ";
                }
                switch (c.j73Operator)
                {
                    case "EQUAL":
                        ss += "=";
                        break;
                    case "NOT-ISNULL":
                        ss += f.tra("Není prázdné");
                        break;
                    case "ISNULL":
                        ss += f.tra("Je prázdné");
                        break;
                    case "INTERVAL":
                        ss += f.tra("Je interval");
                        break;
                    case "GREATERZERO":
                        ss += f.tra("Je větší než nula");
                        break;
                    case "ISNULLORZERO":
                        ss += f.tra("Je nula nebo prázdné");
                        break;
                    case "NOT-EQUAL":
                        ss += f.tra("Není rovno");
                        break;
                    case "CONTAINS":
                        lis.Add(f.tra("Obsahuje"));
                        break;
                    case "STARTS":
                        ss += f.tra("Začíná na");
                        break;
                    default:
                        break;
                }
                if (c.j73ValueAlias != null)
                {
                    ss += c.j73ValueAlias;
                }
                else
                {
                    ss += c.j73Value;
                }
                if (c.j73DatePeriodFlag > 0)
                {
                    var cPeriods = new BO.CLS.ThePeriodProviderSupport();
                    var lisPeriods = cPeriods.GetPallete();

                    var d1 = lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d1;
                    var d2 = Convert.ToDateTime(lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d2).AddDays(1).AddMinutes(-1);
                    ss += ": " + BO.BAS.ObjectDate2String(d1, "dd.MM.yyyy") + " - " + BO.BAS.ObjectDate2String(d2, "dd.MM.yyyy");
                }

                if (c.j73BracketRight != null)
                {
                    ss += ")";
                }
                lis.Add(ss);
            }

            return string.Join("; ", lis);
        }

    }
}
