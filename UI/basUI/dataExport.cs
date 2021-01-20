using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SQLitePCL;

namespace UI
{
    public class dataExport
    {
        public void StatZeroRow(string strFilePathExisting, IEnumerable<BO.f21ReplyUnitJoinedF19> lisColsHelp, BL.Factory f,int intFirstNonF19Cols)
        {
            using (var workbook = new XLWorkbook(strFilePathExisting))
            {
                var worksheet = workbook.Worksheets.First();
                
                int col = intFirstNonF19Cols+1;
                for(int x = col; x <= intFirstNonF19Cols + lisColsHelp.Count(); x++)
                {
                    int tryf19id = 0;
                    int tryf21id = 0;
                    if (worksheet.Cell(2, x).Value.ToString().Contains("-"))
                    {
                        //hledání f19ID+f21ID
                        var lis = BO.BAS.ConvertString2ListInt(worksheet.Cell(2, x).Value.ToString(), "-");
                        tryf19id = lis[0];
                        tryf21id = lis[1];
                    }
                    else
                    {
                        //hledání pouze f19ID
                        tryf19id = BO.BAS.InInt(worksheet.Cell(2, x).Value.ToString());
                    }
                    
                    if (tryf21id > 0)
                    {
                        if (tryf19id > 0 && lisColsHelp.Where(p => p.f19ID == tryf19id && p.f21ID == tryf21id).Count() > 0)
                        {
                            var qry = lisColsHelp.Where(p => p.f19ID == tryf19id && p.f21ID == tryf21id);
                            
                            if (qry.First().f19IsMultiselect)
                            {
                                worksheet.Cell(1, x).Value = qry.First().f19Name+ " - " + qry.First().f21Name;
                            }
                            else
                            {
                                worksheet.Cell(1, x).Value = qry.First().f19Name;
                            }
                            
                        }
                    }
                    else
                    {
                        if (tryf19id > 0 && lisColsHelp.Where(p => p.f19ID == tryf19id).Count() > 0)
                        {
                            worksheet.Cell(1, x).Value = lisColsHelp.Where(p => p.f19ID == tryf19id).First().f19Name;


                        }
                    }
                    
                }
                

                workbook.Save();
            }
        }
        public void StatVysvetlivky(string strFilePathExisting,IEnumerable<BO.f21ReplyUnitJoinedF19> lisColsHelp,BL.Factory f)
        {
            //strFilePathExisting: xls soubor, do které se přidá sešit Vysvětlivky
            using (var workbook = new XLWorkbook(strFilePathExisting))
            {
                var worksheet = workbook.Worksheets.Add(f.tra("Vysvětlivky"));
               
                int row = 1;
                int lastF18ID = 0;
                int lastF06ID = 0;
               
                foreach (var c in lisColsHelp)
                {
                    if (c.f06ID != lastF06ID)
                    {
                        worksheet.Cell(row, 1).Value = c.f06Name;
                        row += 1;
                        worksheet.Cell(row, 1).Value = f.tra("Segment");
                        worksheet.Cell(row, 2).Value = f.tra("Otázka");
                        worksheet.Cell(row, 5).Value = f.tra("Jednotka odpovědi");
                        worksheet.Cell(row, 8).Value = f.tra("Baterie");                        
                        row += 1;
                        worksheet.Cell(row, 2).Value = "PID";
                        worksheet.Cell(row, 3).Value = f.tra("STAT ID");
                        worksheet.Cell(row, 4).Value = f.tra("Název");
                        worksheet.Cell(row, 5).Value = "PID";
                        worksheet.Cell(row, 6).Value = f.tra("STAT ID");
                        worksheet.Cell(row, 7).Value = f.tra("Název");
                        
                        
                        row += 1;

                    }
                    if (lastF18ID != c.f18ID)
                    {
                        worksheet.Cell(row, 1).Value = c.f18Name;
                    }
                    worksheet.Cell(row, 2).Value = c.f19ID;
                    worksheet.Cell(row, 3).Value = c.f19StatID;
                    worksheet.Cell(row, 4).Value = c.f19Name;
                    if (c.f23ID == 2 || c.f23ID == 3 || c.f23ID == 4 || c.f23ID == 5)
                    {
                        worksheet.Cell(row, 5).Value = c.pid;
                        worksheet.Cell(row, 6).Value = c.f21ExportValue;
                        worksheet.Cell(row, 7).Value = c.f21Name;
                    }
                    worksheet.Cell(row, 8).Value = c.f26Name;
                    if (c.f21IsCommentAllowed)
                    {
                        row += 1;
                        worksheet.Cell(row, 2).Value = c.f19ID;
                        worksheet.Cell(row, 5).Value = c.pid;
                        worksheet.Cell(row, 4).Value = f.tra("Komentář k jednotce odpovědi");
                        worksheet.Cell(row, 8).Value = c.f26Name;
                    }
                    //worksheet.Cell(row, 3).Style.Font.Bold = true;
                    row += 1;
                    lastF18ID = c.f18ID; lastF06ID = c.f06ID;
                }
                worksheet.Columns(1, 8).AdjustToContents();


                workbook.Save();

            }
        }
        public bool ToXLSX(System.Data.DataTable dt, string strFilePath, BO.baseQuery mq,int firstrow=1)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Grid");
                int row = firstrow;
                int col = 1;

                foreach (var c in mq.explicit_columns)
                {
                    worksheet.Cell(row, col).Value = c.Header;
                    worksheet.Cell(row, col).Style.Font.Bold = true;

                    col += 1;
                }
                row += 1;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    col = 1;
                    foreach (var c in mq.explicit_columns)
                    {

                        if (!Convert.IsDBNull(dr[c.UniqueName]))
                        {
                            worksheet.Cell(row, col).Value = dr[c.UniqueName];

                        }
                        col += 1;
                    }

                    row += 1;

                }
                worksheet.Columns(1,col).AdjustToContents();
                

                workbook.SaveAs(strFilePath);
               
            }

            return true;
        }
        public bool ToXLSX(System.Data.DataTable dt, string strFilePath, List<BO.StringPair> cols, int firstrow = 1)
        {
            //key: název pole, value: záhlaví sloupce
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Grid");
                int row = firstrow;
                int col = 1;                

                foreach (var c in cols)
                {
                    worksheet.Cell(row, col).Value = c.Value;
                    worksheet.Cell(row, col).Style.Font.Bold = true;

                    col += 1;
                }
                var coltypes = new List<StringPair>();
                foreach(System.Data.DataColumn c in dt.Columns)
                {
                    coltypes.Add(new BO.StringPair() { Key = c.ColumnName, Value = c.DataType.Name });                    
                }
                //worksheet.Column(1).CellsUsed().SetDataType(XLDataType.Text);
                row += 1;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    col = 1;
                    foreach (var c in cols)
                    {
                        
                        if (!Convert.IsDBNull(dr[c.Key]))
                        {
                            worksheet.Cell(row, col).Value = dr[c.Key];
                            
                            if (c.Key.ToUpper().Substring(0, 3) != "COL")    //vynechat v exportu statistiky sloupce colXXXX, které jsou fyzicky stringy!
                            {
                                switch (coltypes.Where(p => p.Key == c.Key).First().Value)
                                {
                                    case "Boolean":
                                        worksheet.Cell(row, col).DataType = XLDataType.Boolean;
                                        break;
                                    case "String":
                                        worksheet.Cell(row, col).DataType = XLDataType.Text;                                        
                                        break;
                                    case "DateTime":
                                        worksheet.Cell(row, col).DataType = XLDataType.DateTime;
                                        break;
                                }
                            }

                        }
                        col += 1;
                    }

                    row += 1;

                }
                worksheet.Columns(1, col).AdjustToContents();
                workbook.SaveAs(strFilePath);

            }

            return true;
        }
        public bool ToCSV(System.Data.DataTable dt, string strFilePath, BO.baseQuery mq)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
            //headers  
            foreach (var col in mq.explicit_columns)
            {
                sw.Write("\"" + col.Header + "\"");
                sw.Write(";");
            }

            sw.Write(sw.NewLine);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                foreach (var col in mq.explicit_columns)
                {
                    string value = "";

                    if (!Convert.IsDBNull(dr[col.UniqueName]))
                    {
                        value = dr[col.UniqueName].ToString();
                        if (col.FieldType == "string")
                        {
                            value = "\"" + value + "\"";
                        }
                    }
                    sw.Write(value);

                    sw.Write(";");


                }

                sw.Write(sw.NewLine);

            }
            sw.Close();

            return true;
        }
    }
}
