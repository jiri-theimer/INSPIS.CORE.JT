using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers;
using UI.Models;

namespace UI.Views.Shared.Components.TheGrid
{
    public class TheGridViewComponent : ViewComponent
    {
        BL.Factory _f;
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;
        public TheGridViewComponent(BL.Factory f, BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _f = f;
            _colsProvider = cp;
            _pp = pp;
        }

        public IViewComponentResult
            Invoke(string entity, int j72id, int go2pid, string master_entity, int master_pid, int contextmenuflag, string ondblclick, string master_flag, int masterviewflag,string addfilterid,string fixedcolumns)
        {
            var ret = new TheGridViewModel();
            ret.Entity = entity;
            var mq = new BO.myQuery(entity);


            BO.TheGridState gridState = null;
            if (j72id > 0)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(j72id, _f.CurrentUser.pid);
            }
            if (gridState == null)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(entity, _f.CurrentUser.pid, master_entity);  //výchozí, systémový grid: j72IsSystem=1
            }

            if (gridState == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat
            {
                var cols = _colsProvider.getDefaultPallete(false, mq);    //výchozí paleta sloupců

                var recJ72 = new BO.j72TheGridTemplate() { j72IsSystem = true, j72Entity = entity, j03ID = _f.CurrentUser.pid, j72Columns = String.Join(",", cols.Select(p => p.UniqueName)), j72MasterEntity = master_entity };

                var intJ72ID = _f.j72TheGridTemplateBL.Save(recJ72, null, null, null);
                gridState = _f.j72TheGridTemplateBL.LoadState(intJ72ID, _f.CurrentUser.pid);
            }
            if (string.IsNullOrEmpty(fixedcolumns)==false)
            {
                gridState.j72Columns = fixedcolumns;
            }
            gridState.MasterViewFlag = masterviewflag;
            gridState.MasterFlag = master_flag;
            gridState.j75CurrentRecordPid = go2pid;
            gridState.ContextMenuFlag = contextmenuflag;
            gridState.j72MasterEntity = master_entity;
            gridState.MasterPID = master_pid;
            gridState.OnDblClick = ondblclick;
            gridState.AddFilterID = addfilterid;
            gridState.FixedColumns = fixedcolumns;

            var cc = new TheGridController(_colsProvider, _pp);
            cc.Factory = _f;
            
            ret.firstdata = cc.render_thegrid_html(gridState);
            ret.ondblclick = ondblclick;
            ret.GridState = gridState;
            ret.Columns = _colsProvider.ParseTheGridColumns(mq.Prefix, gridState.j72Columns, _f.CurrentUser.j03LangIndex);
            ret.AdhocFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, ret.Columns);
            ret.MasterEntity = master_entity;
            ret.MasterPID = master_pid;
            ret.MasterFlag = master_flag;            
            return View("Default", ret);




        }
    }
}
