﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;




using UI.Models;
using System.ComponentModel;

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
                    tabs.Add(new NavTab() { Name = "Info",Entity="viewInfo", Url = "/a01/Info?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Historie událostí",Entity="viewHistory", Url = "/a01/TabHistory?pid=" + AppendPid2Url(v.go2pid) });
                    
                    tabs.Add(new NavTab() { Name="Účastníci akce",Entity = "viewUcastnici", Url = "/a01/TabUcastnici?pid=" + AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Časový plán", Entity = "viewCapacity", Url = "/a35/TabCapacity?pid=" + AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Formuláře", Entity = "viewFormulare", Url = "/a01/TabForms?pid=" + AppendPid2Url(v.go2pid) });

                    //tabs.Add(new NavTab() { Name = "Přílohy", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(new NavTab() { Name = "Úkoly/Lhůty", Entity = "h04ToDo", Url = "SlaveView?prefix=h04" });
                    tabs.Add(new NavTab() { Name = "Související akce", Entity = "a01Event", Url = "SlaveView?prefix=a01" });
                    
                    break;
                case "a03":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/a03/Info?pid=" + AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Svázané akce", Entity = "a01Event", Url = "SlaveView?prefix=a01" });
                    tabs.Add(new NavTab() { Name = "Činnosti školy", Entity = "a37InstitutionDepartment", Url = "SlaveView?prefix=a37" });
                    tabs.Add(new NavTab() { Name = "Vzdělávací obory", Entity = "a19DomainToInstitutionDepartment", Url = "SlaveView?prefix=a19" });
                    tabs.Add(new NavTab() { Name = "Kontaktní osoby", Entity = "a39InstitutionPerson", Url = "SlaveView?prefix=a39" });
                    tabs.Add(new NavTab() { Name = "Učitelé", Entity = "k01Teacher", Url = "SlaveView?prefix=k01" });
                    tabs.Add(new NavTab() { Name = "INEZ", Entity = "a42Qes", Url = "SlaveView?prefix=a42" });
                    tabs.Add(new NavTab() { Name = "Pojmenované seznamy", Entity = "a29InstitutionList", Url = "SlaveView?prefix=a29" });

                    break;
                case "a42":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/a42/Info?pid=" + AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Svázané akce", Entity = "a01Event", Url = "SlaveView?prefix=a01" });
                    tabs.Add(new NavTab() { Name = "Outbox", Entity = "x40MailQueue", Url = "SlaveView?prefix=x40" });
                    break;
                case "j02":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/j02/Info?pid=" + AppendPid2Url(v.go2pid) });                    
                    tabs.Add(new NavTab() { Name = string.Format("{0}: je zakladatelem",Factory.App.Terminology_Akce), Entity = "a01Event", Url = "SlaveView?prefix=a01" });
                    tabs.Add(new NavTab() { Name = string.Format("{0}: je účastníkem", Factory.App.Terminology_Akce), Entity = "a01Event", Url = "SlaveView?prefix=a01" });
                    tabs.Add(new NavTab() { Name = "Instituce", Entity = "a03Institution", Url = "SlaveView?prefix=a03" });
                    tabs.Add(new NavTab() { Name = "Úkoly/Lhůty", Entity = "h04ToDo", Url = "SlaveView?prefix=h04" });
                    tabs.Add(new NavTab() { Name = "Outbox", Entity = "x40MailQueue", Url = "SlaveView?prefix=x40" });
                    tabs.Add(new NavTab() { Name = "PING Log", Entity = "j92PingLog", Url = "SlaveView?prefix=j92" });
                    tabs.Add(new NavTab() { Name = "LOGIN Log", Entity = "j90LoginAccessLog", Url = "SlaveView?prefix=j90" });
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
                Factory.CurrentUser.AddMessage("Musíte vybrat záznam z nadřízeného panelu.");
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
                Factory.CurrentUser.AddMessage("Entity for Grid not found.");
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
            var cJ72 = this.Factory.gridBL.LoadTheGridState(tgi.j72id);
            cJ72.j72MasterPID = tgi.master_pid;
            cJ72.j72ContextMenuFlag = tgi.contextmenuflag;
            cJ72.OnDblClick = tgi.ondblclick;
            var lis = new List<string>();
            foreach (var c in filter)
            {                
                lis.Add(c.field + "###" + c.oper + "###" + c.value);
                
            }
            cJ72.j72CurrentPagerIndex = 0; //po změně filtrovací podmínky je nutné vyčistit paměť stránky
            cJ72.j72CurrentRecordPid = 0;
            
            cJ72.j72Filter = string.Join("$$$", lis);
            
            if (this.Factory.gridBL.SaveTheGridState(cJ72,null,null,null) > 0)
            {
                return render_thegrid_html(cJ72);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se zpracovat filtrovací podmínku.");
            }
        }
        //public TheGridOutput HandleTheGridOper(int j72id,string oper,string key,string value, int master_pid,int contextmenuflag)
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(tgi.j72id);
            cJ72.j72MasterPID = tgi.master_pid;
            cJ72.j72ContextMenuFlag = tgi.contextmenuflag;
            cJ72.j72MasterFlag = tgi.master_flag;
            cJ72.OnDblClick = tgi.ondblclick;
            switch (tgi.key)
            {
                
                case "pagerindex":
                    cJ72.j72CurrentPagerIndex = BO.BAS.InInt(tgi.value);
                    break;
                case "pagesize":
                    cJ72.j72PageSize = BO.BAS.InInt(tgi.value);
                    break;
                case "sortfield":
                    if (cJ72.j72SortDataField != tgi.value)
                    {
                        cJ72.j72SortOrder = "asc";
                        cJ72.j72SortDataField = tgi.value;
                    }
                    else
                    {
                        if (cJ72.j72SortOrder == "desc")
                        {
                            cJ72.j72SortDataField = "";//vyčisitt třídění, třetí stav
                            cJ72.j72SortOrder = "";
                        }
                        else
                        {
                            if (cJ72.j72SortOrder == "asc")
                            {                                
                                cJ72.j72SortOrder = "desc";
                            }
                        }
                    }
                    
                    
                    break;
                case "filter":
                    break;
            }

            if (this.Factory.gridBL.SaveTheGridState(cJ72,null,null,null)> 0)
            {
                return render_thegrid_html(cJ72);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se uložit GRIDSTATE");
            }

            
        }
        
        
        public TheGridOutput GetHtml4TheGrid(TheGridUIContext tgi) //Vrací HTML zdroj tabulky pro TheGrid v rámci j72TheGridState
        {
            
            var cJ72 = this.Factory.gridBL.LoadTheGridState(tgi.j72id);
            if (cJ72 == null)
            {
                return render_thegrid_error(string.Format("Nelze načíst grid state s id!", tgi.j72id.ToString()));
                
            }            
            cJ72.j72CurrentRecordPid = tgi.go2pid;
            cJ72.j72MasterPID = tgi.master_pid;
            cJ72.j72ContextMenuFlag = tgi.contextmenuflag;
            cJ72.OnDblClick = tgi.ondblclick;


            return render_thegrid_html(cJ72);
        }
        
        private System.Data.DataTable prepare_datatable(ref BO.myQuery mq, BO.j72TheGridState cJ72)
        {            
            
            mq.explicit_columns = _colsProvider.ParseTheGridColumns(mq.Prefix,cJ72.j72Columns,Factory.CurrentUser.j03LangIndex);
            if (string.IsNullOrEmpty(cJ72.j72SortDataField)==false)
            {
                
                mq.explicit_orderby = _colsProvider.ByUniqueName(cJ72.j72SortDataField).getFinalSqlSyntax_ORDERBY() + " " + cJ72.j72SortOrder;
            }          
            if (String.IsNullOrEmpty(cJ72.j72Filter) == false)
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(cJ72.j72Filter, mq.explicit_columns);
            }
            mq.lisPeriods = _pp.getPallete();

            if (string.IsNullOrEmpty(cJ72.j72MasterEntity) && Factory.EProvider.ByPrefix(mq.Prefix).IsGlobalPeriodQuery)
            {
                BO.ThePeriod per = InhaleGridPeriodDates();
                mq.global_d1 = per.d1;
                mq.global_d2 = per.d2;
            }
            if (cJ72.j72HashJ73Query)
            {
                mq.lisJ73 = Factory.gridBL.GetList_j73(cJ72);
            }
            mq.InhaleMasterEntityQuery(cJ72.j72MasterEntity, cJ72.j72MasterPID,cJ72.j72MasterFlag);

            return Factory.gridBL.GetList(mq);
        }
        
        public TheGridOutput render_thegrid_html(BO.j72TheGridState cJ72)
        {
            var ret = new TheGridOutput();
            _grid = new TheGridViewModel() { Entity = cJ72.j72Entity };
            _grid.GridState = cJ72;

            ret.sortfield = cJ72.j72SortDataField;
            ret.sortdir = cJ72.j72SortOrder;
            
            var mq = new BO.myQuery(cJ72.j72Entity);
            
            
            _grid.Columns =_colsProvider.ParseTheGridColumns(mq.Prefix,cJ72.j72Columns, Factory.CurrentUser.j03LangIndex);            

            mq.explicit_columns = _grid.Columns;
                        
            if (String.IsNullOrEmpty(cJ72.j72Filter) == false)
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(cJ72.j72Filter, mq.explicit_columns);
            }
            mq.lisPeriods = _pp.getPallete();
            if (string.IsNullOrEmpty(cJ72.j72MasterEntity) && Factory.EProvider.ByPrefix(mq.Prefix).IsGlobalPeriodQuery)
            {
                BO.ThePeriod per = InhaleGridPeriodDates();
                mq.global_d1 = per.d1;
                mq.global_d2 = per.d2;
            }
            if (cJ72.j72HashJ73Query)
            {
                mq.lisJ73 = Factory.gridBL.GetList_j73(cJ72);
            }
            mq.InhaleMasterEntityQuery(cJ72.j72MasterEntity, cJ72.j72MasterPID,cJ72.j72MasterFlag);
                        
            var dtFooter = Factory.gridBL.GetList(mq, true);            
            int intVirtualRowsCount = 0;
            if (dtFooter.Columns.Count > 0)
            {
                intVirtualRowsCount = Convert.ToInt32(dtFooter.Rows[0]["RowsCount"]);
            }
            else
            {
                this.AddMessage("GRID Error: Dynamic SQL failed.");
            }

            if (intVirtualRowsCount > 500)
            {   //dotazy nad 500 záznamů budou mít zapnutý OFFSET režim stránkování
                mq.OFFSET_PageSize = cJ72.j72PageSize;
                mq.OFFSET_PageNum = cJ72.j72CurrentPagerIndex / cJ72.j72PageSize;
            }

            //třídění řešit až po spuštění FOOTER summary DOTAZu
            if (String.IsNullOrEmpty(cJ72.j72SortDataField) == false && _grid.Columns.Where(p => p.UniqueName == cJ72.j72SortDataField).Count() > 0)
            {
                var c = _grid.Columns.Where(p => p.UniqueName == cJ72.j72SortDataField).First();
                mq.explicit_orderby = c.getFinalSqlSyntax_ORDERBY() + " " + cJ72.j72SortOrder;
            }

            var dt = Factory.gridBL.GetList(mq);
            
            

            if (_grid.GridState.j72CurrentRecordPid > 0 && intVirtualRowsCount > cJ72.j72PageSize)
            {
                //aby se mohlo skočit na cílový záznam, je třeba najít stránku, na které se záznam nachází
                System.Data.DataRow[] recs = dt.Select("pid=" + _grid.GridState.j72CurrentRecordPid.ToString());
                if (recs.Count() > 0)
                {
                    var intIndex = dt.Rows.IndexOf(recs[0]);
                    _grid.GridState.j72CurrentPagerIndex = intIndex-(intIndex % _grid.GridState.j72PageSize);
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
                intStartIndex = _grid.GridState.j72CurrentPagerIndex;
                intEndIndex = intStartIndex + _grid.GridState.j72PageSize - 1;
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

                if (mq.Prefix=="a01" && dbRow["bgcolor"] != System.DBNull.Value)
                {
                    _s.Append(string.Format("<td class='td1' style='width:20px;background-color:{0}'></td>", dbRow["bgcolor"]));
                }
                else
                {
                    _s.Append("<td class='td1' style='width:20px;'></td>");
                }
                
                
                if (_grid.GridState.j72ContextMenuFlag > 0)
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
            int intPageSize = _grid.GridState.j72PageSize;

            _s.Append("<select title='Stránkování záznamů' onchange='tg_pagesize(this)'>");            
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

            _s.Append("<button title='První' class='btn btn-light tgp' style='margin-left:6px;' onclick='tg_pager(\n0\n)'>&lt;&lt;</button>");

            int intCurIndex = _grid.GridState.j72CurrentPagerIndex;
            int intPrevIndex = intCurIndex - intPageSize;
            if (intPrevIndex < 0) intPrevIndex = 0;
            _s.Append(string.Format("<button title='Předchozí' class='btn btn-light tgp' style='margin-right:10px;' onclick='tg_pager(\n{0}\n)'>&lt;</button>", intPrevIndex));

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
            _s.Append(string.Format("<button type='button' title='Další' class='btn btn-light tgp' style='margin-left:10px;' onclick='tg_pager(\n{0}\n)'>&gt;</button>", intNextIndex));

            int intLastIndex = intRowsCount - (intRowsCount % intPageSize);  //% je zbytek po celočíselném dělení
            _s.Append(string.Format("<button type='button' title='Poslední' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>&gt;&gt;</button>", intLastIndex));

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
                            _s.Append("<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='tg_switchflag(\""+_grid.Entity.Substring(0, 3)+"\",0)'>Vypnout spodní panel</button>");
                        }
                        else
                        {
                            _s.Append("<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='tg_switchflag(\"" + _grid.Entity.Substring(0, 3) + "\",1)'>Zapnout spodní panel</button>");
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
            BO.j72TheGridState c = Factory.gridBL.LoadTheGridState(j72id);
                   
            sb.AppendLine("<div style='background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>VYBRANÉ (zaškrtlé) záznamy</div>");
            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\",\"selected\")'>MS-EXCEL Export</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\",\"selected\")'>CSV Export</a>", j72id));
            sb.AppendLine("</div>");

            if ("j02,p51,p41,p10,p11,p12,p13,p18,p19,p26,p28,p21,o23".Contains(c.j72Entity.Substring(0, 3)))
            {
                sb.AppendLine("<hr class='hr-mini' />");
                sb.AppendLine("<a class='nav-link' href='javascript:tg_tagging();'>Hromadná kategorizace záznamů★</a>");

            }


            //sb.AppendLine("<hr />");

            sb.AppendLine(string.Format("<div style='margin-top:20px;background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>GRID <kbd>{0}</kbd></div>", Factory.EProvider.ByTable(c.j72Entity).AliasPlural));

            var lis = Factory.gridBL.GetList_j72(c.j72Entity, c.j03ID, c.j72MasterEntity);
            sb.AppendLine("<table style='width:100%;margin-bottom:20px;'>");
            foreach (var rec in lis)
            {
                sb.AppendLine("<tr>");
                if (rec.j72IsSystem)
                {
                    rec.j72Name = "Výchozí GRID";
                }
                if (rec.pid == c.pid)
                {
                    rec.j72Name += " ✔";
                }
                sb.Append(string.Format("<td><a class='nav-link py-0' href='javascript:change_grid({0})'>{1}</a></td>", rec.pid, rec.j72Name));

                sb.AppendLine(string.Format("<td style='width:30px;'><a title='Grid Návrhář' class='nav-link py-0' href='javascript:_window_open(\"/TheGridDesigner/Index?j72id={0}\",2);'><img src='/Images/setting.png'/></a></td>", rec.pid));
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            

            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\")'>MS-EXCEL Export (vše)</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\")'>CSV Export (vše)</a>", j72id));
            sb.AppendLine("</div>");



            sb.AppendLine("<hr class='hr-mini' />");
            sb.AppendLine("<a  href='javascript:tg_select(20)'>Vybrat prvních 20</a>⌾");
            sb.AppendLine("<a  href='javascript:tg_select(50)'>Vybrat prvních 50</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(100)'>Vybrat prvních 100</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(1000)'>Vybrat všechny záznamy na stránce</a>");

            return sb.ToString();
        }

        public FileResult GridExport(string format,int j72id,int master_pid,string master_entity,string pids,string master_flag)
        {
            BO.j72TheGridState cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            cJ72.j72MasterEntity = master_entity;
            cJ72.j72MasterPID = master_pid;
            cJ72.j72MasterFlag = master_flag;
            var mq = new BO.myQuery(cJ72.j72Entity);
            
            if (String.IsNullOrEmpty(pids) == false)
            {
                mq.SetPids(pids);
            }
            
           
            System.Data.DataTable dt = prepare_datatable(ref mq,cJ72);
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