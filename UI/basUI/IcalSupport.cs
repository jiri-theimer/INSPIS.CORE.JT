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

        public string GenerateCalendar(BL.Factory f,BO.j02Person recJ02)
        {
            
            sr_vcalendar_header();
            sr_timezone();


            sr("END:VCALENDAR");

            return _sb.ToString();
        }

        private void a35_record(BO.a35PersonEventPlan rec)
        {
            sr("BEGIN:VEVENT");


            sr("UID:a35-" + rec.a35ID.ToString());

            sr("DTSTAMP:" + Convert.ToDateTime(rec.a35PlanDate).ToUniversalTime().ToString("yyyyMMddTHHmmssZ"));

            sr("DTSTART:" + Convert.ToDateTime(rec.a35PlanDate).ToString("yyyyMMdd"));
            sr("DTEND:" + Convert.ToDateTime(rec.a35PlanDate.AddDays(1)).ToString("yyyyMMdd"));

            sr("SUMMARY:" + rec.a01Signature);
            sr("DESCRIPTION:" + String.Join("\n", "Časový plán akce"+rec.a01Signature));

            sr("TRANSP:OPAQUE");

            sr("END:VEVENT");

            
        }


        private void sr_vcalendar_header()
        {
            
            sr("BEGIN:VCALENDAR");
            sr("VERSION:2.0");
            sr("METHOD:PUBLISH");
            sr("PRODID:marktime.net");
            sr("X-SZN-COLOR:#088acc");
            sr("X-WR-CALNAME:MARKTIME");
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
