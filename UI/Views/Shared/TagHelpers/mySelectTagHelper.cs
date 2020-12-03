using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using System.Collections;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("myselect")]
    public class mySelectTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("entity")]
        public string Entity { get; set; }

        [HtmlAttributeName("textfield")]
        public string TextField { get; set; }

        [HtmlAttributeName("firstemptyrowtext")]
        public string FirstEmptyRowText { get; set; }
        [HtmlAttributeName("firstemptyrowvalue")]
        public string FirstEmptyRowValue { get; set; }

        [HtmlAttributeName("event_after_changevalue")]
        public string Event_After_ChangeValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var sb = new System.Text.StringBuilder();

            var strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");

            sb.AppendLine(string.Format("<select class='form-control' id='{0}' name='{1}' onfocus=\"myselect_focus(event,this,'{2}','{3}')\"", strControlID, this.For.Name, this.Entity,this.TextField));
            if (this.Event_After_ChangeValue != null)
            {
                sb.Append(string.Format(" onchange='{0}(this)'", this.Event_After_ChangeValue));
            }
            sb.Append(">");

            if (this.FirstEmptyRowText != null)
            {
                sb.Append(string.Format("<option value='{0}'>{1}</option>", this.FirstEmptyRowValue, this.FirstEmptyRowText));
            }


            sb.Append("</select>");

            output.Content.AppendHtml(sb.ToString());
        }
    }
}
