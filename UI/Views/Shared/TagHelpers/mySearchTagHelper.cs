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
    [HtmlTargetElement("mysearch")]
    public class mySearchTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("entity")]
        public string Entity { get; set; }

        [HtmlAttributeName("placeholder")]
        public string PlaceHolder { get; set; }


        private System.Text.StringBuilder _sb;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            _sb = new System.Text.StringBuilder();

            var strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");

            sb(string.Format("<div id='divDropdownContainer{0}' class='dropdown input-group' style='border-radius:3px;width:100%;'>", strControlID));
            
            sb(string.Format("<input type='text' id='{0}' name='{1}' class='form-control' placeholder='{2}'/>", strControlID, this.For.Name, this.PlaceHolder));            
            sb(string.Format("<button type='button' id='cmdCombo{0}' class='btn btn-light dropdown-toggle cmdDropdown' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></button>", strControlID));


            sb(string.Format("<div id='divDropdown{0}' class='dropdown-menu' aria-labelledby='cmdCombo{0}' style='width:100%;background-color:aliceblue;' tabindex='-1'>", strControlID));
            sb(string.Format("<div id='divData{0}' style='height:220px;overflow:auto;width:100%;min-width:200px;'>", strControlID));
            sb("</div>");           
            sb("</div>");
            sb("");
            sb("");
            sb("</div>");


            sb("<script type='text/javascript'>");
            sb("");
            _sb.Append(string.Format("var c{0}=", strControlID));
            _sb.Append("{");
            _sb.Append(string.Format("controlid: '{0}',posturl: '/TheCombo/GetHtml4Search',entity:'{1}'", strControlID, this.Entity));
            _sb.Append("};");

            sb("");
            sb(string.Format("mysearch_init(c{0});", strControlID));
            sb("");
            sb("</script>");

            output.Content.AppendHtml(_sb.ToString());
        }



        private void sb(string s)
        {
            _sb.AppendLine(s);
        }
    }



    
}
