using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI
{
    public class WidgetSupport
    {
        private BL.Factory _f;  //podpora k úvodní HOME stránce s obsluhou widgets

        public WidgetSupport(BL.Factory f)
        {
            _f = f;
        }

        public BO.Result SavePocetSloupcu(int x)
        {
            _f.CBL.SetUserParam("Widgets-ColumnsPerPage", x.ToString());            
            return new BO.Result(false);
        }
        public BO.Result SaveWidgetState(string s)
        {
            var rec = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid);
            rec.x56DockState = s;
            _f.x55WidgetBL.SaveState(rec);
            return new BO.Result(false);
        }
        public BO.Result RemoveWidget(int x55id)
        {
            var recX55 = _f.x55WidgetBL.Load(x55id);
            var recX56 = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid);
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
            var recX56 = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid);
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

        public void PrepareWidgets(HomeViewModel v)
        {
            v.ColumnsPerPage = _f.CBL.LoadUserParamInt("Widgets-ColumnsPerPage", 2);
            v.recX56 = _f.x55WidgetBL.LoadState(_f.CurrentUser.pid);

            v.lisAllWidgets = _f.x55WidgetBL.GetList(new BO.myQuery("x55") { IsRecordValid = true });
            v.lisUserWidgets = new List<BO.x55Widget>();
            var boxes = BO.BAS.ConvertString2List(v.recX56.x56Boxes);
            foreach (string s in boxes)
            {
                if (v.lisAllWidgets.Where(p => p.x55Code == s).Count() > 0)
                {
                    v.lisUserWidgets.Add(v.lisAllWidgets.Where(p => p.x55Code == s).First());
                }
            }


            v.DockStructure = new WidgetsEnvironment(v.recX56.x56DockState);
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
