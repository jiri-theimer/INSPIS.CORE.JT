using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;


namespace UI
{
    public class Sql2HtmlTableDef
    {
        public string ColHeaders { get; set; }
        public string ColTypes { get; set; }    //bool|num0|num|date|datetime|string
    }
    public class Sql2HtmlTable
    {
        private Sql2HtmlTableDef _def;
        private System.Text.StringBuilder _sb;
        private List<string> _headers;
        private List<string> _types;
        public Sql2HtmlTable(Sql2HtmlTableDef def)
        {
            _def = def;
            _sb = new System.Text.StringBuilder();

            _headers = BO.BAS.ConvertString2List(def.ColHeaders, "|");
            _types = BO.BAS.ConvertString2List(def.ColTypes, "|");

        }

        public string DataTable2String(System.Data.DataTable dt)
        {
            sb("<table class='table table-sm table-hover'>");
            handle_headers();
            handle_body(dt);

            sb("</table>");
            return _sb.ToString();
        }

        private void handle_headers()
        {
            sb("<thead><tr>");
            foreach (string s in _headers)
            {
                sb("<th>" + s + "</th");
            }
            sb("</tr></thead>");
        }
        private void handle_body(System.Data.DataTable dt)
        {
            sb("<tbody>");
            


            foreach (System.Data.DataRow dbRow in dt.Rows)
            {
                sb("<tr>");
                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    string strVal = "";
                    if (_types[i].ToLower()=="n" || _types[i].ToLower() == "n0")
                    {
                        sb("<td style='text-align:right;'>");
                    }
                    else
                    {
                        sb("<td>");
                    }
                    
                    if (dbRow[i] != DBNull.Value)
                    {
                        switch (_types[i].ToLower())
                        {
                            case "b":
                                if (Convert.ToBoolean(dbRow[i]) == true)
                                {
                                    strVal = "&#10004;";
                                }
                                break;
                            case "n":
                                strVal= string.Format("{0:#,0.00}", dbRow[i]);
                                break;
                            case "n0":
                            case "i":
                                strVal = string.Format("{0:#,0}", dbRow[i]);
                                break;
                            case "d":
                                strVal=Convert.ToDateTime(dbRow[i]).ToString("dd.MM.yyyy");
                                break;
                            case "dt":
                                strVal = Convert.ToDateTime(dbRow[i]).ToString("dd.MM.yyyy HH:mm");
                                break;
                            default:
                                strVal=dbRow[i].ToString();
                                break;
                        }
                        sb(strVal);
                    }
                    sb("</td>");
                }
                sb("</tr>");
            }
            
            sb("</tbody>");
        }

        private void sb(string s)
        {
            _sb.Append(s);
        }
        private void sbl(string s)
        {
            _sb.AppendLine(s);
        }
    }
}
