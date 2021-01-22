using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;


namespace UI
{
    public class IcalSupport
    {
        private StringBuilder _sb { get; set; }
        public IcalSupport()
        {
            _sb = new StringBuilder();
        }

        public string getPersonalCalendar(BL.Factory f,BO.j02Person recJ02,DateTime? d1,DateTime? d2)
        {
            if (d1 == null)
            {
                d1 = DateTime.Today.AddYears(-1);                
            }
            if (d2 == null)
            {
                d2 = DateTime.Today.AddYears(1);
            }
            sr_vcalendar_header();
            sr_timezone();

            var mqA35 = new BO.myQueryA35() { j02id = recJ02.pid, global_d1 =d1,global_d2=d2};
            var lisA35 = f.a35PersonEventPlanBL.GetList(mqA35);
            List<int> a01ids = lisA35.Select(p => p.a01ID).Distinct().ToList();
            var lisA01 = f.a01EventBL.GetList(new BO.myQueryA01() { pids = a01ids });
            foreach (var c in lisA35)
            {
                if (lisA01.Any(p => p.pid == c.a01ID))
                {
                    a35_record(c, lisA01.Where(p => p.pid == c.a01ID).First());
                }
                else
                {
                    a35_record(c, null);
                }                
            }

            var mqH04 = new BO.myQueryH04() { j02id = recJ02.pid, global_d1 = d1, global_d2 = d2 };
            var lisH04 = f.h04ToDoBL.GetList(mqH04);
            foreach (var c in lisH04)
            {
                h04_record(c);
            }

            sr("END:VCALENDAR");

            return _sb.ToString();
        }

        private void h04_record(BO.h04ToDo rec)
        {
            sr("BEGIN:VEVENT");


            sr("UID:h04id-" + rec.pid.ToString());

            sr("DTSTAMP:" + Convert.ToDateTime(rec.DateInsert).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));

            if (rec.h04CapacityPlanFrom !=null && rec.h04CapacityPlanUntil !=null)
            {
                sr("DTSTART:" + Convert.ToDateTime(rec.h04CapacityPlanFrom).ToString("yyyyMMdd"));
                sr("DTEND:" + Convert.ToDateTime(rec.h04CapacityPlanUntil).AddDays(1).ToString("yyyyMMdd"));
            }
            else
            {
                sr("DTSTART:" + Convert.ToDateTime(rec.h04Deadline).ToString("yyyyMMddTHHmmssZ"));
                sr("DTEND:" + Convert.ToDateTime(rec.h04Deadline).ToString("yyyyMMddTHHmmssZ"));
            }
            
            if (rec.h04ReminderDate != null && rec.h04Deadline !=null)
            {
                sr("BEGIN:VALARM");
                TimeSpan dur = Convert.ToDateTime(rec.h04ReminderDate) - rec.h04Deadline;
                sr(String.Format("TRIGGER:-PT{0}{1}", Convert.ToInt32(dur.TotalMinutes), "M"));      //v minutách
                sr("ACTION:DISPLAY");

                sr("DESCRIPTION:" + rec.h04Name + " [" + rec.h07Name + "]");
                sr("END:VALARM");
            }

            sr("SUMMARY:" + rec.h04Name + " [" + rec.h07Name + "]");
            string strDescription = rec.h07Name;
            if (rec.h07IsToDo)
            {
                strDescription += " (" + rec.h05Name + ")";
            }
            strDescription += "\n" +"Akce: "+ rec.a01Signature;
            if (rec.h04Description != null)
            {
                strDescription += "\n\n" + strDescription;
            }

          
            sr("DESCRIPTION:" + String.Join("\n", strDescription));

            sr("TRANSP:OPAQUE");

            sr("END:VEVENT");
        }

        private void a35_record(BO.a35PersonEventPlan rec,BO.a01Event recA01)
        {
            sr("BEGIN:VEVENT");


            sr("UID:a35-" + rec.a35ID.ToString());

            sr("DTSTAMP:" + Convert.ToDateTime(rec.a35PlanDate).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));

            sr("DTSTART:" + Convert.ToDateTime(rec.a35PlanDate).ToString("yyyyMMdd"));
            sr("DTEND:" + Convert.ToDateTime(rec.a35PlanDate.AddDays(1)).ToString("yyyyMMdd"));

            
            if (recA01 != null && recA01.a03ID>0)
            {
                sr("SUMMARY:" + recA01.a03Name);
                sr("DESCRIPTION:" + String.Join("\n", "Časový plán akce: "+ rec.a01Signature+", "+recA01.a03Name));
            }
            else
            {
                sr("SUMMARY:" + rec.a01Signature);
                sr("DESCRIPTION:" + String.Join("\n", "Časový plán akce: " + rec.a01Signature));
            }
                       
            sr("TRANSP:OPAQUE");

            sr("END:VEVENT");            
        }


        private void sr_vcalendar_header()
        {
            
            sr("BEGIN:VCALENDAR");
            sr("VERSION:2.0");
            sr("METHOD:PUBLISH");
            sr("PRODID:marktime.cz");
            sr("X-SZN-COLOR:#088acc");
            sr("X-WR-CALNAME:INSPIS");
        }

        private void sr_timezone()
        {
            sr("BEGIN:VTIMEZONE");
            sr("TZID:Europe/Prague");
            sr("BEGIN:STANDARD");
            sr("DTSTART:20001029T030000");
            sr("RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=10");
            sr("TZNAME:CET");
            sr("TZOFFSETFROM:+0200");
            sr("TZOFFSETTO:+0100");
            sr("END:STANDARD");
            sr("BEGIN:DAYLIGHT");
            sr("DTSTART:20000326T020000");
            sr("RRULE:FREQ=YEARLY;BYDAY=-1SU;BYMONTH=3");
            sr("TZNAME:CEST");
            sr("TZOFFSETFROM:+0100");
            sr("TZOFFSETTO:+0200");
            sr("END:DAYLIGHT");
            sr("END:VTIMEZONE");
        }

        private void sr(string s)
        {
            _sb.AppendLine(s);
        }
    }
}
