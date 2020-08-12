using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("myradiolist")]
    public class myRadioListHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";  //jedna hodnota - string

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("valuefield")]
        public string ValueField { get; set; }  //musí být integer
        [HtmlAttributeName("textfield")]
        public string TextField { get; set; }

        [HtmlAttributeName("event_after_changevalue")]
        public string Event_After_ChangeValue { get; set; }


        [HtmlAttributeName("datasource")]
        public ModelExpression DataSource { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IEnumerable lisDatasource = this.DataSource.Model as IEnumerable;
            if (lisDatasource == null)
            {
                return;
            }
            
            string strModelValue = this.For.Model.ToString() as string;
            
            var sb = new System.Text.StringBuilder();
            

            sb.AppendLine("<ul style='list-style:none;padding-left:0px;'>");
            foreach (var item in lisDatasource)
            {

                string strText = DataSource.Metadata.ElementMetadata.Properties[this.TextField].PropertyGetter(item).ToString();                
                string strValue = Convert.ToInt32(DataSource.Metadata.ElementMetadata.Properties[this.ValueField].PropertyGetter(item)).ToString();
                string strChecked = "";

                if (strModelValue == strValue)
                {
                    strChecked = "checked";
                }
               


                sb.AppendLine("<li>");
                sb.Append(string.Format("<input type='radio' id='chk{0}_{1}' name='my{0}' onclick='myradiolist_checked(\"{0}\",\"{1}\",\"{3}\")' {2} />", this.For.Name, strValue, strChecked,this.Event_After_ChangeValue));
                sb.Append(string.Format("<label  for='chk{0}_{1}'>{2}</label>", this.For.Name, strValue, strText));
                

                sb.AppendLine("</li>");
                
                
            }
            sb.AppendLine("</ul>");

            sb.Append(string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", this.For.Name, strModelValue));

            output.Content.AppendHtml(sb.ToString());



        }
    
    }
}
