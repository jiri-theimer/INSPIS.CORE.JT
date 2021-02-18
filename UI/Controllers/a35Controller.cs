using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Tab;

namespace UI.Controllers
{
    public class a35Controller : BaseController
    {
        public IActionResult TimeLine(int go2month, int go2year, int a05id,int a01id)
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

            var v = new a35TimeLineViewModel() { CurMonth = go2month, CurYear = go2year, a05ID = a05id,a01ID=a01id };
            
            v.PersonQueryFlag = Factory.CBL.LoadUserParamInt("a35TimeLine-PersonQueryFlag", 0);
            v.A38QueryFlag = Factory.CBL.LoadUserParamInt("a35TimeLine-A38QueryFlag", 0);
            v.o51IDs = Factory.CBL.LoadUserParam("a35TimeLine-o51IDs") ;
            
            if (v.a05ID == 0)
            {
                v.a05ID = Factory.CBL.LoadUserParamInt("a35TimeLine-a05ID", Factory.CurrentUser.a05ID);                
            }
            if (v.a05ID>0) v.a05Name = Factory.a05RegionBL.Load(v.a05ID).a05Name;

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

            var mqJ02 = new BO.myQueryJ02();
            mqJ02.IsRecordValid = true;
            if (v.a05ID > 0)
            {
                mqJ02.a05id = v.a05ID;
            }
            else
            {
                mqJ02.a02inspector = true;               
            }
            if (!string.IsNullOrEmpty(v.o51IDs))
            {
                mqJ02.o51ids = BO.BAS.ConvertString2ListInt(v.o51IDs);
            }

            mqJ02.explicit_orderby = "a.j02LastName";
            v.lisJ02 = Factory.j02PersonBL.GetList(mqJ02);
            if (v.PersonQueryFlag == 1)
            {
                v.lisJ02 = v.lisJ02.Where(p => p.j02IsInvitedPerson == true);   //filtr: pouze přizvané osoby
            }
            if (v.PersonQueryFlag == 2)
            {
                v.lisJ02 = v.lisJ02.Where(p => p.j02IsInvitedPerson == false);   //filtr: pouze inspektoři
            }
            
            v.lisTimeLine = Factory.a35PersonEventPlanBL.GetListTimeLine(new BO.myQueryA35() { global_d1 = d1, global_d2 = d2, a05id = v.a05ID });

            
            v.lisH04 = Factory.h04ToDoBL.GetListCapacity(new BO.myQueryH04() { global_d1 = d1, global_d2 = d2, a05id = v.a05ID });

            if (v.A38QueryFlag > 0)
            {                
                v.lisTimeLineA38 = Factory.a38NonPersonEventPlanBL.GetListPersonTimeLine(new BO.myQueryA38() { global_d1 = d1, global_d2 = d2, a05id = v.a05ID });
            }
            v.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= d1 && p.j26Date <= d2);
           
            if (!string.IsNullOrEmpty(v.o51IDs))
            {
                var mq = new BO.myQuery("o51") { pids = BO.BAS.ConvertString2ListInt(v.o51IDs) };
                v.o51Names = string.Join(", ", Factory.o51TagBL.GetList(mq).Select(p => p.o51Name));
            }

            if (v.a01ID > 0)
            {
                
                v.lisA41 = Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = v.a01ID });
             }


            return View(v);
        }

        public IActionResult CapacityEdit(int pid, int go2month, int go2year)
        {
            var v = new a01TabCapacityEdit() { pid = pid, CurMonth = go2month, CurYear = go2year };
            if (v.pid == 0)
            {
                return this.StopPage(true, "a01id missing.");
            }
            RefreshStateCapacityEdit(v);
            v.DateFrom = Convert.ToDateTime(v.RecA01.a01DateFrom); v.DateUntil = Convert.ToDateTime(v.RecA01.a01DateUntil);

            InhaleCapacityTimeline(v);

            if (v.PermA01.PermValue == BO.a01EventPermissionENUM.ShareTeam_Leader || v.PermA01.PermValue == BO.a01EventPermissionENUM.ShareTeam_Owner || v.PermA01.PermValue == BO.a01EventPermissionENUM.FullAccess)
            {
                return View(v);
            }
            else
            {
                return this.StopPage(true, "Chybí oprávnění.");
            }


        }
        [HttpPost]
        public IActionResult CapacityEdit(a01TabCapacityEdit v, string oper)
        {
            RefreshStateCapacityEdit(v);
            InhaleCapacityTimeline(v);
            if (oper == "gonext")
            {
                v.CurMonth = v.NextMonth; v.CurYear = v.NextYear;
                InhaleCapacityTimeline(v);
                return View(v);
            }
            if (oper == "goprev")
            {
                v.CurMonth = v.PrevMonth; v.CurYear = v.PrevYear;
                InhaleCapacityTimeline(v);
                return View(v);
            }
            if (ModelState.IsValid)
            {
                var c = Factory.a01EventBL.Load(v.pid);
                c.a01DateFrom = v.DateFrom;
                c.a01DateUntil = v.DateUntil;
                
                c.pid = Factory.a01EventBL.SaveA01Record(c, Factory.a10EventTypeBL.Load(c.a10ID));
                if (c.pid > 0)
                {
                    var chks = BO.BAS.ConvertString2List(v.CheckedDays);
                    foreach (string strChk in chks)
                    {
                        var rec = new BO.a35PersonEventPlan() { a01ID = v.pid };
                        var arr = strChk.Split("_");
                        rec.j02ID = Convert.ToInt32(arr[0]);
                        rec.a35PlanDate = BO.BAS.String2Date(arr[1]);

                        if (v.lisA35.Where(p => p.j02ID == rec.j02ID && p.a35PlanDate == rec.a35PlanDate).Count() == 0)
                        {
                            Factory.a35PersonEventPlanBL.Save(rec);
                        }

                    }
                    chks = BO.BAS.ConvertString2List(v.UnCheckedDays);
                    foreach (string strChk in chks)
                    {
                        var arr = strChk.Split("_");
                        int intJ02ID = Convert.ToInt32(arr[0]);
                        DateTime d = BO.BAS.String2Date(arr[1]);
                        if (v.lisA35.Where(p => p.j02ID == intJ02ID && p.a35PlanDate == d).Count() > 0)
                        {
                            Factory.CBL.DeleteRecord("a35", v.lisA35.Where(p => p.j02ID == intJ02ID && p.a35PlanDate == d).First().pid);
                        }

                    }


                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
        private void RefreshStateCapacityEdit(a01TabCapacityEdit v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);
            
            v.lisA41 = Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = v.pid }).OrderBy(p => p.PersonDesc).Where(p => p.a45ID != BO.EventRoleENUM.Vlastnik);
            if (v.lisA35 == null)
            {               
                v.lisA35 = Factory.a35PersonEventPlanBL.GetList(new BO.myQueryA35() { a01id = v.pid });
            }
            v.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= v.RecA01.a01DateFrom && p.j26Date <= v.RecA01.a01DateUntil);

           
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

        public IActionResult TabCapacity(int pid, int go2month, int go2year)
        {
            var v = new a01TabCapacity() { pid = pid, CurMonth = go2month, CurYear = go2year };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid missing");
            }
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            if (Factory.a10EventTypeBL.Load(v.RecA01.a10ID).a10IsUse_Period == false)
            {
                return this.StopPageSubform(string.Format("Typ akce '{0}' nepodporuje časové plánování.", v.RecA01.a10Name));
            }
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);

            
            v.lisA35 = Factory.a35PersonEventPlanBL.GetList(new BO.myQueryA35() { a01id = v.pid });
            
            v.lisA38 = Factory.a38NonPersonEventPlanBL.GetList(new BO.myQueryA38() { a01id = v.pid });
            
            v.lisA41 = Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = v.pid }).OrderBy(p => p.PersonDesc).Where(p => p.a45ID != BO.EventRoleENUM.Vlastnik);
            v.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= v.RecA01.a01DateFrom && p.j26Date <= v.RecA01.a01DateUntil);

           
            v.lisH04 = Factory.h04ToDoBL.GetListCapacity(new BO.myQueryH04() { a01id = v.pid });
            InhaleCapacityTimeline(v);

            

            return View(v);
        }

        public BO.Result AppendToEvent(int j02id,int a01id)
        {
            var recA01 = Factory.a01EventBL.Load(a01id);
            var recJ02 = Factory.j02PersonBL.Load(j02id);
            
            if (Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = a01id, j02id = j02id }).Count() > 0)
            {
                return new BO.Result(true, Factory.tra("Zvolená osoba již je členem této akce."));
            }
            var recA41 = new BO.a41PersonToEvent() { a01ID = recA01.pid, j02ID = recJ02.pid, a41IsAllocateAllPeriod = true, a45ID = BO.EventRoleENUM.Resitel };
            
            if (Factory.a41PersonToEventBL.Save(recA41, true)>0)
            {
                for(DateTime d = Convert.ToDateTime(recA01.a01DateFrom); d <= Convert.ToDateTime(recA01.a01DateUntil); d = d.AddDays(1))
                {
                    var c = new BO.a35PersonEventPlan() { a01ID = a01id, j02ID = j02id, a35PlanDate = d };
                    Factory.a35PersonEventPlanBL.Save(c);
                }
                
                return new BO.Result(false,Factory.tra("Vloženo do akce."));
            }
            else
            {
                return new BO.Result(true,"error");
            }
            
        }

        public IActionResult TabPersonalTimeline(int pid, int go2month, int go2year)
        {
            if (pid == 0)
            {
                pid = Factory.CurrentUser.j02ID;
            }
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

            var v = new j02PersonalTimeline() { pid = pid, CurMonth = go2month, CurYear = go2year };
            v.Rec = Factory.j02PersonBL.Load(v.pid);

            v.lisYears = new List<int>();
            for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 2; i++)
            {
                v.lisYears.Add(i);
            }
            v.d1 = new DateTime(v.CurYear, v.CurMonth, 1);
            v.d2 = v.d1.AddMonths(1).AddDays(-1);
            v.NextMonth = v.d1.AddMonths(1).Month; v.NextYear = v.d1.AddMonths(1).Year; v.PrevMonth = v.d1.AddMonths(-1).Month; v.PrevYear = v.d1.AddMonths(-1).Year;
            v.lisDays = new List<DateTime>();
            for (var d = v.d1; d <= v.d2; d = d.AddDays(1))
            {
                v.lisDays.Add(d);
            }
            v.lisJ26 = Factory.j26HolidayBL.GetList(new BO.myQuery("j26")).Where(p => p.j26Date >= v.d1 && p.j26Date <= v.d2);

            var mq = new BO.myQueryA35() { j02id = v.pid, global_d1 = v.d1, global_d2 = v.d2, ZahrnoutCeleObdobiAkce = true };
            v.lisTimeLine = Factory.a35PersonEventPlanBL.GetListTimeLinePersonal(mq);

            v.lisH04 = Factory.h04ToDoBL.GetListCapacity(new BO.myQueryH04() { global_d1 = v.d1, global_d2 = v.d2, j02id = v.pid });
            v.lisA38 = Factory.a38NonPersonEventPlanBL.GetList(new BO.myQueryA38() { global_d1 = v.d1, global_d2 = v.d2, j02id = v.pid });

            
            return View(v);
        }


        public IActionResult ical(string guid)
        {
            
            var cgen = new IcalSupport();
            var recJ02 = Factory.j02PersonBL.LoadByGuid(guid);
            if (recJ02 == null)
            {
                return Content("personal guid not found");
            }

            string strFileName = recJ02.j02Guid + ".ics";

            
            Response.Headers["Content-Type"] = "text/calendar";
            Response.Headers["Content-Disposition"] = string.Format("attachment; filename={0}", strFileName);

            //var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "text/plain");

            return Content(cgen.getPersonalCalendar(Factory, recJ02, null, null));
        }
    }
}