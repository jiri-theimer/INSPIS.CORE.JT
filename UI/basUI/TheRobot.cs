using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using Microsoft.AspNetCore.Mvc;
using BL;
using DocumentFormat.OpenXml.Drawing.Charts;

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

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            string strLogin = _gp.LoadParam("SysUser_Login");
            if (string.IsNullOrEmpty(strLogin) == true)
            {
                LogInfo("SysUser_Login in x35GlobalParam missing.");
                return;
            }
            
            BO.RunningUser ru = new BO.RunningUser() { j03Login = strLogin };
            BL.Factory f = new BL.Factory(ru, _app, null, _gp,_tt);
            if (f.CurrentUser == null)
            {
                LogInfo("f.CurrentUser is null.");
                return;
            }
            
            LogInfo(string.Format("Timed Hosted Service is working by user:{0}. Count: {1}",f.CurrentUser.j03Login, count));



            //čekající pošta na odeslání
            Handle_MailQueue_INEZ(f);
            Handle_MailQueue(f);
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

        private void LogInfo(string strMessage)
        {
            var strPath = string.Format("{0}\\the-robot-{1}.log", _app.LogFolder, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", DateTime.Now.ToString()+": ", strMessage });
        }
    }
}
