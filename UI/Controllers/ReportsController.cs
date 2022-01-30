using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Telerik.Reporting;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using System.Collections.Generic;
using Telerik.Reporting.Services.Engine;
using System.IO;
using BL;
using System;

namespace UI.Controllers
{    
    [Route("apix/reports")]    
    public class ReportsController : ReportsControllerBase
    {
        

        public ReportsController(IReportServiceConfiguration reportServiceConfiguration,BL.RunningApp app) : base(reportServiceConfiguration)
        {
            
        
            var resolver = new CustomReportSourceResolver(app);
            reportServiceConfiguration.Storage = new Telerik.Reporting.Cache.File.FileStorage(app.TempFolder);
            reportServiceConfiguration.HostAppId = "ReportViewer" + app.AppName.Length.ToString()+app.UserUrl.Length.ToString();    //HostAppId musí být unikátní v rámci všech websites na serveru


            reportServiceConfiguration.ReportSourceResolver = resolver;

            
            

        }



        

    }


    public class CustomReportSourceResolver : IReportSourceResolver
    {
        private BL.RunningApp _app;
        public CustomReportSourceResolver(BL.RunningApp app)
        {
            _app = app;
        }
        public Telerik.Reporting.ReportSource Resolve(string reportId, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
        {
            //soubor sestavy###login uživatele###j76id
            
            List<string> lis = BO.BAS.ConvertString2List(reportId, "###");
            reportId = lis[0];
            string strLogin = lis[1];
            int intJ76ID = 0;
            if (lis.Count > 2)
            {
                intJ76ID = BO.BAS.InInt(lis[2]);
            }

            
            
            string reportXml = File.ReadAllText(_app.ReportFolder + "\\" + reportId);
           

            if (reportXml.Contains("1=1"))
            {
                var cu = new BO.RunningUser() { j03Login = strLogin };
                BL.Factory f = new BL.Factory(cu, _app, null, null, null);
                if (intJ76ID > 0)
                {
                    var recJ76 = f.j76NamedQueryBL.Load(intJ76ID);                    
                    var mq = new BO.InitMyQuery().Load(recJ76.j76Entity);
                    mq.lisJ73_Named= f.j76NamedQueryBL.GetList_j73(intJ76ID, recJ76.j76Entity);
                    
                    DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql("", mq, cu);
                    //File.WriteAllText("c:\\temp\\hovado.txt", fq.SqlWhere);
                    string strFilterAlias = recJ76.j76Name;
                    var cc = new BL.bas.QuerySupport();
                    strFilterAlias = cc.getFiltrAlias(recJ76.j76Entity.Substring(0, 3), mq.lisJ73_Named, f);

                    reportXml = reportXml.Replace("1=1", fq.SqlWhere).Replace("#query_alias#", strFilterAlias);
                    
                }
                
            }


            

            return new Telerik.Reporting.XmlReportSource { Xml = reportXml };
        }
    }


}