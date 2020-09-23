using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;




using UI.Models;
using System.ComponentModel;
using BO;

namespace UI.Controllers
{
    public class TheGridController : BaseController        
    {
       
        private System.Text.StringBuilder _s;
        private UI.Models.TheGridViewModel _grid;
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        public IActionResult FlatView(string prefix,int go2pid)    //pouze grid bez subform
        {
            var v = inhaleGridViewInstance(prefix, go2pid);
            v.j72id = Factory.CBL.LoadUserParamInt("flatview-j72id-" + prefix);
            return View(v);
        }
        
        public IActionResult MasterView(string prefix,int go2pid)    //grid horní + spodní panel
        {
            TheGridInstanceViewModel v = inhaleGridViewInstance(prefix, go2pid);
            v.j72id = Factory.CBL.LoadUserParamInt("masterview-j72id-" + prefix);
            BO.TheEntity ce = Factory.EProvider.ByPrefix(prefix);
            var tabs = new List<NavTab>();
            
            switch (prefix)
            {
                case "a01":
                    tabs.Add(AddTab("Info", "viewInfo", "/a01/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Historie událostí","viewHistory","/a01/TabHistory?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Účastníci akce", "viewUcastnici", "/a01/TabUcastnici?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Časový plán", "viewCapacity","/a35/TabCapacity?pid=" + AppendPid2Url(v.go2pid)));

                    tabs.Add(AddTab("Formuláře", "viewFormulare", "/a01/TabForms?pid=" + AppendPid2Url(v.go2pid)));

                    //tabs.Add(new NavTab() { Name = "Přílohy", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(AddTab("Úkoly/Lhůty","h04ToDo","SlaveView?prefix=h04"));
                    tabs.Add(AddTab("Související akce", "a01Event", "SlaveView?prefix=a01"));
                    
                    break;
                case "a03":
                    tabs.Add(AddTab("Info", "viewInfo", "/a03/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Svázané akce", "a01Event", "SlaveView?prefix=a01"));
                    tabs.Add(AddTab("Činnosti školy", "a37InstitutionDepartment", "SlaveView?prefix=a37"));
                    tabs.Add(AddTab("Vzdělávací obory","a19DomainToInstitutionDepartment", "SlaveView?prefix=a19"));
                    tabs.Add(AddTab("Kontaktní osoby","a39InstitutionPerson","SlaveView?prefix=a39"));
                    tabs.Add(AddTab("Učitelé", "k01Teacher", "SlaveView?prefix=k01"));
                    tabs.Add(AddTab("INEZ", "a42Qes", "SlaveView?prefix=a42"));
                    tabs.Add(AddTab( "Pojmenované seznamy",  "a29InstitutionList","SlaveView?prefix=a29"));

                    break;
                case "a42":
                    tabs.Add(AddTab("Info", "viewInfo","/a42/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Svázané akce", "a01Event", "SlaveView?prefix=a01"));
                    tabs.Add(AddTab("Outbox", "x40MailQueue","SlaveView?prefix=x40"));
                    break;
                case "j02":
                    tabs.Add(AddTab("Info", "viewInfo", "/j02/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab(string.Format(Factory.tra("{0}: je zakladatelem"), Factory.App.Terminology_Akce), "a01Event", "SlaveView?prefix=a01",false));
                    tabs.Add(AddTab(string.Format(Factory.tra("{0}: je účastníkem"), Factory.App.Terminology_Akce), "a01Event", "SlaveView?prefix=a01", false));
                    tabs.Add(AddTab("Instituce", "a03Institution", "SlaveView?prefix=a03"));
                    tabs.Add(AddTab("Úkoly/Lhůty", "h04ToDo", "SlaveView?prefix=h04"));
                    tabs.Add(AddTab("Outbox", "x40MailQueue", "SlaveView?prefix=x40"));
                    tabs.Add(AddTab("PING Log", "j92PingLog", "SlaveView?prefix=j92"));
                    tabs.Add(AddTab("LOGIN Log", "j90LoginAccessLog", "SlaveView?prefix=j90"));
                    break;
                
                
                
            }
            string strDefTab = Factory.CBL.LoadUserParam("masterview-tab-" + prefix);
            var deftab = tabs[0];
            
            foreach (var tab in tabs)
            {
               if (tab.Url.Contains("?pid")==false)
                {
                    tab.Url += "&master_entity=" + ce.TableName + "&master_pid=" + AppendPid2Url(v.go2pid);

                }

                if (strDefTab !="" && tab.Entity== strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka
                }
            }
            deftab.CssClass += " active";
            if (go2pid > 0)
            {
                v.go2pid_url_in_iframe = deftab.Url;
                
            }

            v.NavTabs = tabs;
            return View(v);
        }

        private string AppendPid2Url(int go2pid)
        {
            if (go2pid > 0)
            {
                return go2pid.ToString();
            }
            else
            {
                return  "@pid";
            }
        }
        public IActionResult SlaveView(string master_entity,int master_pid, string prefix, int go2pid,string master_flag)    //podřízený subform v rámci MasterView
        {
            TheGridInstanceViewModel v = inhaleGridViewInstance(prefix, go2pid);
            v.j72id = Factory.CBL.LoadUserParamInt("slaveview-j72id-" + prefix + "-" + master_entity);
            v.master_entity = master_entity;
            v.master_pid = master_pid;
            v.master_flag = master_flag;
            if (String.IsNullOrEmpty(v.master_entity) || v.master_pid == 0)
            {
                AddMessage("Musíte vybrat záznam z nadřízeného panelu.");
            }
            
            return View(v);
        }
        private TheGridInstanceViewModel inhaleGridViewInstance(string prefix,int go2pid)
        {
            var v = new TheGridInstanceViewModel() { prefix = prefix, go2pid = go2pid,contextmenuflag=1 };
            BO.TheEntity c = Factory.EProvider.ByPrefix(prefix);
            v.entity = c.TableName;
            v.entityTitle = c.AliasPlural;

            if (v.entity == "")
            {
                AddMessage("Grid entita nebyla nalezena.");
            }
            if (c.IsGlobalPeriodQuery)
            {
                v.period = new PeriodViewModel();
                v.period.IsShowButtonRefresh = true;
                BO.ThePeriod per = InhaleGridPeriodDates();
                v.period.PeriodValue = per.pid;
                v.period.d1 = per.d1;
                v.period.d2 = per.d2;
            }

            return v;

        }
        




        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.TheGridColumnFilter> filter)
        {
            var gridState = this.Factory.j72TheGridTemplateBL.LoadState(tgi.j72id,Factory.CurrentUser.pid);
            gridState.MasterPID = tgi.master_pid;
            gridState.ContextMenuFlag = tgi.contextmenuflag;
            gridState.OnDblClick = tgi.ondblclick;
            gridState.AddFilterID = tgi.addfilterid;
            gridState.FixedColumns = tgi.fixedcolumns;
            if (string.IsNullOrEmpty(gridState.FixedColumns) == false)
            {
                gridState.j72Columns = gridState.FixedColumns;
            }
            var lis = new List<string>();
            foreach (var c in filter)
            {                
                lis.Add(c.field + "###" + c.oper + "###" + c.value);
                
            }
            gridState.j75CurrentPagerIndex = 0; //po změně filtrovací podmínky je nutné vyčistit paměť stránky
            gridState.j75CurrentRecordPid = 0;

            gridState.j75Filter = string.Join("$$$", lis);
            
            if (this.Factory.j72TheGridTemplateBL.SaveState(gridState,Factory.CurrentUser.pid) > 0)
            {
                return render_thegrid_html(gridState);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se zpracovat filtrovací podmínku.");
            }
        }
        //public TheGridOutput HandleTheGridOper(int j72id,string oper,string key,string value, int master_pid,int contextmenuflag)
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi)
        {
            var gridState = this.Factory.j72TheGridTemplateBL.LoadState(tgi.j72id, Factory.CurrentUser.pid);            
            gridState.MasterPID = tgi.master_pid;
            gridState.ContextMenuFlag = tgi.contextmenuflag;
            gridState.MasterFlag = tgi.master_flag;
            gridState.OnDblClick = tgi.ondblclick;
            gridState.AddFilterID = tgi.addfilterid;
            gridState.FixedColumns = tgi.fixedcolumns;
            if (string.IsNullOrEmpty(gridState.FixedColumns) == false)
            {
                gridState.j72Columns = gridState.FixedColumns;
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
                    break;
            }

            if (this.Factory.j72TheGridTemplateBL.SaveState(gridState,Factory.CurrentUser.pid)> 0)
            {
                return render_thegrid_html(gridState);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se uložit GRIDSTATE");
            }

            
        }
        
        
        public TheGridOutput GetHtml4TheGrid(TheGridUIContext tgi) //Vrací HTML zdroj tabulky pro TheGrid v rámci j72TheGridState
        {
            
            var gridState = this.Factory.j72TheGridTemplateBL.LoadState(tgi.j72id,Factory.CurrentUser.pid);
            if (gridState == null)
            {
                return render_thegrid_error(string.Format("Nelze načíst grid state s id!", tgi.j72id.ToString()));
                
            }
            
            gridState.j75CurrentRecordPid = tgi.go2pid;
            gridState.MasterPID = tgi.master_pid;
            gridState.ContextMenuFlag = tgi.contextmenuflag;
            gridState.OnDblClick = tgi.ondblclick;
            gridState.AddFilterID = tgi.addfilterid;
            gridState.FixedColumns = tgi.fixedcolumns;
            if (string.IsNullOrEmpty(gridState.FixedColumns) == false)
            {
                gridState.j72Columns = gridState.FixedColumns;
            }

            return render_thegrid_html(gridState);
        }
        
        private System.Data.DataTable prepare_datatable(ref BO.myQuery mq, BO.TheGridState gridState)
        {            
            
            mq.explicit_columns = _colsProvider.ParseTheGridColumns(mq.Prefix, gridState.j72Columns,Factory.CurrentUser.j03LangIndex);
            if (string.IsNullOrEmpty(gridState.j75SortDataField)==false)
            {
                
                mq.explicit_orderby = _colsProvider.ByUniqueName(gridState.j75SortDataField).getFinalSqlSyntax_ORDERBY() + " " + gridState.j75SortOrder;
            }          
            if (String.IsNullOrEmpty(gridState.j75Filter) == false)
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, mq.explicit_columns);
            }
            mq.lisPeriods = _pp.getPallete();

            InhaleAddFilter(gridState.AddFilterID, ref mq);
            if (string.IsNullOrEmpty(gridState.j72MasterEntity) && Factory.EProvider.ByPrefix(mq.Prefix).IsGlobalPeriodQuery)
            {
                BO.ThePeriod per = InhaleGridPeriodDates();
                mq.global_d1 = per.d1;
                mq.global_d2 = per.d2;
            }
            if (gridState.j72HashJ73Query)
            {
                mq.lisJ73 = Factory.j72TheGridTemplateBL.GetList_j73(gridState.j72ID,gridState.j72Entity.Substring(0,3));
            }
            mq.InhaleMasterEntityQuery(gridState.j72MasterEntity, gridState.MasterPID,gridState.MasterFlag);

            return Factory.gridBL.GetList(mq);
        }

        private void InhaleAddFilter(string strAddFilterID,ref BO.myQuery mq)
        {
            
            switch (strAddFilterID)
            {
                case "dashboard":
                    BO.ThePeriod per = InhaleGridPeriodDates();
                    mq.global_d1 = per.d1;
                    mq.global_d2 = per.d2;
                    break;
                case "school":
                    mq.a10id = Factory.CBL.LoadUserParamInt("DashboardSchool-a10id");
                    break;
            }
        }
        
        public TheGridOutput render_thegrid_html(BO.TheGridState gridState)
        {
            var ret = new TheGridOutput();
            _grid = new TheGridViewModel() { Entity = gridState.j72Entity };
            _grid.GridState = gridState;

            ret.sortfield = gridState.j75SortDataField;
            ret.sortdir = gridState.j75SortOrder;
            
            var mq = new BO.myQuery(gridState.j72Entity);
            
            
            _grid.Columns =_colsProvider.ParseTheGridColumns(mq.Prefix,gridState.j72Columns, Factory.CurrentUser.j03LangIndex);            


            mq.explicit_columns = _grid.Columns.ToList();
                        
            if (String.IsNullOrEmpty(gridState.j75Filter) == false)
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, mq.explicit_columns);
            }
            mq.lisPeriods = _pp.getPallete();
            InhaleAddFilter(gridState.AddFilterID, ref mq);
            if (string.IsNullOrEmpty(gridState.j72MasterEntity) && Factory.EProvider.ByPrefix(mq.Prefix).IsGlobalPeriodQuery)
            {
                BO.ThePeriod per = InhaleGridPeriodDates();
                mq.global_d1 = per.d1;
                mq.global_d2 = per.d2;
            }
            if (gridState.j72HashJ73Query)
            {
                mq.lisJ73 = Factory.j72TheGridTemplateBL.GetList_j73(gridState.j72ID,gridState.j72Entity.Substring(0,3));
            }
            mq.InhaleMasterEntityQuery(gridState.j72MasterEntity, gridState.MasterPID,gridState.MasterFlag);
                        
            var dtFooter = Factory.gridBL.GetList(mq, true);            
            int intVirtualRowsCount = 0;
            if (dtFooter.Columns.Count > 0)
            {
                intVirtualRowsCount = Convert.ToInt32(dtFooter.Rows[0]["RowsCount"]);
            }
            else
            {
                AddMessage("GRID Error: Dynamic SQL failed.");
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

            var dt = Factory.gridBL.GetList(mq);
            
            

            if (_grid.GridState.j75CurrentRecordPid > 0 && intVirtualRowsCount > gridState.j75PageSize)
            {
                //aby se mohlo skočit na cílový záznam, je třeba najít stránku, na které se záznam nachází
                System.Data.DataRow[] recs = dt.Select("pid=" + _grid.GridState.j75CurrentRecordPid.ToString());
                if (recs.Count() > 0)
                {
                    var intIndex = dt.Rows.IndexOf(recs[0]);
                    _grid.GridState.j75CurrentPagerIndex = intIndex-(intIndex % _grid.GridState.j75PageSize);
                }
            }

            _s = new System.Text.StringBuilder();

            Render_DATAROWS(dt,mq);
            ret.body = _s.ToString();
            _s = new System.Text.StringBuilder();

            Render_TOTALS(dtFooter);
            ret.foot = _s.ToString();
            _s = new System.Text.StringBuilder();

            RENDER_PAGER(intVirtualRowsCount);
            ret.pager = _s.ToString();
            return ret;
        }

        private void Render_DATAROWS(System.Data.DataTable dt,BO.myQuery mq)
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
                if (Convert.ToBoolean(dbRow["isclosed"])==true)
                {
                    strRowClass+= " trbin";
                }
                if (_grid.GridState.OnDblClick == null)
                {
                    _s.Append(string.Format("<tr id='r{0}' class='{1}'>", dbRow["pid"], strRowClass));
                }
                else
                {
                    _s.Append(string.Format("<tr id='r{0}' class='{1}' ondblclick='{2}(this)'>", dbRow["pid"], strRowClass, _grid.GridState.OnDblClick));
                }
                

                
                if (_grid.GridState.j72SelectableFlag>0)
                {
                    _s.Append(string.Format("<td class='td0' style='width:20px;'><input type='checkbox' id='chk{0}'/></td>", dbRow["pid"]));
                }
                else
                {
                    _s.Append("<td class='td0' style='width:20px;'></td>");
                }

                if ((mq.Prefix=="a01" || mq.Prefix=="a11") && dbRow["bgcolor"] != System.DBNull.Value)
                {
                    _s.Append(string.Format("<td class='td1' style='width:20px;background-color:{0}'></td>", dbRow["bgcolor"]));
                }
                else
                {
                    _s.Append("<td class='td1' style='width:20px;'></td>");
                }
                
                
                if (_grid.GridState.ContextMenuFlag > 0)
                {
                    _s.Append(string.Format("<td class='td2' style='width:20px;'><a class='cm' onclick='tg_cm(event)'>&#9776;</a></td>"));      //hamburger menu
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
                    
                    if (i==intStartIndex)   //první řádek musí mít explicitně šířky, aby to z něj zdědili další řádky
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
            _s.Append(string.Format("<th class='th0' title='Celkový počet záznamů' colspan=3 style='width:60px;'><span class='badge badge-primary'>{0}</span></th>", string.Format("{0:#,0}", dt.Rows[0]["RowsCount"])));
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
                _s.Append(string.Format(" style='width:{0}'>{1}</th>",col.ColumnWidthPixels, strVal));


            }
            _s.Append("</tr>");
        }



        


        private void render_select_option(string strValue,string strText,string strSelValue)
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

            _s.Append("<select title='"+Factory.tra("Stránkování záznamů")+"' onchange='tg_pagesize(this)'>");            
            render_select_option("50", "50", intPageSize.ToString());
            render_select_option("100", "100", intPageSize.ToString());
            render_select_option("200", "200", intPageSize.ToString());
            render_select_option("500", "500", intPageSize.ToString());
            render_select_option("1000", "1000", intPageSize.ToString());            
            _s.Append("</select>");
            if (intRowsCount < 0)
            {
                RenderPanelsSwitchFlag();
                return;
            }
            
            if (intRowsCount <= intPageSize)
            {
                RenderPanelsSwitchFlag();
                return;
            }

            _s.Append("<button title='"+Factory.tra("První")+"' class='btn btn-light tgp' style='margin-left:6px;' onclick='tg_pager(\n0\n)'>&lt;&lt;</button>");

            int intCurIndex = _grid.GridState.j75CurrentPagerIndex;
            int intPrevIndex = intCurIndex - intPageSize;
            if (intPrevIndex < 0) intPrevIndex = 0;
            _s.Append(string.Format("<button title='"+Factory.tra("Předchozí")+"' class='btn btn-light tgp' style='margin-right:10px;' onclick='tg_pager(\n{0}\n)'>&lt;</button>", intPrevIndex));

            if (intCurIndex >= intPageSize * 10)
            {
                intPrevIndex = intCurIndex - 10 * intPageSize;
                _s.Append(string.Format("<button class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>...</button>", intPrevIndex));
            }
            

            int intStartIndex = 0;
            for (int i = 0; i <= intRowsCount; i += intPageSize*10)
            {
                if (intCurIndex>=i && intCurIndex<i+intPageSize*10)
                {
                    intStartIndex = i;
                    break;
                }
            }
                           
            int intEndIndex = intStartIndex+(9 * intPageSize);
            if (intEndIndex+1 > intRowsCount) intEndIndex = intRowsCount-1;

            
            int intPageNum = intStartIndex/intPageSize; string strClass;
            for (var i = intStartIndex; i <= intEndIndex; i+=intPageSize)
            {
                intPageNum += 1;
                if (intCurIndex>=i && intCurIndex < i+ intPageSize)
                {
                    strClass = "btn btn-secondary tgp";
                }
                else
                {
                    strClass = "btn btn-light tgp";
                }
                _s.Append(string.Format("<button type='button' class='{0}' onclick='tg_pager(\n{1}\n)'>{2}</button>",strClass, i,intPageNum));
               
            }
            if (intEndIndex+1 < intRowsCount)
            {
                intEndIndex += intPageSize;
                if (intEndIndex + 1 > intRowsCount) intEndIndex = intRowsCount - intPageSize;
                _s.Append(string.Format("<button type='button' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>...</button>", intEndIndex));
            }

            int intNextIndex = intCurIndex + intPageSize;
            if (intNextIndex + 1>intRowsCount) intNextIndex = intRowsCount-intPageSize;
            _s.Append(string.Format("<button type='button' title='"+Factory.tra("Další")+"' class='btn btn-light tgp' style='margin-left:10px;' onclick='tg_pager(\n{0}\n)'>&gt;</button>", intNextIndex));

            int intLastIndex = intRowsCount - (intRowsCount % intPageSize);  //% je zbytek po celočíselném dělení
            _s.Append(string.Format("<button type='button' title='"+Factory.tra("Poslední")+"' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>&gt;&gt;</button>", intLastIndex));

            RenderPanelsSwitchFlag();
        }

        private void RenderPanelsSwitchFlag()
        {
            if (_grid.GridState.MasterViewFlag<3)
            {
                switch (_grid.Entity.Substring(0, 3))
                {
                    case "a01":
                    case "a03":
                    case "j02":
                        if (_grid.GridState.MasterViewFlag==2)
                        {
                            _s.Append("<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='tg_switchflag(\""+_grid.Entity.Substring(0, 3)+"\",0)'>"+Factory.tra("Vypnout spodní panel")+"</button>");
                        }
                        else
                        {
                            _s.Append("<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='tg_switchflag(\"" + _grid.Entity.Substring(0, 3) + "\",1)'>"+Factory.tra("Zapnout spodní panel")+"</button>");
                        }
                        break;
                }
            }            
        }


        private TheGridOutput render_thegrid_error(string strError)
        {
            var ret = new TheGridOutput();
            ret.message = strError;
            if (this.Factory.CurrentUser.Messages4Notify.Count > 0)
            {
                ret.message += " | " + string.Join(",", this.Factory.CurrentUser.Messages4Notify.Select(p => p.Value));
            }
            return ret;
        }

       

        public string getHTML_ContextMenu(int j72id)
        {
            var sb = new System.Text.StringBuilder();
            var recJ72 = Factory.j72TheGridTemplateBL.LoadState(j72id,Factory.CurrentUser.pid);
                   
            sb.AppendLine("<div style='background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>"+Factory.tra("VYBRANÉ (zaškrtlé) záznamy")+"</div>");
            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\",\"selected\")'>" + Factory.tra("MS-EXCEL Export")+"</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\",\"selected\")'>"+Factory.tra("CSV Export")+"</a>", j72id));
            sb.AppendLine("</div>");

            if ("j02,a01,a03,f06".Contains(recJ72.j72Entity.Substring(0, 3)))
            {
                sb.AppendLine("<hr class='hr-mini' />");
                sb.AppendLine("<a class='nav-link' href='javascript:tg_tagging();'>" + Factory.tra("Hromadná kategorizace záznamů")+"★</a>");

            }


            sb.AppendLine(string.Format("<div style='margin-top:20px;background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>GRID <kbd>{0}</kbd></div>", Factory.EProvider.ByTable(recJ72.j72Entity).AliasPlural));

            var lis = Factory.j72TheGridTemplateBL.GetList(recJ72.j72Entity, recJ72.j03ID, recJ72.j72MasterEntity);
            sb.AppendLine("<table style='width:100%;margin-bottom:20px;'>");
            foreach (var c in lis)
            {
                sb.AppendLine("<tr>");
                if (c.j72IsSystem)
                {
                    c.j72Name = Factory.tra("Výchozí GRID");
                }
                if (c.pid == recJ72.pid)
                {
                    c.j72Name += " ✔";
                }
                sb.Append(string.Format("<td><a class='nav-link py-0' href='javascript:change_grid({0})'>{1}</a></td>", c.pid, c.j72Name));

                sb.AppendLine(string.Format("<td style='width:30px;'><a title='Grid Návrhář' class='nav-link py-0' href='javascript:_window_open(\"/TheGridDesigner/Index?j72id={0}\",2);'><img src='/Images/setting.png'/></a></td>", c.pid));
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            

            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\")'>" + Factory.tra("MS-EXCEL Export (vše)")+"</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\")'>" + Factory.tra("CSV Export (vše)")+"</a>", j72id));
            sb.AppendLine("</div>");



            sb.AppendLine("<hr class='hr-mini' />");
            sb.AppendLine("<a  href='javascript:tg_select(20)'>" + Factory.tra(string.Format("Vybrat prvních {0}",20))+"</a>⌾");
            sb.AppendLine("<a  href='javascript:tg_select(50)'>" + Factory.tra(string.Format("Vybrat prvních {0}", 50)) +"</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(100)'>" + Factory.tra(string.Format("Vybrat prvních {0}", 100)) +"</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(1000)'>" + Factory.tra("Vybrat všechny záznamy na stránce")+"</a>");

            return sb.ToString();
        }

        public FileResult GridExport(string format,int j72id,int master_pid,string master_entity,string pids,string master_flag)
        {
            var gridState = this.Factory.j72TheGridTemplateBL.LoadState(j72id,Factory.CurrentUser.pid);
            gridState.j72MasterEntity = master_entity;
            gridState.MasterPID = master_pid;
            gridState.MasterFlag = master_flag;
            var mq = new BO.myQuery(gridState.j72Entity);
            
            if (String.IsNullOrEmpty(pids) == false)
            {
                mq.SetPids(pids);
            }
            
           
            System.Data.DataTable dt = prepare_datatable(ref mq,gridState);
            string filepath = Factory.App.TempFolder+"\\"+BO.BAS.GetGuid()+"."+ format;

            var cExport = new UI.dataExport();
            string strFileClientName = "gridexport_" + mq.Prefix + "." + format;

            if (format == "csv")
            {
                if (cExport.ToCSV(dt, filepath, mq))
                {                    
                    return File(System.IO.File.ReadAllBytes(filepath), "application/CSV", strFileClientName);
                }
            }
            if (format == "xlsx")
            {
                if (cExport.ToXLSX(dt, filepath, mq))
                {                    
                    return File(System.IO.File.ReadAllBytes(filepath), "application/vnd.ms-excel", strFileClientName);
                }
            }


            return null;

        }


        private BO.ThePeriod InhaleGridPeriodDates()
        {
            var ret = _pp.ByPid(0);
            var x = Factory.CBL.LoadUserParamInt("grid-period-value");
            if (x > 0)
            {
                ret = _pp.ByPid(x);
            }
            else
            {
                ret.d1 = Factory.CBL.LoadUserParamDate("grid-period-d1");
                ret.d2 = Factory.CBL.LoadUserParamDate("grid-period-d2");

            }
            return ret;
        }



    }
}