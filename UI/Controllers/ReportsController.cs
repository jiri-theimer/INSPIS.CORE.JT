using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Telerik.Reporting;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using System.Collections.Generic;
using ClosedXML;
using Telerik.Reporting.Services.Engine;

namespace UI.Controllers
{
    [Route("api/reports")]
    public class ReportsController : ReportsControllerBase
    {


        private readonly BL.Factory _f;

        public ReportsController(IReportServiceConfiguration reportServiceConfiguration, BL.Factory f) : base(reportServiceConfiguration)
        {
            _f = f;

            var resolver = new UriReportSourceResolver(_f.App.ReportFolder)
            .AddFallbackResolver(new TypeReportSourceResolver()
                .AddFallbackResolver(new CustomReportSourceResolver()));


            reportServiceConfiguration.ReportSourceResolver = resolver;
        }



        

    }


    public class CustomReportSourceResolver : IReportSourceResolver
    {
        public Telerik.Reporting.ReportSource Resolve(string reportId, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
        {

            //this method should be fired, but nothing happens.
            string reportXml = "test";

            return new Telerik.Reporting.XmlReportSource { Xml = reportXml };
        }
    }


}