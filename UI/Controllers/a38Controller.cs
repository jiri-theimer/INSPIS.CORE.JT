using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Tab;

namespace UI.Controllers
{
    public class a38Controller : BaseController
    {
        public IActionResult AppendToA01(int pid, int go2month, int go2year)
        {
            var v = new a38AppendToA01ViewModel() { pid = pid, CurMonth = go2month, CurYear = go2year };
            if (v.pid == 0)
            {
                return this.StopPage(true, "a01id missing.");
            }

            RefreshState_a38Edit(v);
            v.a05ID = v.RecA01.a05ID;
            v.a05Name = v.RecA01.a05Name;

            return View(v);
        }
        [HttpPost]
        public IActionResult AppendToA01(a38AppendToA01ViewModel v, string oper,int a38id)
        {
            RefreshState_a38Edit(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "delete")
            {
                Factory.CBL.DeleteRecord("a38", a38id);
                RefreshState_a38Edit(v);
                return View(v);
            }
            if (ModelState.IsValid)
            {
                if (v.SelectedJ23ID == 0)
                {
                    this.AddMessage("Na vstupu chybí nepersonální zdroj."); return View(v);
                }
                if (v.SelectedJ02ID == 0)
                {
                    this.AddMessage("Na vstupu chybí osobní profil."); return View(v);
                }

                var a35ids = BO.BAS.ConvertString2ListInt(v.CheckedDays); bool bolIsOK = true;
                var drivers = BO.BAS.ConvertString2List(v.CheckedIsDriver);
                if (a35ids.Count() == 0)
                {
                    this.AddMessage("Musíte zaškrtnout minimálně jedno datum.");return View(v);
                }
                foreach (int intA35ID in a35ids)
                {
                    var recA35 = Factory.a35PersonEventPlanBL.Load(intA35ID);
                    if (v.lisA38.Where(p => p.j02ID == recA35.j02ID && p.a38PlanDate == recA35.a35PlanDate && p.j23ID == v.SelectedJ23ID).Count() > 0)
                    {
                        this.AddMessage(string.Format("[{0}] má pro den [{1}] a zdroj [{2}] již uloženou rezervaci.", recA35.PersonAsc, recA35.a35PlanDate, v.SelectedJ23Name));
                        bolIsOK = false;
                    }                    
                    if (drivers.Where(p => p == recA35.pid.ToString() + "-1").Count() > 0)
                    {
                        if (v.lisA38.Where(p => p.a38PlanDate == recA35.a35PlanDate && p.j23ID == v.SelectedJ23ID && p.a38IsDriver == true).Count() > 0)
                        {
                            this.AddMessage(string.Format("Pro den [{0}] a zdroj [{1}] již existuje rezervace s řidičem.", recA35.a35PlanDate, v.SelectedJ23Name));
                            bolIsOK = false;
                        }
                    }
                    
                }
                if (bolIsOK == false)
                {
                    return View(v);
                }
                foreach (int intA35ID in a35ids)
                {
                    var recA35 = Factory.a35PersonEventPlanBL.Load(intA35ID);
                    var recA38 = new BO.a38NonPersonEventPlan() { a01ID = v.pid, j02ID = v.SelectedJ02ID, a38PlanDate = recA35.a35PlanDate, j23ID = v.SelectedJ23ID,a38Location=v.a38Location };
                    if (drivers.Where(p => p == recA35.pid.ToString() + "-1").Count() > 0)
                    {
                        recA38.a38IsDriver = true;
                    }
                    if (Factory.a38NonPersonEventPlanBL.Save(recA38) == 0)
                    {
                        bolIsOK = false;
                    }

                }

                if (bolIsOK == true)
                {
                    v.SetJavascript_CallOnLoad(0);
                    return View(v);
                }
                
                
            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState_a38Edit(a38AppendToA01ViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (v.SelectedJ23ID > 0)
            {
                v.RecJ23 = Factory.j23NonPersonBL.Load(v.SelectedJ23ID);
            }

            var cp = new a01TabCapacity() { pid = v.pid, CurMonth = v.CurMonth, CurYear = v.CurYear, RecA01 = v.RecA01 };
            var mq = new BO.myQuery("a35");
            mq.a01id = v.pid;
            cp.lisA35 = Factory.a35PersonEventPlanBL.GetList(mq);
            v.lisA35 = cp.lisA35.Where(p => p.j02ID == v.SelectedJ02ID);
            mq = new BO.myQuery("a38");
            mq.a01id = v.pid;
            cp.lisA38 = Factory.a38NonPersonEventPlanBL.GetList(mq);
            v.lisA38 = cp.lisA38;    //.Where(p => p.j23ID == v.SelectedJ23ID);
          
            mq = new BO.myQuery("a41");
            mq.a01id = v.pid;
            cp.lisA41 = Factory.a41PersonToEventBL.GetList(mq).OrderBy(p => p.PersonDesc).Where(p => p.a45ID != BO.EventRoleENUM.Vlastnik);
            cp.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= v.RecA01.a01DateFrom && p.j26Date <= v.RecA01.a01DateUntil);
            mq = new BO.myQuery("h04");
            mq.a01id = v.pid;
            cp.lisH04 = Factory.h04ToDoBL.GetListCapacity(mq);

            InhaleCapacityTimeline(cp);

            v.CapacityView = cp;

        }
        private void InhaleCapacityTimeline(a01TabCapacity v)
        {
            v.lisDays = new List<DateTime>();
            var d1 = (DateTime)v.RecA01.a01DateFrom;
            var d2 = (DateTime)v.RecA01.a01DateUntil;
            if (d1.AddDays(180) < d2 || v.CurMonth > 0)
            {
                v.IsMonthView = true;   //nad 180 dní to řešit jako zobrazení jedniného měsíce
                if (v.CurMonth == 0 || v.CurYear == 0)
                {
                    v.CurMonth = d1.Month; v.CurYear = d1.Year;
                    if (d2 >= DateTime.Today && d1 <= DateTime.Today)
                    {
                        v.CurMonth = DateTime.Today.Month; v.CurYear = DateTime.Today.Year;
                    }
                }
                d1 = new DateTime(v.CurYear, v.CurMonth, 1);
                d2 = d1.AddMonths(1).AddDays(-1);
            }
            for (var d = d1; d <= d2; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                {
                    v.PracovnichDni += 1;
                }

            }
            if (v.IsMonthView)
            {
                v.NextMonth = d1.AddMonths(1).Month;
                v.NextYear = d1.AddMonths(1).Year;
                v.PrevMonth = d1.AddMonths(-1).Month;
                v.PrevYear = d1.AddMonths(-1).Year;
            }
            else
            {
                if (d1.Day <= d2.Day)
                {
                    d1 = new DateTime(d1.Year, d1.Month, 1);
                }
                else
                {
                    d2 = new DateTime(d2.Year, d2.Month, 1).AddMonths(1).AddDays(-1);
                }
                if (d1.Month == d2.Month)
                {
                    d2 = new DateTime(d2.Year, d2.Month, 1).AddMonths(1).AddDays(-1);
                }
            }


            for (var d = d1; d <= d2; d = d.AddDays(1))
            {
                v.lisDays.Add(d);
            }
        }

        public IActionResult TimeLine(int go2month, int go2year, int a05id)
        {
            if (go2month == 0 || go2year == 0)
            {
                go2month = Factory.CBL.LoadUserParamInt("TimeLine-Month", DateTime.Now.Month);
                go2year = Factory.CBL.LoadUserParamInt("TimeLine-Year", DateTime.Now.Year);
            }
            else
            {
                Factory.CBL.SetUserParam("TimeLine-Month", go2month.ToString());
                Factory.CBL.SetUserParam("TimeLine-Year", go2year.ToString());
            }

            var v = new a38TimelineViewModel() { CurMonth = go2month, CurYear = go2year, a05ID = a05id };

            if (v.a05ID == 0)
            {
                v.a05ID = Factory.CBL.LoadUserParamInt("a38TimeLine-a05ID", Factory.CurrentUser.a05ID);
            }
            if (v.a05ID > 0) v.a05Name = Factory.a05RegionBL.Load(v.a05ID).a05Name;

            v.lisYears = new List<int>();
            for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 2; i++)
            {
                v.lisYears.Add(i);
            }
            var d1 = new DateTime(v.CurYear, v.CurMonth, 1);
            var d2 = d1.AddMonths(1).AddDays(-1);
            v.NextMonth = d1.AddMonths(1).Month; v.NextYear = d1.AddMonths(1).Year; v.PrevMonth = d1.AddMonths(-1).Month; v.PrevYear = d1.AddMonths(-1).Year;
            v.lisDays = new List<DateTime>();
            for (var d = d1; d <= d2; d = d.AddDays(1))
            {
                v.lisDays.Add(d);
            }

            var mq = new BO.myQuery("j23");
            mq.IsRecordValid = true;
            if (v.a05ID > 0)
            {
                mq.a05id = v.a05ID;
            }

            mq.explicit_orderby = "a.j23Name,a.j23Code";
            v.lisJ23 = Factory.j23NonPersonBL.GetList(mq);

            mq = new BO.myQuery("a38");
            mq.global_d1 = d1;
            mq.global_d2 = d2;
            mq.a05id = v.a05ID;
            v.lisTimeLine = Factory.a38NonPersonEventPlanBL.GetListTimeLine(mq);
            v.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p=>p.j26Date>=d1 && p.j26Date<=d2);

            return View(v);
        }
    }
}