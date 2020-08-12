﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Pkcs;
using UI.Models;

namespace UI.Controllers
{
    public class TheComboController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;

        public TheComboController(BL.TheColumnsProvider cp)
        {
            _colsProvider = cp;
        }

        public mysearchSetting LoadMySearchSetting()
        {
            var ret = new mysearchSetting();
            ret.TopRecs = Factory.CBL.LoadUserParamInt("mysearch_toprecs", 50);
            ret.ClosedRecsClass = Factory.CBL.LoadUserParam("mysearch_closedrecs", "isclosed");

            return ret;
        }

        public string GetHtml4TheCombo(string entity, string tableid, string param1, string pids, string filterflag, string searchstring, string masterprefix, int masterpid) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            if (string.IsNullOrEmpty(param1) == false)
            {
                mq.param1 = param1;
            }

            var ce = Factory.EProvider.ByPrefix(mq.Prefix);
            if (ce.IsWithoutValidity == false)
            {
                mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy
            }
            

            if (filterflag != "0" && filterflag != "")
            {
                mq.SearchString = searchstring; //filtrování na straně serveru
                mq.TopRecordsOnly = 50; //maximálně prvních 50 záznamů, které vyhovují podmínce
            }

            var cols = _colsProvider.getDefaultPallete(true, mq);
            bool bolZrizovatel = false;
            switch (mq.Prefix)
            {
                case "a03":
                    if (masterprefix == "a06" && masterpid == 2)      //combo zřizovatele-> vyndat redizo sloupec
                    {
                        if (cols.Where(p => p.Field == "a03REDIZO").Count() > 0)
                        {
                            cols.Remove(cols.Where(p => p.Field == "a03REDIZO").First());
                            bolZrizovatel = true;
                        }
                    }
                    break;
                

            }
            mq.explicit_columns = cols;

            mq.explicit_orderby = ce.SqlOrderByCombo;


            mq.InhaleMasterEntityQuery(masterprefix, masterpid, null);


            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();

            if (mq.TopRecordsOnly > 0)
            {
                if (intRows >= mq.TopRecordsOnly)
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>Zobrazeno prvních {0} záznamů. Zpřesněte filtrovací podmínku.</small>", intRows));
                }
                else
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>Počet záznamů: {0}.</small>", intRows));
                }

            }

            s.Append(string.Format("<table id='{0}' class='table table-hover'>", tableid));

            s.Append("<thead><tr>");
            foreach (var col in cols)
            {
                s.Append(string.Format("<th>{0}</th>", col.Header));
            }
            s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>", tableid));
            string strTrClass = "";
            for (int i = 0; i < intRows; i++)
            {
                strTrClass = "txz";
                if (Convert.ToBoolean(dt.Rows[i]["isclosed"]) == true)
                {

                    strTrClass += " isclosed";
                }
                s.Append(string.Format("<tr class='{0}' data-v='{1}'", strTrClass, dt.Rows[i]["pid"]));
                if (mq.Prefix == "a03" && bolZrizovatel == false)
                {
                    //po výběru hodnoty z comba bude název a nikoliv pouze hodnota prvního sloupce
                    s.Append(string.Format(" data-t='{0} - {1}'", dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "REDIZO"], dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Name"]));
                    //s.Append(string.Format(" data-t='{0} - {1}'", dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "REDIZO"], dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Name"]));
                }
                if (mq.Prefix == "a18")
                {
                    s.Append(string.Format(" data-t='{0} - {1}'", dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Code"], dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Name"]));
                }
                if (mq.Prefix == "a37")
                {
                    s.Append(string.Format(" data-t='{0} - {1}'", dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "IZO"], dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Name"]));
                }
                if (mq.Prefix == "j23")
                {
                    s.Append(string.Format(" data-t='{0} ({1})'", dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Name"], dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Code"]));
                }

                s.Append(">");
                foreach (var col in cols)
                {
                    if (col.NormalizedTypeName == "num")
                    {
                        s.Append(string.Format("<td style='text-align:right;'>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }
                    else
                    {
                        s.Append(string.Format("<td>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }


                }
                s.Append("</tr>");
            }
            s.Append("</tbody></table>");

            return s.ToString();
        }



        //zdroj checkboxů pro taghelper mycombochecklist:
        public string GetHtml4Checkboxlist(string controlid, string entity, string selectedvalues, string masterprefix, int masterpid, string param1) //Vrací HTML seznam checkboxů pro taghelper: mycombochecklist
        {
            var mq = new BO.myQuery(entity);
            if (string.IsNullOrEmpty(param1) == false)
            {
                mq.param1 = param1;
            }
            
            mq.explicit_columns = _colsProvider.getDefaultPallete(false, mq);

            mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy
            mq.InhaleMasterEntityQuery(masterprefix, masterpid, null);

            List<int> selpids = null;
            if (String.IsNullOrEmpty(selectedvalues) == false)
            {
                selpids = BO.BAS.ConvertString2ListInt(selectedvalues);
            }

            string strTextField = "a__" + entity + "__" + mq.Prefix + "Name";
            if (mq.Prefix == "a17")
            {
                mq.explicit_selectsql = "a17UIVCode+' - '+ a17Name as CodePlusName";
                strTextField = "CodePlusName";
            }
            if (mq.Prefix == "x32")
            {
                strTextField = "a__x32ReportType__TreeItem";                
            }
            string strGroupField = null;
            string strLastGroup = null;
            string strGroup = null;
            string strChecked = "";
            int intValue = 0;
            string strText = "";
            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<div class='row' style='padding:0px;margin:0px;'>");
            sb.AppendLine("<div class='col-8'>");
            sb.AppendLine("<ul style='list-style:none;padding-left:0px;'>");


            for (int i = 0; i < intRows; i++)
            {
                intValue = Convert.ToInt32(dt.Rows[i]["pid"]);
                strText = Convert.ToString(dt.Rows[i][strTextField]);
                strChecked = "";
                if (strGroupField != null)
                {
                    if (dt.Rows[i][strGroupField] == null)
                    {
                        strGroup = null;
                    }
                    else
                    {
                        strGroup = Convert.ToString(dt.Rows[i][strGroupField]);
                    }
                    if (strGroup != strLastGroup)
                    {
                        sb.AppendLine("<li>");
                        sb.AppendLine("<div style='font-weight:bold;background-color:#ADD8E6;'><span style='padding-left:10px;'>" + strGroup + "</span></div>");
                        sb.AppendLine("</li>");
                    }

                }
                if (selpids != null && selpids.Where(p => p == intValue).Count() > 0)
                {
                    strChecked = "checked";
                }

                sb.AppendLine("<li>");
                sb.Append(string.Format("<input type='checkbox' id='chk{0}_{1}' name='chk{0}' value='{1}' {2} />", controlid, intValue, strChecked));
                sb.Append(string.Format("<label style='min-width:200px;' for='chk{0}_{1}'>{2}</label>", controlid, intValue, strText));

                sb.AppendLine("</li>");
            }



            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-4'>");
            sb.AppendLine(string.Format("<button type='button' id='cmdCheckAll{0}' class='btn btn-light'>Zaškrtnout vše</button>", controlid));
            sb.AppendLine(string.Format("<button type='button' id='cmdUnCheckAll{0}' class='btn btn-light'>Odškrtnout vše</button>", controlid));
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            return sb.ToString();
        }

        public string GetHtml4SearchSetting(string controlid, string entity)
        {
            var setting = LoadMySearchSetting();
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<div style='padding:20px;'>");
            sb.Append("<h5>FULLTEXT nastavení</h5>");
            sb.Append("<hr>");
            if (entity.Substring(0, 3) == "a01")
            {
                sb.Append("U hledání akcí systém pracuje s poli: <b>[Signatura], [Kód spisu], [Jméno vedoucího akce], [Jméno člena akce], [Název školy], [REDIZO]</b>.");
            }
            if (entity.Substring(0, 3) == "a03")
            {
                sb.Append("U hledání institucí systém pracuje s poli: <b>[Název školy], [REDIZO],[IZO činnosti školy] [IČ], [Obec], [Ulice]</b>.");
            }
            if (entity.Substring(0, 3) == "j02")
            {
                sb.Append("U hledání osoby systém pracuje s poli: <b>[Jméno], [Příjmení], [E-mail], [Osobní číslo]</b>.");
            }
            sb.Append("<br>");
            sb.Append("<span class='text-danger'>Fulltext hledání funguje na principu shody celých slov!</span>");
            sb.Append("<br><span class='text-success'>Mezera mezi slovy je operátor 'and', můžete psát mezi slovy i přímo 'or'.</span>");
            sb.Append("<br><span class='text-info'>Narozdíl od GRID filtrování, kde se pracuje s částečnou shodou (LIKE).</span>");
            sb.Append("<hr>");
            sb.Append("<label>Zobrazovat maximálně </label><select id='mysearch_toprecs'>");
            sb.Append(basUI.render_select_option("20", "20", setting.TopRecs.ToString()));
            sb.Append(basUI.render_select_option("50", "50", setting.TopRecs.ToString()));
            sb.Append(basUI.render_select_option("100", "100", setting.TopRecs.ToString()));
            sb.Append("</select> záznamů.<br>");

            sb.Append("<label>Uzavřené záznamy odlišovat: </label><select id='mysearch_closedrecs'>");
            sb.Append(basUI.render_select_option("isclosed", "Přeškrtlé", setting.ClosedRecsClass));
            sb.Append(basUI.render_select_option("isclosed_by_color", "Barvou", setting.ClosedRecsClass));
            sb.Append("</select>.");
            sb.AppendLine("<button type='button' class='btn btn-primary' onclick='mysearch_save_setting()'>Uložit nastavení</button>");


            sb.AppendLine("</div>");
            return sb.ToString();
        }

        public string GetHtml4Search(string entity, string searchstring) //Vrací HTML zdroj tabulky pro MySearch
        {
            var setting = LoadMySearchSetting();
            var mq = new BO.myQuery(entity);
            mq.SearchImplementation = Factory.App.Implementation;
            //mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy

            mq.SearchString = searchstring; //filtrování na straně serveru
            mq.TopRecordsOnly = setting.TopRecs; //maximálně prvních 50 záznamů, které vyhovují podmínce

            var cols = _colsProvider.getDefaultPallete(true, mq);
            mq.explicit_columns = cols;

            mq.explicit_orderby = Factory.EProvider.ByPrefix(mq.Prefix).SqlOrderByCombo;

            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();

            if (searchstring == null || searchstring.Length <= 3)
            {
                s.AppendLine("<small style='margin-left:10px;'>Musíte zadat minimálně 4 znaky.</small>");
                return s.ToString();
            }

            if (mq.TopRecordsOnly > 0)
            {
                if (intRows >= mq.TopRecordsOnly)
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>Zobrazeno prvních {0} záznamů. Zpřesněte filtrovací podmínku.</small>", intRows));
                }
                else
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>Počet záznamů: {0}.</small>", intRows));
                }

            }

            s.Append(string.Format("<table id='{0}' class='table table-hover'>", "hovado"));

            s.Append("<thead><tr>");
            foreach (var col in cols)
            {
                s.Append(string.Format("<th>{0}</th>", col.Header));
            }
            s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>", "hovado"));
            string strTrClass = "";
            for (int i = 0; i < intRows; i++)
            {
                strTrClass = "txs";
                if (Convert.ToBoolean(dt.Rows[i]["isclosed"]) == true)
                {
                    strTrClass += " " + setting.ClosedRecsClass;
                }

                s.Append(string.Format("<tr class='{0}' data-v='{1}'", strTrClass, dt.Rows[i]["pid"]));


                s.Append(">");
                foreach (var col in cols)
                {
                    if (col.NormalizedTypeName == "num")
                    {
                        s.Append(string.Format("<td style='text-align:right;'>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }
                    else
                    {
                        s.Append(string.Format("<td>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }


                }
                s.Append("</tr>");
            }
            s.Append("</tbody></table>");

            return s.ToString();
        }

        public string GetAutoCompleteHtmlItems(int o15flag, string tableid) //Vrací HTML zdroj tabulky pro MyAutoComplete
        {
            var lis = Factory.FBL.GetListAutoComplete(o15flag);
            var s = new System.Text.StringBuilder();

            s.Append(string.Format("<table id='{0}' class='table table-sm table-hover'>", tableid));
            foreach (var item in lis)
            {
                s.Append(string.Format("<tr class='txz'><td>{0}</td></tr>", item.Value));
            }
            s.Append("</table>");

            return s.ToString();
        }



    }
}