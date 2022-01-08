using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BO.CLS
{
    public class Datatable2Chart
    {
        private System.Text.StringBuilder _sb;
        private List<string> _headers;
        private string _guid { get; set; }

        public Datatable2Chart()
        {
            _sb = new System.Text.StringBuilder();
            _guid = BO.BAS.GetGuid();
        }
        public string CreateGoogleChartHtml(System.Data.DataTable dt, string strChartType, string strHeaders)
        {

            _headers = BO.BAS.ConvertString2List(strHeaders, "|");

            wr($"<div id='myChart{_guid}' style='max-width:100%; height:400px'></div>");wr("");
            wr("<script type='text/javascript'>"); wr("");


            wr("google.charts.load('current', {'packages':['corechart']});");
            wr($"google.charts.setOnLoadCallback(drawChart{_guid});");


            wr("function drawChart"+_guid+"() {");
            wr("var data = google.visualization.arrayToDataTable(");
            wr("[");

            wr("]");
            wr(");");
            wr("");
            wr("var options = {title:'Nadpis grafu'};");
            wr("");
            wr($"var chart = new google.visualization.BarChart(document.getElementById('myChart{_guid}'));");
            wr("chart.draw(data, options);");
            wr("}");


            wr("");
            wr("</script>");

            return _sb.ToString();
        }

        private void wr(string s)
        {
            _sb.AppendLine(s);
            
            
        }
    }

    
    
   
}
