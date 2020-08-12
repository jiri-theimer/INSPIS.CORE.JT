using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o51Tag:BaseBO
    {
        [Key]
        public int o51ID { get; set; }
        public int o53ID { get; set; }
        public int j02ID_Owner { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string o51Name { get; set; }


        public bool o51IsColor { get; set; }
        public string o51BackColor { get; set; }
        public string o51ForeColor { get; set; }
        public int o51Ordinary { get; set; }




        public string o51Code { get; set; }

        public string o53Name { get; set; }
        public string o53Entities;
        public bool o53IsMultiSelect;
        public int o53Ordinary;

        public string HtmlText { get
            {
                if (this.o51IsColor == false)
                {
                    return string.Format("<div class='tagbox'>{0}</div>", this.o51Name);
                }
                else
                {
                    return string.Format("<div class='tagbox' style='background-color:{0};color:{1};'>{2}</div>",this.o51BackColor,this.o51ForeColor, this.o51Name);
                }

                
            }
        }
    }

    public class TaggingHelper
    {
        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
        public IEnumerable<BO.o51Tag> Tags { get; set; }
    }
}
