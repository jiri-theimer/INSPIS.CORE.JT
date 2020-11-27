using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using Microsoft.AspNetCore.Mvc;
using BL;
using Microsoft.VisualBasic;

namespace UI
{
    public class TheRobot : IHostedService, IDisposable
    {
        private int executionCount = 0;        
        private Timer _timer;
        private readonly  BL.RunningApp _app;
        private readonly BL.TheGlobalParams _gp;
        private readonly BL.TheTranslator _tt;

        public TheRobot(BL.RunningApp app,BL.TheGlobalParams gp,BL.TheTranslator tt)
        {            
            _app = app;
            _gp = gp;
            _tt = tt;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            LogInfo("Timed Hosted Service running.");
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(300));   //každých 300 sekund
            
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            LogInfo("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }


        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            
            if (string.IsNullOrEmpty(_app.RobotUser) == true)
            {
                LogInfo("[RobotUser] of appsettings is missing!");
                return;
            }
            
            BO.RunningUser ru = new BO.RunningUser() { j03Login = _app.RobotUser };
            BL.Factory f = new BL.Factory(ru, _app, null, _gp,_tt);
            if (f.CurrentUser == null)
            {
                LogInfo("f.CurrentUser is null. SysUser_Login: "+ _app.RobotUser);
                return;
            }
            
            LogInfo(string.Format("Timed Hosted Service is working by user:{0}. Count: {1}",f.CurrentUser.j03Login, count));

            Handle_h04_Reminder(f); //odesílač připomenutí úkolů a lhůt
            Handle_AutoWorkflow(f); //robotické workflow

            //čekající pošta na odeslání
            Handle_MailQueue_INEZ(f);
            Handle_MailQueue(f);
            
        }

        private void Handle_h04_Reminder(BL.Factory f)
        {            
            var lisH04 = f.h04ToDoBL.GetList_ReminderWaiting();            
            foreach(var rec in lisH04)
            {                
                var recB65 = new BO.b65WorkflowMessage() { b65MessageSubject = "Připomenutí | " + rec.h07Name + ": " + rec.a01Signature };
                recB65.b65MessageBody = "Dobrý den," + System.Environment.NewLine + "posíláme Vám připomenutí k:" + System.Environment.NewLine+ rec.h07Name + ": " + rec.h04Name + System.Environment.NewLine + System.Environment.NewLine + rec.h04Description;
                var intB65ID = f.h07ToDoTypeBL.Load(rec.h07ID).b65ID;
                if (intB65ID > 0)
                {
                    recB65 = f.b65WorkflowMessageBL.MailMergeRecord(intB65ID, rec.pid, null);
                }
                

                recB65.b65MessageBody += System.Environment.NewLine +System.Environment.NewLine+"------------------------"+System.Environment.NewLine + _app.UserUrl + "/h04/RecPage?pid=" + rec.pid.ToString();
                
                var c = new BO.x40MailQueue() {x29ID = 604,x40DataPID=rec.pid, x40IsAutoNotification=true,x40Subject=recB65.b65MessageSubject,x40Body=recB65.b65MessageBody };
                var mq = new BO.myQuery("j02") { h04id = rec.pid, IsRecordValid = true };
                c.x40Recipient = string.Join(",", f.j02PersonBL.GetList(mq).Select(p => p.j02Email));

                f.MailBL.SendMessage(c, false);
            }
        }
        private void Handle_AutoWorkflow(BL.Factory f)
        {            
            var mq = new BO.myQuery("b06") {IsRecordValid=true };
            var lisB06 = f.b06WorkflowStepBL.GetList(mq).Where(p=>p.b06ValidateAutoMoveSQL != null);    //testování automatiky podle návratové podmínky kroku přes b06ValidateAutoMoveSQL
            foreach (var recB06 in lisB06)
            {
                //LogInfo("Test workflow kroku (b06ValidateAutoMoveSQL): " + recB06.b06Name);
                mq = new BO.myQuery("a01") { b02id = recB06.b02ID,a01IsClosed=false };
                var lisA01 = f.a01EventBL.GetList(mq).Where(p => p.a01ParentID == 0);
                foreach(var recA01 in lisA01)
                {
                    LogInfo("Test akce pro automaticky spouštěný krok (b06ValidateAutoMoveSQL): " + recA01.a01Signature);

                    if (f.WorkflowBL.RunAutoWorkflowStepByRawSql(recA01, recB06))
                    {
                        //krok byl spuštěn
                        LogInfo("Automaticky spuštěn krok: " + recB06.b06Name + " u akce: " + recA01.a01Signature);
                    }
                    else
                    {
                        LogInfo("Nedošlo ke spuštění kroku: " + recB06.b06Name + " u akce: " + recA01.a01Signature);
                    }
                }
            }

            mq = new BO.myQuery("b06") { IsRecordValid = true };
            lisB06 = f.b06WorkflowStepBL.GetList(mq).Where(p => p.b06IsAutoRun_Missing_Form==true || p.b06IsAutoRun_Missing_Attachment==true);    //testování automatiky podle vyplňovaných formulářů
            foreach (var recB06 in lisB06)
            {
                LogInfo("Test workflow kroku (b06IsAutoRun_Missing_Form/b06IsAutoRun_Missing_Attachment): " + recB06.b06Name);
                mq = new BO.myQuery("a01") { b02id = recB06.b02ID,a01IsClosed=false };
                var lisA01 = f.a01EventBL.GetList(mq);
                foreach (var recA01 in lisA01)
                {
                    LogInfo("Test akce pro automaticky spouštěný krok (b06IsAutoRun_Missing_Form/b06IsAutoRun_Missing_Attachment): " + recA01.a01Signature);
                    mq = new BO.myQuery("b05") { a01id = recA01.pid };
                    var lisB05 = f.b05Workflow_HistoryBL.GetList(mq);
                    if (recB06.b06IsAutoRun_Missing_Attachment && recA01.a01ParentID == 0)
                    {
                        //testovat, zda spustit krok, pokud k akci nejsou přiloženy povinné přílohy - netestovat v podřízených akcích
                        if (lisB05.Where(p => p.b06ID == recB06.pid).Count() == 0)
                        {
                            //v historii ještě nebyl automatický krok spuštěn
                            var lisA14 = f.a08ThemeBL.GetListA14(recA01.a08ID).Where(p => p.a14IsRequired == true);
                            foreach(var recA14 in lisA14)
                            {
                                var mqx = new BO.myQuery("o27") { a01id = recA01.pid, o13id = recA14.o13ID };
                                if (f.o27AttachmentBL.GetList(mqx, null).Count() == 0)
                                {
                                    //dokument neexistuje -> krok spustit
                                    f.WorkflowBL.RunWorkflowStep(recA01, recB06, null, "Automaticky spuštěný krok systémem", null, false);
                                    break;
                                }
                                
                            }
                        }
                    }
                    if (recB06.b06IsAutoRun_Missing_Form && lisB05.Where(p=>p.b06ID==recB06.pid).Count()==0)
                    {
                        //testovat, zda spustit krok, pokud v akci nejsou vyplněné povinné formuláře && v historii ještě nebyl automatický krok spuštěn
                        var mqx = new BO.myQuery("a11") { a01id = recA01.pid };
                        var lisA11 = f.a11EventFormBL.GetList(mqx);
                        foreach(var recA11 in lisA11)
                        {
                            var cValidate = new FormValidation(f);
                            var lisErrs = cValidate.GetValidateResult(recA11.pid);
                            if (lisErrs.Count() > 0)
                            {
                                
                                //minimálně jedna povinná otázka není vyplněna
                                f.WorkflowBL.RunWorkflowStep(recA01, recB06, null, "Automaticky spuštěný krok systémem: ["+ lisErrs.First().f06Name+"] ["+lisErrs.First().Otazka+"] "+ lisErrs.First().Message, null, false);
                                break;
                            }
                           
                        }
                    }
                }
            }

        }
        private void Handle_MailQueue(BL.Factory f)
        {
            var mq = new BO.myQuery("x40");
           
            var lisX40 = f.MailBL.GetList(mq).Where(p => p.x40BatchGuid != null && p.x40Status == BO.x40StateFlag.InQueque && (p.x40BatchGuid.Substring(0,4)=="REAL" || p.x40BatchGuid.Substring(0, 4) == "TEST")).Take(20);
            if (lisX40.Count() > 0)
            {
                foreach (var recX40 in lisX40)
                {
                    if (recX40.x40BatchGuid.Substring(0, 4) == "TEST")
                    {
                        f.MailBL.SendMessage(recX40, true);  //testovací režim
                    }
                    else
                    {
                        f.MailBL.SendMessage(recX40, false);
                    }
                }

            }
        }

        private void Handle_MailQueue_INEZ(BL.Factory f)
        {
            var mq = new BO.myQuery("a42");
            var lisA42= f.a42QesBL.GetList(mq).Where(p => p.a42JobGuid != null && (p.a42JobState == BO.a42JobState.PreparedX40 || p.a42JobState == BO.a42JobState.MailQueue));
            if (lisA42.Count()>0)
            {
                var recA42 = lisA42.First();
                //INEZ dávka s poštovními zprávy ve frontě
                mq = new BO.myQuery("x40") { explicit_orderby = "a.x40ID DESC", explicit_sqlwhere = "a.x40BatchGuid='" + BO.BAS.GSS(recA42.a42JobGuid)+"'" };
                var lisX40 = f.MailBL.GetList(mq).Where(p => p.x40Status == BO.x40StateFlag.InQueque).Take(20); //odeslat maximálně 20 zpráv
                if (lisX40.Count() > 0)
                {
                    foreach (var recX40 in lisX40)
                    {
                        if (recA42.a42TestFlag == 1)
                        {
                            f.MailBL.SendMessage(recX40,true);  //testovací režim
                        }
                        else
                        {
                            f.MailBL.SendMessage(recX40, false);
                        }
                        
                    }
                    return;
                }
                
            }                                               
        }

        
        private void LogInfo(string strMessage)
        {
            var strPath = string.Format("{0}\\the-robot-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString() + ": ", strMessage });
        }


    }
}
