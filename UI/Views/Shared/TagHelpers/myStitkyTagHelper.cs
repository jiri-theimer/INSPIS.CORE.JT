using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("mystitky")]
    public class myStitkyTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("entity")]
        public string Entity { get; set; }

        [HtmlAttributeName("buttontext")]
        public string ButtonText { get; set; } = "Zatřídit do kategorií";

        [HtmlAttributeName("tagnames")]
        public string SelectedTagNames { get; set; }

        [HtmlAttributeName("taghtml")]
        public string SelectedTagHtml { get; set; }

        private System.Text.StringBuilder _sb;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            _sb = new System.Text.StringBuilder();
            string strSelectedValues = this.For.Model as string;

            
            
            _sb.AppendLine(string.Format("<input type='hidden' id='TagPids' name='TagPids' value='{0}' />", strSelectedValues));
           
            _sb.AppendLine("<div class='form-group'>");
            _sb.AppendLine(string.Format("<button id='cmdTagging' type='button' class='btn btn-primary' onclick='mystitky_multiselect(event,\"{0}\")'>{1} ★</button>", this.Entity,this.ButtonText));
            _sb.AppendLine("</div>");

            _sb.AppendLine(string.Format("<input type='hidden' id='TagHtml' name='TagHtml' value=\"{0}\" />", this.SelectedTagHtml));           

            _sb.Append("<div id='divTagHtml'>");
            if (string.IsNullOrEmpty(this.SelectedTagHtml) == false)
            {
                _sb.Append(this.SelectedTagHtml);     
            }
            _sb.Append("</div>");

            output.Content.AppendHtml(_sb.ToString());
        }
    }
}
