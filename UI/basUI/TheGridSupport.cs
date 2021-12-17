using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;



namespace UI
{
    public class TheGridSupport
    {
        private System.Text.StringBuilder _s;
        private readonly BL.TheColumnsProvider _colsProvider;
        private BL.Factory _Factory { get; set; }
        private UI.Models.TheGridViewModel _grid;

        public TheGridInput gridinput { get; set; }
        //public string extendpagerhtml { get; set; }
        //public string oncmclick { get; set; }
        //public string ondblclick { get; set; }
        //public string fixedcolumns { get; set; }

        public TheGridSupport(TheGridInput input, BL.Factory f, BL.TheColumnsProvider cp)
        {
            _Factory = f;
            _colsProvider = cp;
            this.gridinput = input;
        }

        public TheGridOutput GetFirstData(TheGridState gridState) //vrátí grid html pro úvodní načtení na stránku
        {
            if (gridState == null)
            {
                return render_thegrid_error("gridState is null!");
            }

            if (!string.IsNullOrEmpty(this.gridinput.fixedcolumns))
            {
                gridState.j72Columns = this.gridinput.fixedcolumns;
            }

            return render_thegrid_html(gridState, this.gridinput.query);
        }



        public TheGridOutput Event_HandleTheGridOper(TheGridUIContext tgi)     //grid událost: třídění, změna stránky a pagesize
        {

            var gridState = _Factory.j72TheGridTemplateBL.LoadState(tgi.j72id, _Factory.CurrentUser.pid);   //načtení naposledy uloženého grid stavu uživatele

            if (!string.IsNullOrEmpty(this.gridinput.fixedcolumns))
            {
                gridState.j72Columns = this.gridinput.fixedcolumns;
            }
            switch (tgi.key)
            {

                case "pagerindex":
                    gridState.j75CurrentPagerIndex = BO.BAS.InInt(tgi.value);
                    break;
                case "pagesize":
                    gridState.j75PageSize = BO.BAS.InInt(tgi.value);
                    break;
                case "sortfield":
                    if (gridState.j75SortDataField != tgi.value)
                    {
                        gridState.j75SortOrder = "asc";
                        gridState.j75SortDataField = tgi.value;
                    }
                    else
                    {
                        if (gridState.j75SortOrder == "desc")
                        {
                            gridState.j75SortDataField = "";//vyčisitt třídění, třetí stav
                            gridState.j75SortOrder = "";
                        }
                        else
                        {
                            if (gridState.j75SortOrder == "asc")
                            {
                                gridState.j75SortOrder = "desc";
                            }
                        }
                    }


                    break;
                case "filter":
                    break;  //samostatná událost Event_HandleTheGridFilter
            }

            if (_Factory.j72TheGridTemplateBL.SaveState(gridState, _Factory.CurrentUser.pid) > 0)   //uložení změny grid stavu
            {
                return render_thegrid_html(gridState, gridinput.query);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se uložit GRIDSTATE");
            }


        }

        public TheGridOutput Event_HandleTheGridFilter(TheGridUIContext tgi, List<BO.TheGridColumnFilter> filter)    //grid událost: Změna sloupcového filtru
        {

            var gridState = _Factory.j72TheGridTemplateBL.LoadState(tgi.j72id, _Factory.CurrentUser.pid);


            if (string.IsNullOrEmpty(this.gridinput.fixedcolumns) == false)
            {
                gridState.j72Columns = this.gridinput.fixedcolumns;
            }
            var lis = new List<string>();
            foreach (var c in filter)
            {
                lis.Add(c.field + "###" + c.oper + "###" + c.value);

            }
            gridState.j75CurrentPagerIndex = 0; //po změně filtrovací podmínky je nutné vyčistit paměť stránky
            gridState.j75CurrentRecordPid = 0;

            gridState.j75Filter = string.Join("$$$", lis);

            if (_Factory.j72TheGridTemplateBL.SaveState(gridState, _Factory.CurrentUser.pid) > 0)
            {
                return render_thegrid_html(gridState, gridinput.query);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se zpracovat filtrovací podmínku.");
            }
        }


        private TheGridOutput render_thegrid_html(BO.TheGridState gridState, BO.baseQuery mq) //vrací kompletní html gridu: header+body+footer+message
        {
            var ret = new TheGridOutput();
            _grid = new TheGridViewModel();
            _grid.GridState = gridState;


            ret.sortfield = gridState.j75SortDataField;
            ret.sortdir = gridState.j75SortOrder;


            _grid.Columns = _colsProvider.ParseTheGridColumns(mq.Prefix, gridState.j72Columns, _Factory.CurrentUser.j03LangIndex);
            mq.explicit_columns = _grid.Columns.ToList();

            if (!String.IsNullOrEmpty(gridState.j75Filter))
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, mq.explicit_columns, _Factory.CurrentUser.j03LangIndex);
            }

            if (gridState.j72HashJ73Query)
            {
                mq.lisJ73 = _Factory.j72TheGridTemplateBL.GetList_j73(gridState.j72ID, gridState.j72Entity.Substring(0, 3));
                _grid.GridMessage = _Factory.j72TheGridTemplateBL.getFiltrAlias(gridState.j72Entity.Substring(0, 3), mq);


            }

            var dtFooter = _Factory.gridBL.GetList(mq, true);
            int intVirtualRowsCount = 0;
            if (dtFooter.Columns.Count > 0)
            {
                intVirtualRowsCount = Convert.ToInt32(dtFooter.Rows[0]["RowsCount"]);
            }
            else
            {
                _Factory.CurrentUser.AddMessage("GRID Error: Dynamic SQL failed.");
            }

            if (intVirtualRowsCount > 500)
            {   //dotazy nad 500 záznamů budou mít zapnutý OFFSET režim stránkování
                mq.OFFSET_PageSize = gridState.j75PageSize;
                mq.OFFSET_PageNum = gridState.j75CurrentPagerIndex / gridState.j75PageSize;
            }

            //třídění řešit až po spuštění FOOTER summary DOTAZu
            if (String.IsNullOrEmpty(gridState.j75SortDataField) == false && _grid.Columns.Where(p => p.UniqueName == gridState.j75SortDataField).Count() > 0)
            {
                var c = _grid.Columns.Where(p => p.UniqueName == gridState.j75SortDataField).First();
                mq.explicit_orderby = c.getFinalSqlSyntax_ORDERBY() + " " + gridState.j75SortOrder;
            }

            var dt = _Factory.gridBL.GetList(mq);



            if (_grid.GridState.j75CurrentRecordPid > 0 && intVirtualRowsCount > gridState.j75PageSize)
            {
                //aby se mohlo skočit na cílový záznam, je třeba najít stránku, na které se záznam nachází
                System.Data.DataRow[] recs = dt.Select("pid=" + _grid.GridState.j75CurrentRecordPid.ToString());
                if (recs.Count() > 0)
                {
                    var intIndex = dt.Rows.IndexOf(recs[0]);
                    _grid.GridState.j75CurrentPagerIndex = intIndex - (intIndex % _grid.GridState.j75PageSize);
                }
            }

            _s = new System.Text.StringBuilder();

            Render_DATAROWS(dt, mq);
            ret.body = _s.ToString();
            _s = new System.Text.StringBuilder();

            Render_TOTALS(dtFooter);
            ret.foot = _s.ToString();
            _s = new System.Text.StringBuilder();

            RENDER_PAGER(intVirtualRowsCount);
            ret.pager = _s.ToString();
            return ret;
        }

        private void Render_DATAROWS(System.Data.DataTable dt, BO.baseQuery mq)
        {
            int intRows = dt.Rows.Count;
            int intStartIndex = 0;
            int intEndIndex = 0;

            if (mq.OFFSET_PageSize > 0)
            {   //Zapnutý OFFSET - pouze jedna stránka díky OFFSET
                intStartIndex = 0;
                intEndIndex = intRows - 1;
            }
            else
            {   //bez OFFSET               
                intStartIndex = _grid.GridState.j75CurrentPagerIndex;
                intEndIndex = intStartIndex + _grid.GridState.j75PageSize - 1;
                if (intEndIndex + 1 > intRows) intEndIndex = intRows - 1;
            }

            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                System.Data.DataRow dbRow = dt.Rows[i];
                string strRowClass = "selectable";
                if (Convert.ToBoolean(dbRow["isclosed"]) == true)
                {
                    strRowClass += " trbin";
                }
                if (mq.Prefix == "a01" && Convert.ToBoolean(dbRow["istemporary"]))  //temp akce
                {
                    if (dbRow["temporaryflag"] != System.DBNull.Value && Convert.ToInt32(dbRow["temporaryflag"]) == 1)
                    {
                        strRowClass += " trsimulation"; //dočasná simulace formuláře
                    }
                    else
                    {
                        strRowClass += " trtemporary"; //rozpracovaná temp akce - úraz
                    }
                   
                }
                if (mq.Prefix == "a03" && Convert.ToBoolean(dbRow["istestrecord"]))
                {
                    strRowClass += " trtestrecord";
                }
                if (string.IsNullOrEmpty(this.gridinput.ondblclick))
                {
                    _s.Append(string.Format("<tr id='r{0}' class='{1}'>", dbRow["pid"], strRowClass));
                }
                else
                {
                    _s.Append(string.Format("<tr id='r{0}' class='{1}' ondblclick='{2}(this)'>", dbRow["pid"], strRowClass, this.gridinput.ondblclick));
                }



                if (_grid.GridState.j72SelectableFlag > 0)
                {
                    _s.Append(string.Format("<td class='td0'><input type='checkbox' style='padding:0px;' id='chk{0}'/></td>", dbRow["pid"]));
                }
                else
                {
                    _s.Append("<td class='td0' ></td>");
                }

                if ((mq.Prefix == "a01" || mq.Prefix == "a11") && dbRow["bgcolor"] != System.DBNull.Value)
                {
                    _s.Append(string.Format("<td class='td1' style='width:20px;background-color:{0}'>", dbRow["bgcolor"]));
                }
                else
                {
                    _s.Append("<td class='td1' style='width:20px;'>");
                }

                if (mq.Prefix == "a01")
                {
                    if (dbRow["parentpid"] != DBNull.Value)
                    {
                        _s.Append("<img src='/images/child.png'/>");
                    }
                    else
                    {
                        if (Convert.ToInt32(dbRow["childscount"]) > 0) _s.Append("<img src='/images/mother.png'/>");
                    }
                }
                if (mq.Prefix == "a03")
                {
                    if (dbRow["parentflag"] != DBNull.Value)
                    {
                        if (Convert.ToInt32(dbRow["parentflag"]) == 1)
                        {
                            _s.Append("<img src='/images/a03ParentFlag1.png'/>");
                        }
                        if (Convert.ToInt32(dbRow["parentflag"]) == 2)
                        {
                            _s.Append("<img src='/images/a03ParentFlag2.png'/>");
                        }

                    }


                }
                _s.Append("</td>");


                if (!string.IsNullOrEmpty(this.gridinput.oncmclick))
                {
                    _s.Append(string.Format("<td class='td2' style='width:20px;'><a class='cm' onclick='{0}'>&#9776;</a></td>", this.gridinput.oncmclick));      //hamburger menu
                }
                else
                {
                    _s.Append("<td class='td2' style='width:20px;'>");  //bez hamburger menu
                }



                foreach (var col in _grid.Columns)
                {
                    _s.Append("<td");
                    if (col.CssClass != null)
                    {
                        _s.Append(string.Format(" class='{0}'", col.CssClass));
                    }

                    if (i == intStartIndex)   //první řádek musí mít explicitně šířky, aby to z něj zdědili další řádky
                    {
                        _s.Append(string.Format(" style='width:{0}'", col.ColumnWidthPixels));
                    }
                    _s.Append(string.Format(">{0}</td>", BO.BAS.ParseCellValueFromDb(dbRow, col)));


                }
                _s.Append("</tr>");
            }
        }

        private void Render_TOTALS(System.Data.DataTable dt)
        {
            if (dt.Columns.Count == 0)
            {
                return;
            }
            _s.Append("<tr id='tabgrid1_tr_totals'>");
            _s.Append(string.Format("<th class='th0' title='{0}' colspan=3 style='width:60px;'><span class='badge bg-primary'>{1}</span></th>", _Factory.tra("Celkový počet záznamů"), string.Format("{0:#,0}", dt.Rows[0]["RowsCount"])));
            //_s.Append("<th style='width:20px;'></th>");
            //_s.Append("<th class='th0' style='width:20px;'></th>");
            string strVal = "";
            foreach (var col in _grid.Columns)
            {
                _s.Append("<th");
                if (col.CssClass != null)
                {
                    _s.Append(string.Format(" class='{0}'", col.CssClass));
                }

                strVal = "&nbsp;";
                if (dt.Rows[0][col.UniqueName] != System.DBNull.Value)
                {
                    strVal = BO.BAS.ParseCellValueFromDb(dt.Rows[0], col);
                }
                _s.Append(string.Format(" style='width:{0}'>{1}</th>", col.ColumnWidthPixels, strVal));


            }
            _s.Append("</tr>");
        }






        private void render_select_option(string strValue, string strText, string strSelValue)
        {
            if (strSelValue == strValue)
            {
                _s.Append(string.Format("<option selected value='{0}'>{1}</option>", strValue, strText));
            }
            else
            {
                _s.Append(string.Format("<option value='{0}'>{1}</option>", strValue, strText));
            }

        }

        private void RENDER_PAGER(int intRowsCount) //pager má maximálně 10 čísel, j72PageNum začíná od 0
        {
            int intPageSize = _grid.GridState.j75PageSize;

            _s.Append("<select title='" + _Factory.tra("Stránkování záznamů") + "' onchange='tg_pagesize(this)'>");
            render_select_option("50", "50", intPageSize.ToString());
            render_select_option("100", "100", intPageSize.ToString());
            render_select_option("200", "200", intPageSize.ToString());
            render_select_option("500", "500", intPageSize.ToString());
            render_select_option("1000", "1000", intPageSize.ToString());
            _s.Append("</select>");
            if (intRowsCount < 0)
            {
                Render_ExtendPagerHtml();
                RenderGridMessage();
                return;
            }

            if (intRowsCount <= intPageSize)
            {
                Render_ExtendPagerHtml();
                RenderGridMessage();
                return;
            }

            _s.Append("<button title='" + _Factory.tra("První") + "' class='btn btn-light tgp' style='margin-left:6px;' onclick='tg_pager(\n0\n)'>&lt;&lt;</button>");

            int intCurIndex = _grid.GridState.j75CurrentPagerIndex;
            int intPrevIndex = intCurIndex - intPageSize;
            if (intPrevIndex < 0) intPrevIndex = 0;
            _s.Append(string.Format("<button title='" + _Factory.tra("Předchozí") + "' class='btn btn-light tgp' style='margin-right:10px;' onclick='tg_pager(\n{0}\n)'>&lt;</button>", intPrevIndex));

            if (intCurIndex >= intPageSize * 10)
            {
                intPrevIndex = intCurIndex - 10 * intPageSize;
                _s.Append(string.Format("<button class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>...</button>", intPrevIndex));
            }


            int intStartIndex = 0;
            for (int i = 0; i <= intRowsCount; i += intPageSize * 10)
            {
                if (intCurIndex >= i && intCurIndex < i + intPageSize * 10)
                {
                    intStartIndex = i;
                    break;
                }
            }

            int intEndIndex = intStartIndex + (9 * intPageSize);
            if (intEndIndex + 1 > intRowsCount) intEndIndex = intRowsCount - 1;


            int intPageNum = intStartIndex / intPageSize; string strClass;

            for (var i = intStartIndex; i <= intEndIndex; i += intPageSize)
            {
                intPageNum += 1;

                if (intCurIndex >= i && intCurIndex < i + intPageSize)
                {
                    strClass = "btn btn-secondary tgp";
                }
                else
                {
                    strClass = "btn btn-light tgp";
                }

                _s.Append(string.Format("<button type='button' class='{0}' onclick='tg_pager(\n{1}\n)'>{2}</button>", strClass, i, intPageNum));

            }
            if (intEndIndex + 1 < intRowsCount)
            {
                intEndIndex += intPageSize;
                if (intEndIndex + 1 > intRowsCount) intEndIndex = intRowsCount - intPageSize;
                _s.Append(string.Format("<button type='button' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>...</button>", intEndIndex));
            }

            int intNextIndex = intCurIndex + intPageSize;
            if (intNextIndex + 1 > intRowsCount) intNextIndex = intRowsCount - intPageSize;
            _s.Append(string.Format("<button type='button' title='" + _Factory.tra("Další") + "' class='btn btn-light tgp' style='margin-left:10px;' onclick='tg_pager(\n{0}\n)'>&gt;</button>", intNextIndex));

            int intLastIndex = intRowsCount - (intRowsCount % intPageSize);  //% je zbytek po celočíselném dělení
            _s.Append(string.Format("<button type='button' title='" + _Factory.tra("Poslední") + "' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>&gt;&gt;</button>", intLastIndex));

            Render_ExtendPagerHtml();

            RenderGridMessage();
        }

        private void RenderGridMessage()
        {
            if (!_grid.GridState.j72IsSystem)
            {
                _s.Append("<span id='gridname'>" + _grid.GridState.j72Name + "</span>");
            }

            if (_grid.GridMessage != null)
            {
                // _s.Append("<div class='text-nowrap bd-highlight'>" + _grid.GridMessage + "</div>");
                _s.Append("<span id='gridmessage'>" + _grid.GridMessage + "</span>");
            }
        }
        private void Render_ExtendPagerHtml()
        {
            if (string.IsNullOrEmpty(this.gridinput.extendpagerhtml))
            {
                return;
            }
            _s.Append(this.gridinput.extendpagerhtml);

            //if (_grid.GridState.MasterViewFlag < 3)
            //{
            //    switch (_grid.Entity.Substring(0, 3))
            //    {
            //        case "a01":
            //        case "a03":
            //        case "j02":
            //            if (_grid.GridState.MasterViewFlag == 2)
            //            {
            //                _s.Append("<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='tg_switchflag(\"" + _grid.Entity.Substring(0, 3) + "\",0)'>" + _Factory.tra("Vypnout spodní panel") + "</button>");
            //            }
            //            else
            //            {
            //                _s.Append("<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='tg_switchflag(\"" + _grid.Entity.Substring(0, 3) + "\",1)'>" + _Factory.tra("Zapnout spodní panel") + "</button>");
            //            }
            //            break;
            //    }
            //}
        }


        private TheGridOutput render_thegrid_error(string strError)
        {
            var ret = new TheGridOutput();
            ret.message = strError;
            if (_Factory.CurrentUser.Messages4Notify.Count > 0)
            {
                ret.message += " | " + string.Join(",", _Factory.CurrentUser.Messages4Notify.Select(p => p.Value));
            }
            return ret;
        }



        public string Event_HandleTheGridMenu(TheGridUIContext tgi)
        {
            int j72id = tgi.j72id;
            var sb = new System.Text.StringBuilder();
            var recJ72 = _Factory.j72TheGridTemplateBL.LoadState(j72id, _Factory.CurrentUser.pid);

            sb.AppendLine("<div style='background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>" + _Factory.tra("VYBRANÉ (zaškrtlé) záznamy") + "</div>");
            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\",\"selected\")'>" + _Factory.tra("MS-EXCEL Export") + "</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\",\"selected\")'>" + _Factory.tra("CSV Export") + "</a>", j72id));
            sb.AppendLine("</div>");
            sb.AppendLine("<hr class='hr-mini' />");

            if ("a01,a03".Contains(recJ72.j72Entity.Substring(0, 3)))
            {
                sb.AppendLine("<a class='nav-link' href=\"javascript:tg_batchupdate('" + recJ72.j72Entity.Substring(0, 3) + "')\" >" + _Factory.tra("Hromadné operace") + "★</a>");

            }
            if ("j02,a01,a03,f06".Contains(recJ72.j72Entity.Substring(0, 3)))
            {
                sb.AppendLine("<a class='nav-link' href='javascript:tg_tagging();'>" + _Factory.tra("Hromadná kategorizace záznamů") + "★</a>");

            }
            if (tgi.fixedcolumns == null)
            {
                //pokud grid nemá fixní sloupce natvrdo, pak možnost navrhovat sloupce
                sb.AppendLine(string.Format("<div style='margin-top:20px;background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>{0}</div>", _Factory.tra("Seznam pojmenovaných GRID šablon")));

                var lis = _Factory.j72TheGridTemplateBL.GetList(recJ72.j72Entity, recJ72.j03ID, recJ72.j72MasterEntity);
                sb.AppendLine("<table style='width:100%;margin-bottom:20px;'>");
                string strGridNavrhar = _Factory.tra("Grid návrhář");
                foreach (var c in lis)
                {
                    sb.AppendLine("<tr>");
                    if (c.j72IsTemplate4SystemGrid)
                    {
                        sb.Append("<td><span class='k-icon k-i-table-light-dialog' style='color:red;'></span></td>");
                    }
                    else
                    {
                        if (c.j72HashJ73Query)
                        {
                            sb.Append("<td><span class='k-icon k-i-filter'></span></td>");
                        }
                        else
                        {
                            sb.Append("<td><span class='k-icon k-i-table'></span></td>");
                        }
                    }

                    if (c.j72IsSystem)
                    {
                        c.j72Name = _Factory.tra("Výchozí GRID");
                    }
                    if (c.pid == recJ72.pid)
                    {
                        c.j72Name += " ✔";
                    }
                    sb.Append(string.Format("<td><a class='nav-link py-0' href='javascript:change_grid({0})'>{1}</a></td>", c.pid, c.j72Name));

                    sb.AppendLine(string.Format("<td style='width:30px;'><a title='" + strGridNavrhar + "' class='btn btn-sm btn-primary py-0' href='javascript:_window_open(\"/TheGridDesigner/Index?j72id={0}\",2);'>...</a></td>", c.pid));
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");
            }




            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\")'>" + _Factory.tra("MS-EXCEL Export (vše)") + "</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\")'>" + _Factory.tra("CSV Export (vše)") + "</a>", j72id));
            sb.AppendLine("</div>");



            sb.AppendLine("<hr class='hr-mini' />");
            sb.AppendLine("<a  href='javascript:tg_select(20)'>" + _Factory.tra(string.Format("Vybrat prvních {0}", 20)) + "</a>⌾");
            sb.AppendLine("<a  href='javascript:tg_select(50)'>" + _Factory.tra(string.Format("Vybrat prvních {0}", 50)) + "</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(100)'>" + _Factory.tra(string.Format("Vybrat prvních {0}", 100)) + "</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(1000)'>" + _Factory.tra("Vybrat všechny záznamy na stránce") + "</a>");

            return sb.ToString();
        }


        public TheGridExportedFile Event_HandleTheGridExport(string format, int j72id, string pids)
        {
            var gridState = this._Factory.j72TheGridTemplateBL.LoadState(j72id, _Factory.CurrentUser.pid);

            if (String.IsNullOrEmpty(pids) == false)
            {
                this.gridinput.query.SetPids(pids);
            }


            System.Data.DataTable dt = prepare_datatable_4export(gridState);
            string strTempFileName = BO.BAS.GetGuid();
            string filepath = _Factory.App.TempFolder + "\\" + strTempFileName + "." + format;

            var cExport = new UI.dataExport();
            string strFileClientName = "gridexport_" + this.gridinput.query.Prefix + "." + format;

            if (format == "csv")
            {
                if (cExport.ToCSV(dt, filepath, this.gridinput.query))
                {
                    //return File(System.IO.File.ReadAllBytes(filepath), "application/CSV", strFileClientName);
                    return new TheGridExportedFile() { contenttype = "application/CSV", downloadfilename = strFileClientName, tempfilename = strTempFileName + "." + format };


                }
            }
            if (format == "xlsx")
            {
                if (cExport.ToXLSX(dt, filepath, this.gridinput.query))
                {

                    //return File(System.IO.File.ReadAllBytes(filepath), "application/vnd.ms-excel", strFileClientName);
                    return new TheGridExportedFile() { contenttype = "application/vnd.ms-excel", downloadfilename = strFileClientName, tempfilename = strTempFileName + "." + format };
                }
            }


            return null;

        }

        private System.Data.DataTable prepare_datatable_4export(BO.TheGridState gridState)
        {

            this.gridinput.query.explicit_columns = _colsProvider.ParseTheGridColumns(this.gridinput.query.Prefix, gridState.j72Columns, _Factory.CurrentUser.j03LangIndex);

            if (!string.IsNullOrEmpty(gridState.j75SortDataField))  //v gridu uživatel má nějaké třídění sloupce
            {
                if (this.gridinput.query.explicit_columns.Any(p => p.UniqueName == gridState.j75SortDataField))
                {
                    this.gridinput.query.explicit_orderby = this.gridinput.query.explicit_columns.Where(p => p.UniqueName == gridState.j75SortDataField).First().getFinalSqlSyntax_ORDERBY() + " " + gridState.j75SortOrder;
                }
                //this.gridinput.query.explicit_orderby = _colsProvider.ByUniqueName(gridState.j75SortDataField).getFinalSqlSyntax_ORDERBY() + " " + gridState.j75SortOrder;
            }
            //if (string.IsNullOrEmpty(gridState.j75SortDataField) == false && _colsProvider.ByUniqueName(gridState.j75SortDataField) !=null)
            //{

            //    this.gridinput.query.explicit_orderby = _colsProvider.ByUniqueName(gridState.j75SortDataField).getFinalSqlSyntax_ORDERBY() + " " + gridState.j75SortOrder;
            //}
            if (String.IsNullOrEmpty(gridState.j75Filter) == false)
            {
                this.gridinput.query.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, this.gridinput.query.explicit_columns, _Factory.CurrentUser.j03LangIndex);
            }
            if (gridState.j72HashJ73Query)
            {
                this.gridinput.query.lisJ73 = _Factory.j72TheGridTemplateBL.GetList_j73(gridState.j72ID, gridState.j72Entity.Substring(0, 3));
            }

            return _Factory.gridBL.GetList(this.gridinput.query);
        }

    }
}
