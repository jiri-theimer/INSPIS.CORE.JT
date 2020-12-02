using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Dashboard;

namespace UI
{
    public class WidgetSupport
    {
        private BL.Factory _f;  //podpora k úvodní HOME stránce s obsluhou widgets
        private string _skin;   //hodnoty: widgets/index
        public WidgetSupport(BL.Factory f,string skin)
        {
            _f = f;
            _skin = skin;
        }

        public BO.Result SavePocetSloupcu(int x)
        {
            _f.CBL.SetUserParam("Widgets-ColumnsPerPage-"+_skin, x.ToString());            
            return new BO.Result(false);
        }
        public BO.Result SaveWidgetState(string s)
        {
            var rec = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid,_skin);
            rec.x56DockState = s;
            rec.x56Skin = _skin;
            _f.x55WidgetBL.SaveState(rec);
            return new BO.Result(false);
        }
        public BO.Result RemoveWidget(int x55id)
        {
            var recX55 = _f.x55WidgetBL.Load(x55id);
            var recX56 = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid,_skin);
            var boxes = BO.BAS.ConvertString2List(recX56.x56Boxes);
            if (boxes.Where(p => p == recX55.x55Code).Count() > 0)
            {
                boxes.Remove(recX55.x55Code);
                recX56.x56Boxes = string.Join(",", boxes);
                _f.x55WidgetBL.SaveState(recX56);
                return new BO.Result(false);
            }

            return new BO.Result(true, "widget not found");
        }
        public BO.Result InsertWidget(int x55id)
        {
            var recX55 = _f.x55WidgetBL.Load(x55id);
            var recX56 = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid,_skin);
            var boxes = BO.BAS.ConvertString2List(recX56.x56Boxes);
            if (boxes.Where(p => p == recX55.x55Code).Count() == 0)
            {
                boxes.Add(recX55.x55Code);
                recX56.x56Boxes = string.Join(",", boxes);
                _f.x55WidgetBL.SaveState(recX56);
                return new BO.Result(false);
            }
            return new BO.Result(true, "widget not found");
        }

        public void InhaleWidgetsDataContent(WidgetsViewModel v)
        {
            foreach(var rec in v.lisUserWidgets)
            {
                if (rec.x55TableSql != null && rec.x55TableColHeaders !=null)
                {
                    string s = rec.x55TableSql;
                    s = DL.BAS.ParseMergeSQL(s, _f.CurrentUser.j02ID.ToString()).Replace("@j04id",_f.CurrentUser.j04ID.ToString().Replace("@j03id",_f.CurrentUser.pid.ToString()));                    
                    var dt = _f.gridBL.GetListFromPureSql(s);
                    var cGen = new BO.CLS.Datatable2Html(new BO.CLS.Datatable2HtmlDef() { ColHeaders = rec.x55TableColHeaders, ColTypes = rec.x55TableColTypes });
                    rec.x55Content = cGen.CreateHtmlTable(dt,500);
                }
                else
                {
                    switch (rec.x55Code.ToLower())
                    {
                        case "pandulak":                           
                            var pandulak = new ThePandulak(_f.App.AppRootFolder+"\\wwwroot\\images\\pandulak");
                            rec.x55Content = string.Format("<img src='/images/pandulak/{0}'/>", pandulak.getPandulakImage(1));
                            if (v.ColumnsPerPage <= 2)
                            {
                                rec.x55Content += string.Format("<img src='/images/pandulak/{0}'/>", pandulak.getPandulakImage(2));
                            }                            
                            break;
                    }
                }
                
            }
        }
        public void PrepareWidgets(WidgetsViewModel v)
        {
            v.lisAllWidgets = _f.x55WidgetBL.GetList(new BO.myQuery("x55") { IsRecordValid = true, MyRecordsDisponible = true, CurrentUser = _f.CurrentUser });
            v.lisUserWidgets = new List<BO.x55Widget>();
            v.ColumnsPerPage = _f.CBL.LoadUserParamInt("Widgets-ColumnsPerPage-"+_skin, 2);
            v.recX56 = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid,_skin);
            v.DockStructure = new WidgetsEnvironment(v.recX56.x56DockState);

            if (v.recX56==null || v.recX56.x56Boxes == null)
            {
                return; //uživatel nemá na ploše žádný widget, dál není třeba pokračovat
            }                        
            
            var boxes = BO.BAS.ConvertString2List(v.recX56.x56Boxes);
            foreach (string s in boxes)
            {
                if (v.lisAllWidgets.Where(p => p.x55Code == s).Count() > 0)
                {
                    v.lisUserWidgets.Add(v.lisAllWidgets.Where(p => p.x55Code == s).First());
                }
            }
            

            foreach (var onestate in v.DockStructure.States)
            {
                if (v.lisUserWidgets.Where(p => p.pid.ToString() == onestate.Value).Count() > 0)
                {
                    var c = v.lisUserWidgets.Where(p => p.pid.ToString() == onestate.Value).First();
                    switch (onestate.Key)
                    {
                        case "2":
                            if (v.ColumnsPerPage >= 2) v.DockStructure.Col2.Add(c);
                            break;
                        case "3":
                            if (v.ColumnsPerPage >= 3) v.DockStructure.Col3.Add(c);
                            break;
                        default:
                            v.DockStructure.Col1.Add(c);
                            break;
                    }
                }
            }
            foreach (var c in v.lisUserWidgets)
            {
                if ((v.DockStructure.Col1.Contains(c) || v.DockStructure.Col2.Contains(c) || v.DockStructure.Col3.Contains(c)) == false)
                {
                    switch (v.ColumnsPerPage)
                    {
                        case 2 when (v.DockStructure.Col1.Count() >= 2):
                            v.DockStructure.Col2.Add(c);
                            break;
                        case 3 when (v.DockStructure.Col1.Count() >= 2 && v.DockStructure.Col2.Count() >= 2):
                            v.DockStructure.Col3.Add(c);
                            break;
                        case 3 when (v.DockStructure.Col1.Count() >= 2 && v.DockStructure.Col2.Count() < 2):
                            v.DockStructure.Col2.Add(c);
                            break;
                        default:
                            v.DockStructure.Col1.Add(c);
                            break;
                    }

                }
            }
            switch (v.ColumnsPerPage)
            {
                case 1:
                    v.BoxColCss = "col-12";
                    break;
                case 2:
                    v.BoxColCss = "col-lg-6";
                    break;
                case 3:
                    v.BoxColCss = "col-sm-6 col-lg-4";
                    break;
            }
        }
    }
}
