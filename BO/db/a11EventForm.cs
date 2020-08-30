using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a11EventForm:BaseBO
    {
        [Key]
        public int a11ID { get; set; }
        public int a01ID { get; set; }
        public int f06ID { get; set; }
        public int a37ID { get; set; }
        public int k01ID { get; set; }
        public int a25ID { get; set; }
        public int a22ID { get; set; }

        public bool a11IsPoll { get; set; }
        public bool a11IsLocked { get; set; }
        public bool a11IsSimulation { get; set; }
        public bool a11IsInProcessing { get; set; }
        public int a11Ordinal { get; set; }
        public string a11Description { get; set; }
        public string a11AccessToken { get; set; }
        public string a11TeacherPID { get; set; }
        public string a11IzoOffice { get; set; }
        public DateTime? a11LockDate { get; set; }
        public bool a11IsLockedByWorkflow { get; set; }

        //readonly        
        public bool a01IsAllFormsClosed;
        public bool a01IsClosed;
        public DateTime a01ValidFrom;
        public DateTime a01ValidUntil;        
        public string f06Name { get; set; } //combo
        public string a37Name { get; set; } //combo
        public string a37IZO;
        public string a25Name { get; set; } //combo
        public string k01FullName_Desc { get; set; }    //combo
        public int a03ID;
        public string a25Color;
        public DateTime f06ValidFrom;
        public DateTime f06ValidUntil;
        public bool f06IsA01ClosedStrict;
        public bool f06IsA01PeriodStrict;
        public int f06UserLockFlag;
        public string FormNameHtml
        {
            get
            {
                string s = "<span style='font-weight:normal;";
                if (this.a11IsInProcessing)
                {
                    s += "background-color:yellow;'";
                }
                if (this.isclosed)
                {
                    s += "text-decoration:line-through;";
                    if (this.a01IsAllFormsClosed)
                    {
                        s += "color:red;";
                    }
                }
                s += "'>"+this.f06Name+"</span>";

                return s;
            }
        }
        public string FullNameHtml
        {
            get
            {
                string s = this.FormNameHtml;
                
                if (this.a37ID > 0)
                {
                    s += " <span style='color:orange;'>" + this.a37IZO + " - " + this.a37Name + "</span>";
                }
                if (this.a11Description != null)
                {
                    s += " <i>" + this.a11Description + "</i>";
                }
                if (this.a25ID > 0)
                {
                    s += " <span style='color:navy;'>" + this.a25Name+ "</span>";
                    if (this.a25Color != null)
                    {
                        s += "<span style='background-color:" + this.a25Color + ";'>🚩</span>";
                    }
                }

                return s;
            }
        }
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:flex";
                }
            }
        }

    }
}
