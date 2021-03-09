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
        public IActionResult CreateAus(int a01id, int go2month, int go2year)
        {
            var v = new a38CreateAusViewModel() { a01ID = a01id, CurMonth = go2month, CurYear = go2year };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing.");
            }

            RefreshState_CreateAus(v);
            v.a05ID = v.RecA01.a05ID;
            v.a05Name = v.RecA01.a05Name;

            return View(v);
        }
        [HttpPost]
        public IActionResult CreateAus(a38CreateAusViewModel v, string oper, int a38id)
        {
            RefreshState_CreateAus(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "delete")
            {
                Factory.CBL.DeleteRecord("a38", a38id);
                RefreshState_CreateAus(v);
                return View(v);
            }
            if (ModelState.IsValid)
            {    
                if (v.D1==null || v.D2 == null)
                {
                    this.AddMessage("Musíte vyplnit [Datum od] i [Datum do]."); return View(v);
                }
                if (v.D1 > v.D2)
                {
                    this.AddMessage("[Datum do] nesmí být menší než [Datum od].");return View(v);
                }
                if (v.SelectedJ23ID == 0)
                {
                    this.AddMessage("Na vstupu chybí nepersonální zdroj."); return View(v);
                }
                if (v.SelectedJ02ID == 0)
                {
                    this.AddMessage("Na vstupu chybí ID osoby nebo ID akce."); return View(v);
                }
               
                var lis2SaveA38 = new List<BO.a38NonPersonEventPlan>();                
                for (DateTime d = Convert.ToDateTime(v.D1); d <= Convert.ToDateTime(v.D2); d = d.AddDays(1))
                {                    
                    var cA38 = new BO.a38NonPersonEventPlan() {a38PlanDate=d,a38IsDriver=v.a38IsDriver, a01ID = v.a01ID, j02ID = v.SelectedJ02ID, j23ID = v.SelectedJ23ID, j25ID = BO.BAS.InInt(v.SelectedJ25ID), a38Location=v.a38Location };
                    if (Factory.a38NonPersonEventPlanBL.ValidateBeforeSave(cA38)==false)
                    {
                        return View(v);
                    }
                    
                    lis2SaveA38.Add(cA38);
                    

                }

                int intOKs = 0;
                foreach(var c in lis2SaveA38)
                {
                    int intPID = Factory.a38NonPersonEventPlanBL.Save(c);
                    if (intPID > 0)
                    {
                        var cA35 = new BO.a35PersonEventPlan() { a01ID = c.a01ID, j02ID = c.j02ID, a35PlanDate = c.a38PlanDate };
                        if (Factory.a35PersonEventPlanBL.Save(cA35) > 0)
                        {
                            intOKs += 1;
                        }
                    }
                }
                
                
                
                
                if (intOKs ==lis2SaveA38.Count)
                {
                    v.SetJavascript_CallOnLoad(0);
                    return View(v);
                }
                else
                {
                    RefreshState_CreateAus(v);
                    return View(v);
                }


            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        public IActionResult AppendToA01(int pid, int go2month, int go2year)
        {
            var v = new a38AppendToA01ViewModel() { pid = pid, CurMonth = go2month, CurYear = go2year };
            if (v.pid == 0)
            {
                return this.StopPage(true, "a01id missing.");
            }

            RefreshState_AppendToA01(v);
            if (v.RecA01.isclosed)
            {
                return this.StopPage(true, "Tato akce je již uzavřena.",true);
            }
            v.a05ID = v.RecA01.a05ID;
            v.a05Name = v.RecA01.a05Name;

            return View(v);
        }
        [HttpPost]
        public IActionResult AppendToA01(a38AppendToA01ViewModel v, string oper,int a38id)
        {
            RefreshState_AppendToA01(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "delete")
            {
                Factory.CBL.DeleteRecord("a38", a38id);
                RefreshState_AppendToA01(v);
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
                        this.AddMessageTranslated(string.Format("[{0}] má pro den [{1}] a zdroj [{2}] již uloženou rezervaci.", recA35.PersonAsc, recA35.a35PlanDate, v.SelectedJ23Name));
                        bolIsOK = false;
                    }                    
                    if (drivers.Where(p => p == recA35.pid.ToString() + "-1").Count() > 0)
                    {
                        if (v.lisA38.Where(p => p.a38PlanDate == recA35.a35PlanDate && p.j23ID == v.SelectedJ23ID && p.a38IsDriver == true).Count() > 0)
                        {
                            this.AddMessageTranslated(string.Format("Pro den [{0}] a zdroj [{1}] již existuje rezervace s řidičem.", recA35.a35PlanDate, v.SelectedJ23Name));
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

        private void RefreshState_CreateAus(a38CreateAusViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (v.SelectedJ23ID > 0)
            {
                v.RecJ23 = Factory.j23NonPersonBL.Load(v.SelectedJ23ID);
            }

            var cp = new a01TabCapacity() { pid = v.a01ID, CurMonth = v.CurMonth, CurYear = v.CurYear, RecA01 = v.RecA01 };
                      
            cp.lisA35 = Factory.a35PersonEventPlanBL.GetList(new BO.myQueryA35() { a01id = v.a01ID });
            v.lisA35 = cp.lisA35.Where(p => p.j02ID == v.SelectedJ02ID);            
            
            cp.lisA38 = Factory.a38NonPersonEventPlanBL.GetList(new BO.myQueryA38() { a01id =v.a01ID });
            v.lisA38 = cp.lisA38;
            var mq = new BO.myQuery("j25") { IsRecordValid = true };
            v.lisJ25 = Factory.j25NonPersonPlanReasonBL.GetList(mq);
            
            cp.lisA41 = Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = v.a01ID }).OrderBy(p => p.PersonDesc).Where(p => p.a45ID != BO.EventRoleENUM.Vlastnik);
            cp.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= v.RecA01.a01DateFrom && p.j26Date <= v.RecA01.a01DateUntil);
            
            cp.lisH04 = Factory.h04ToDoBL.GetListCapacity(new BO.myQueryH04() { a01id = v.a01ID });
            InhaleCapacityTimeline(cp);

            v.CapacityView = cp;

        }
        private void RefreshState_AppendToA01(a38AppendToA01ViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (v.SelectedJ23ID > 0)
            {
                v.RecJ23 = Factory.j23NonPersonBL.Load(v.SelectedJ23ID);
            }

            var cp = new a01TabCapacity() { pid = v.pid, CurMonth = v.CurMonth, CurYear = v.CurYear, RecA01 = v.RecA01 };
           
            cp.lisA35 = Factory.a35PersonEventPlanBL.GetList(new BO.myQueryA35() { a01id = v.pid });
            v.lisA35 = cp.lisA35.Where(p => p.j02ID == v.SelectedJ02ID);
            
            cp.lisA38 = Factory.a38NonPersonEventPlanBL.GetList(new BO.myQueryA38() { a01id = v.pid });
            v.lisA38 = cp.lisA38;    //.Where(p => p.j23ID == v.SelectedJ23ID);
                      
            cp.lisA41 = Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = v.pid }).OrderBy(p => p.PersonDesc).Where(p => p.a45ID != BO.EventRoleENUM.Vlastnik);
            cp.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= v.RecA01.a01DateFrom && p.j26Date <= v.RecA01.a01DateUntil);
            
            cp.lisH04 = Factory.h04ToDoBL.GetListCapacity(new BO.myQueryH04() { a01id = v.pid });

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
                    if (v.lisJ26.Where(p => p.j26Date == d).Count() == 0) v.PracovnichDni += 1;                    
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

            var mqA38 = new BO.myQueryA38();
            mqA38.global_d1 = d1;
            mqA38.global_d2 = d2;
            mqA38.a05id = v.a05ID;
            v.lisTimeLine = Factory.a38NonPersonEventPlanBL.GetListTimeLine(mqA38);
            v.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p=>p.j26Date>=d1 && p.j26Date<=d2);

            return View(v);
        }
    }
}