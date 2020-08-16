using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers;
using UI.Models;

namespace UI.Views.Shared.Components.TheGrid
{
    public class TheGridViewComponent: ViewComponent
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
            Invoke(string entity,int j72id,int go2pid,string master_entity,int master_pid,int contextmenuflag,string ondblclick,string master_flag, int masterviewflag)
        {
            var ret = new TheGridViewModel();
            ret.Entity = entity;
            var mq = new BO.myQuery(entity);
           

            BO.j72TheGridTemplate cJ72 = null;
            if (j72id > 0)
            {
                cJ72 = _f.gridBL.LoadTheGridState(j72id);
            }
            if (cJ72 == null)
            {
                cJ72 = _f.gridBL.LoadTheGridState(entity, _f.CurrentUser.pid, master_entity);  //výchozí, systémový grid: j72IsSystem=1
            }

            if (cJ72 == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat
            {
                var cols= _colsProvider.getDefaultPallete(false,mq);    //výchozí paleta sloupců
                
                cJ72 = new BO.j72TheGridTemplate() {j72IsSystem=true, j72Entity = entity, j03ID = _f.CurrentUser.pid,j72Columns=String.Join(",",cols.Select(p=>p.UniqueName)),j72PageSize=100,j72MasterEntity= master_entity };
                
                var intJ72ID = _f.gridBL.SaveTheGridState(cJ72,null,null,null);
                cJ72= _f.gridBL.LoadTheGridState(intJ72ID);
            }
            cJ72.MasterViewFlag = masterviewflag;
            cJ72.j72MasterFlag = master_flag;
            cJ72.j72CurrentRecordPid = go2pid;
            cJ72.j72ContextMenuFlag = contextmenuflag;
            cJ72.j72MasterEntity = master_entity;
            cJ72.j72MasterPID = master_pid;
            cJ72.OnDblClick = ondblclick;
                        
            var cc = new TheGridController(_colsProvider,_pp);
            cc.Factory = _f;

            ret.firstdata = cc.render_thegrid_html(cJ72);
            ret.ondblclick = ondblclick;
            ret.GridState = cJ72;
            ret.Columns = _colsProvider.ParseTheGridColumns(mq.Prefix, cJ72.j72Columns,_f.CurrentUser.j03LangIndex);
            ret.AdhocFilter = _colsProvider.ParseAdhocFilterFromString(cJ72.j72Filter, ret.Columns);
            ret.MasterEntity = master_entity;
            ret.MasterPID = master_pid;
            ret.MasterFlag = master_flag;
            return View("Default", ret);

            


        }
    }
}
