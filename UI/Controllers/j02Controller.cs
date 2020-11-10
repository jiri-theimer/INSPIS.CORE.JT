using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class j02Controller : BaseController
    {
        public IActionResult InfoCapacity(int pid,int m,int y)
        {
            var v = new j02InfoCapacity() { pid = pid };
            v.Rec = Factory.j02PersonBL.Load(v.pid);
            if (v.Rec.a04ID > 0)
            {
                v.RecA04 = Factory.a04InspectorateBL.Load(v.Rec.a04ID);
            }
            if (v.Rec.j03ID > 0)
            {
                v.RecJ03 = Factory.j03UserBL.Load(v.Rec.j03ID);
            }
            v.TagHtml = Factory.o51TagBL.GetTagging("j02", v.pid).TagHtml;

            var mq = new BO.myQuery("a35");
            mq.j02id = v.pid;
            mq.global_d1 = new DateTime(y, m, 1);
            mq.global_d2 = Convert.ToDateTime(mq.global_d1).AddMonths(1).AddDays(-1);
            v.lisA35 = Factory.a35PersonEventPlanBL.GetList(mq);
            if (v.lisA35.Count() > 0)
            {
                mq = new BO.myQuery("a01");
                mq.pids = v.lisA35.Select(p => p.a01ID).Distinct().ToList();
                mq.explicit_orderby = "a.a01DateFrom";
                v.lisA01 = Factory.a01EventBL.GetList(mq);
            }
            

            return View(v);
        }
        public IActionResult Info(int pid)
        {
            var v = new j02RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(v.pid);
                if (v.Rec != null)
                {
                    v.TagHtml = Factory.o51TagBL.GetTagging("j02", v.pid).TagHtml;
                    if (v.Rec.a04ID > 0)
                    {
                        v.RecA04 = Factory.a04InspectorateBL.Load(v.Rec.a04ID);
                    }
                    if (v.Rec.j03ID > 0)
                    {
                        v.RecJ03 = Factory.j03UserBL.Load(v.Rec.j03ID);
                    }

                    var mq = new BO.myQuery("a39InstitutionPerson");
                    mq.IsRecordValid = true;
                    mq.j02id = v.pid;
                    v.LisA39 = Factory.a39InstitutionPersonBL.GetList(mq).ToList();
                    var tg = Factory.o51TagBL.GetTagging("j02", v.pid);
                    v.Rec.TagHtml = tg.TagHtml;
                }
            }
            return View(v);
        }
        public IActionResult RecPage(int pid, string login)
        {
            var v = new j02RecPage();
            v.NavTabs = new List<NavTab>();
            if (pid == 0 && String.IsNullOrEmpty(login) == false)
            {
                var recJ03 = Factory.j03UserBL.LoadByLogin(login,0);
                if (recJ03 != null && recJ03.j02ID > 0)
                {
                    pid = recJ03.j02ID;
                }
            }
            v.pid = pid;
            if (v.pid == 0)
            {
                v.pid = Factory.CBL.LoadUserParamInt("j02-RecPage-pid");
            }
            if (v.pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(v.pid);
                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {
                    v.TagHtml = Factory.o51TagBL.GetTagging("j02", v.pid).TagHtml;
                    if (pid > 0)
                    {
                        Factory.CBL.SetUserParam("j02-RecPage-pid", pid.ToString());
                    }
                    if (v.Rec.a04ID > 0)
                    {
                        v.RecA04 = Factory.a04InspectorateBL.Load(v.Rec.a04ID);
                    }
                    if (v.Rec.j03ID > 0)
                    {
                        v.RecJ03 = Factory.j03UserBL.Load(v.Rec.j03ID);
                    }
                   

                    var mq = new BO.myQuery("a39InstitutionPerson");
                    mq.IsRecordValid = true;
                    mq.j02id = v.pid;
                    v.LisA39 = Factory.a39InstitutionPersonBL.GetList(mq).ToList();

                    RefreshNavTabs(v);
                    var tg = Factory.o51TagBL.GetTagging("j02", v.pid);
                    v.Rec.TagHtml = tg.TagHtml;
                }

            }

            if (v.pid == 0)
            {
                v.Rec = new BO.j02Person();
            }

            return View(v);

        }

        private void RefreshNavTabs(j02RecPage v)
        {
            v.NavTabs.Add(AddTab(string.Format(Factory.tra("{0}: je účastníkem"), Factory.App.Terminology_Akce), "a01Event", "/TheGrid/SlaveView?prefix=a01",false));
            v.NavTabs.Add(AddTab( string.Format(Factory.tra("{0}: je zakladatelem"), Factory.App.Terminology_Akce),"a01Event", "/TheGrid/SlaveView?prefix=a01&master_flag=issuer",false));
            v.NavTabs.Add(AddTab(string.Format(Factory.tra("{0}: je vedoucím"), Factory.App.Terminology_Akce), "a01Event", "/TheGrid/SlaveView?prefix=a01&master_flag=leader",false));

            v.NavTabs.Add(AddTab("Instituce", "a03Institution", "/TheGrid/SlaveView?prefix=a03"));
            v.NavTabs.Add(AddTab("Úkoly/Lhůty", "h04ToDo", "/TheGrid/SlaveView?prefix=h04"));
            v.NavTabs.Add(AddTab("Outbox", "x40MailQueue", "/TheGrid/SlaveView?prefix=x40"));
            v.NavTabs.Add(AddTab("PING Log", "j92PingLog", "/TheGrid/SlaveView?prefix=j92"));
            v.NavTabs.Add(AddTab("LOGIN Log", "j90LoginAccessLog", "/TheGrid/SlaveView?prefix=j90"));


            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-j02");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=j02Person&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }


        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j02Record() { rec_pid = pid, rec_entity = "j02" };
            v.Rec = new BO.j02Person();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("j02", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

                var mq = new BO.myQuery("a39");
                mq.j02id = v.rec_pid;
                var lisA39 = Factory.a39InstitutionPersonBL.GetList(mq).Where(p=>p.a39RelationFlag==BO.a39InstitutionPerson.a39RelationFlagEnum.Employee);
                if (lisA39.Count() > 0)
                {
                    v.a03ID_Employer = lisA39.First().a03ID;
                    v.EmployerName= lisA39.First().a03Name;
                }

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.uzivatel_er);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j02Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j02Person c = new BO.j02Person();
                if (v.rec_pid > 0) c = Factory.j02PersonBL.Load(v.rec_pid);
                c.j02TitleBeforeName = v.Rec.j02TitleBeforeName;
                c.j02FirstName = v.Rec.j02FirstName;
                c.j02LastName = v.Rec.j02LastName;
                c.j02TitleAfterName = v.Rec.j02TitleAfterName;

                c.j02IsInvitedPerson = v.Rec.j02IsInvitedPerson;

                c.j02PID = v.Rec.j02PID;
                c.j02Email = v.Rec.j02Email;
                c.j02Phone = v.Rec.j02Phone;
                c.j02Mobile = v.Rec.j02Mobile;
                c.j02Address = v.Rec.j02Address;
                c.j02Position = v.Rec.j02Position;
               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j02PersonBL.Save(c);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("j02", c.pid, v.TagPids);

                    var mq = new BO.myQuery("a39");
                    mq.j02id = c.pid;
                    var lisA39 = Factory.a39InstitutionPersonBL.GetList(mq).Where(p => p.a39RelationFlag == BO.a39InstitutionPerson.a39RelationFlagEnum.Employee);
                    if (v.a03ID_Employer > 0)
                    {
                        var recA39 = new BO.a39InstitutionPerson() { a39RelationFlag = BO.a39InstitutionPerson.a39RelationFlagEnum.Employee,j02ID=c.pid };
                        if (lisA39.Count() > 0 && v.a03ID_Employer > 0)
                        {
                            recA39 = lisA39.First();                            
                        }                        
                        recA39.a03ID = v.a03ID_Employer;
                        Factory.a39InstitutionPersonBL.Save(recA39);
                    }
                    else
                    {
                        if (lisA39.Count() > 0)
                        {
                            Factory.CBL.DeleteRecord("a39InstitutionPerson", lisA39.First().pid);
                        }
                    }
                    
                    

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }
    }
}