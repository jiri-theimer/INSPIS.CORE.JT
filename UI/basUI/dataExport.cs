using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ClosedXML.Excel;

namespace UI
{
    public class dataExport
    {
        public bool ToXLSX(System.Data.DataTable dt, string strFilePath, BO.myQuery mq)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Grid");
                int row = 1;
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

                //worksheet.Cell("A1").Value = "Hello World!";
                //worksheet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";
                workbook.SaveAs(strFilePath);
               
            }

            return true;
        }
        public bool ToCSV(System.Data.DataTable dt, string strFilePath, BO.myQuery mq)
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
