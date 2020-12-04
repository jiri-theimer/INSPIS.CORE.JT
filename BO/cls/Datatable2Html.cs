using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace BO.CLS
{
    public class Datatable2HtmlDef
    {
        public string ColHeaders { get; set; }
        public string ColTypes { get; set; }    //bool|num0|num|date|datetime|string           
        public string ClientID { get; set; }
        public bool IsUseDatatables { get; set; }
    }
    public class Datatable2Html
    {
        private Datatable2HtmlDef _def;
        private System.Text.StringBuilder _sb;
        private List<string> _headers;
        private List<string> _types;
        
        public Datatable2Html(Datatable2HtmlDef def)
        {
            _def = def;
            _sb = new System.Text.StringBuilder();

            _headers = BO.BAS.ConvertString2List(def.ColHeaders, "|");
            _types = BO.BAS.ConvertString2List(def.ColTypes, "|");

        }

        public string CreateHtmlTable(System.Data.DataTable dt,int maxrows=1000)
        {
            if (_def.IsUseDatatables)
            {                
                sb("<table class='display compact' style='width:100%;'");       //plugin datatables
            }
            else
            {
                sb("<table class='table table-sm table-striped table-hover'");       //prostá bootstrap tabulka
            }
            if (_def.ClientID != null)
            {
                sb(" id='" + _def.ClientID + "'");
            }
            sb(">");
            

            handle_headers();
            handle_body(dt, maxrows);
            
            sb("</table>");

           
            return _sb.ToString();
        }

        private void handle_headers()
        {
            sb("<thead><tr>");
            foreach (string s in _headers)
            {
                sb("<th>" + s + "</th>");
            }
            sb("</tr></thead>");
        }
        private void handle_body(System.Data.DataTable dt,int intLimitRows=1000)
        {
            sb("<tbody>");


            int x = 0;
            List<string> styles;
            foreach (System.Data.DataRow dbRow in dt.Rows)
            {                
                sb("<tr>");
                for (int i = 0; i <= _headers.Count - 1; i++)
                {
                    string strVal = "";
                    string strDataSort = null;
                    styles = new List<string>();
                    sb("<td");
                    if (_types[i].ToLower()=="n" || _types[i].ToLower() == "n0")
                    {
                        styles.Add("text-align:right");
                        
                    }
                    if (_types[i].ToLower() == "rcm")
                    {
                        styles.Add("width:20px");
                    }
                    if (styles.Count > 0)
                    {
                        sb(" style='");
                        sb(string.Join(";", styles));
                        sb("'");
                    }
                    

                    if (dbRow[i] != DBNull.Value)                   
                    {
                        switch (_types[i].ToLower())
                        {
                            case "rcm":
                                strVal= string.Format("<a class='cm' onclick=\"_cm(event, '{0}',{1})\">☰</a>", dbRow ["prefix"], dbRow[i]);
                                break;
                            case "b":
                                if (Convert.ToBoolean(dbRow[i]) == true)
                                {
                                    strVal = "&#10004;";
                                    strDataSort = "1";
                                }
                                break;
                            case "n":
                                strVal= string.Format("{0:#,0.00}", dbRow[i]);
                                strDataSort = dbRow[i].ToString();
                                break;
                            case "n0":
                            case "i":
                                strVal = string.Format("{0:#,0}", dbRow[i]);
                                strDataSort = dbRow[i].ToString();
                                break;
                            case "d":
                                strVal=Convert.ToDateTime(dbRow[i]).ToString("dd.MM.yyyy");
                                strDataSort = Convert.ToDateTime(dbRow[i]).ToString("yyyy-MM-dd");
                                break;
                            case "dt":
                                strVal = Convert.ToDateTime(dbRow[i]).ToString("dd.MM.yyyy HH:mm");
                                strDataSort = Convert.ToDateTime(dbRow[i]).ToString("yyyy-MM-dd HH:mm");
                                break;
                            case "a":
                                strVal = dbRow[i].ToString().Replace("€€", "\"").Replace("$$", "'");    //link, odkaz
                                break;
                            default:
                                strVal=dbRow[i].ToString();
                                break;
                        }

                        if (strDataSort != null)
                        {
                            sb(string.Format(" data-sort='{0}'", strDataSort));           //plugin datatables tomuto rozumí                     
                        }                        
                    }
                    sb(">");
                    sb(strVal);
                    sb("</td>");
                }
                sb("</tr>");
                x += 1;
                if (x > intLimitRows)
                {
                    break;  //omezovač na extrémně dlouhé tabulky
                }
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
