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
            wr(""); wr("");
            wr($"<div id='myChart{_guid}' style='max-width:100%; height:400px'></div>"); wr("");
            wr("<script type='text/javascript'>"); wr("");


            wr("google.charts.load('current', {'packages':['corechart']});");
            wr($"google.charts.setOnLoadCallback(drawChart{_guid});"); wr("");


            wr("function drawChart" + _guid + "() {");
            wr("var data = google.visualization.arrayToDataTable(");
            wr("[");

            wr("['" + string.Join("','", _headers) + "']");

            int x = 0;
            
            foreach (System.Data.DataRow dbRow in dt.Rows)
            {
                _sb.Append(",[");

                for (int i = 0; i <= _headers.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        _sb.Append(",");
                    }
                    

                    if (i==0)
                    {
                        _sb.Append("'" + dbRow[i] + "'");
                    }
                    else
                    {
                        if (dbRow[i] == null)
                        {
                            _sb.Append("0");
                        }
                        else
                        {
                            _sb.Append(dbRow[i].ToString().Replace(",","."));
                        }
                        
                    }

                    
                }
                wr("]");
                x += 1;
            }


            wr("]");
            wr(");");
            wr("");
            //wr("var options = {title:'Nadpis grafu'};");
            wr("var options = {legend: { position: 'top'}};");
            wr("");
            wr($"var chart = new google.visualization.{strChartType}Chart(document.getElementById('myChart{_guid}'));");
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
