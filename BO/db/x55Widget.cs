using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    
    public enum x55DataTablesBtns
    {
        None=0,
        Export=1,
        ExportPrint=2,
        ExportPrintPdf=3
    }
    public class x55Widget : BaseBO
    {
        [Key]
        public int x55ID { get; set; }
       
        public string x55Name { get; set; }
        public string x55Code { get; set; }
        public int x55Ordinal { get; set; }
        public string x55Content { get; set; }
        public string x55Description { get; set; }
        public string x55TableSql { get; set; }
        public string x55TableColHeaders { get; set; }
        public string x55TableColTypes { get; set; }
        public string x55Image { get; set; }
        public string x55BoxBackColor { get; set; }
        public string x55HeaderBackColor { get; set; }
        public string x55HeaderForeColor { get; set; }
        public int x55BoxMaxHeight { get; set; }        
        public int x55DataTablesLimit { get; set; }
        public x55DataTablesBtns x55DataTablesButtons { get; set; }
        public string x55Help { get; set; }
        public bool IsUseDatatables { get; set; }   //není db pole - naplní ho incializátor widgetů na stránce
        public string x55Skin { get; set; }

        public string CssHeaderDiv { get
            {
                if (this.x55HeaderBackColor==null && this.x55HeaderForeColor == null)
                {
                    return null;
                }
                string s = "";
                if (this.x55HeaderBackColor != null)
                {
                    s += "background-color:" + this.x55HeaderBackColor + ";";
                }
                if (this.x55HeaderForeColor != null)
                {
                    s += "color:" + this.x55HeaderForeColor+";";
                }
                return "style='"+s+"'";
            }
        }
        public string CssContentDiv { get
            {
                if (this.x55BoxMaxHeight <= 0)
                {
                    this.x55BoxMaxHeight = 400;
                }
                string s = "max-height: " + this.x55BoxMaxHeight.ToString() + "px;";
                if (this.x55BoxBackColor != null)
                {
                    s += "background-color:" + this.x55BoxBackColor + ";";
                }
                return "style='" + s + "'";
            }
        }
        public string HeaderImage { get
            {
                if (this.x55Image == null)
                {
                    return "/images/widget.png";
                }
                else
                {
                    return "/images/"+this.x55Image;
                }
            } 
        }
    }
}
