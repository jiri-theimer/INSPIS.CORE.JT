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
    [HtmlTargetElement("mycombochecklist")]
    public class myComboCheckListTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("entity")]
        public string Entity { get; set; }

        [HtmlAttributeName("placeholder")]
        public string PlaceHolder { get; set; }
        [HtmlAttributeName("masterpid")]
        public int masterpid { get; set; }

        [HtmlAttributeName("masterprefix")]
        public string masterprefix { get; set; }

        [HtmlAttributeName("param1")]
        public string Param1 { get; set; }

        [HtmlAttributeName("selectedtext")]
        public ModelExpression SelectedText { get; set; }

        [HtmlAttributeName("dropdown-height")]
        public string dropdown_height { get; set; }

        [HtmlAttributeName("event_after_changevalue")]
        public string Event_After_ChangeValue { get; set; }


        private System.Text.StringBuilder _sb;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;


            _sb = new System.Text.StringBuilder();
            string strSelectedValues = this.For.Model as string;
            if (this.dropdown_height == null)
            {
                this.dropdown_height = "220px";
            }

            var strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");


            sb(string.Format("<div id='divDropdownContainer{0}' class='dropdown'>", strControlID));


            string strTitle = "";
            if (this.SelectedText.Model != null)
            {
                strTitle = Convert.ToString(this.SelectedText.Model).Replace(",", "★");
            }



            //sb("<div class='input-group-append'>");
            sb(string.Format("<button type='button' id='cmdCombo{0}' class='btn dropdown-toggle form-control' data-bs-toggle='dropdown' aria-expanded='false' style='border: solid 1px #C8C8C8; border-radius: 3px;width:100%;text-align:left;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;'>", strControlID));
            if (this.PlaceHolder != null)
            {
                sb(this.PlaceHolder);
            }
            sb("</button>");

            sb(string.Format("<div id='divDropdown{0}' class='dropdown-menu' aria-labelledby='cmdCombo{0}' style='width:100%;' tabindex='-1'>", strControlID));
            sb(string.Format("<div id='divData{0}' style='height:{1};overflow:auto;width:100%;min-width:200px;'>", strControlID, this.dropdown_height));
            sb("</div>");
            sb("</div>");

            sb("");
            sb("");


            sb("</div>");

            sb(string.Format("<input type='hidden' id='value_alias_{0}' name='{1}' value='{2}'/>", strControlID, this.SelectedText.Name, this.SelectedText.Model));


            sb("<script type='text/javascript'>");
            sb("");
            _sb.Append(string.Format("var c{0}=", strControlID));
            _sb.Append("{");
            _sb.Append(string.Format("controlid: '{0}',posturl: '/TheCombo/GetHtml4Checkboxlist',entity:'{1}',masterprefix:'{2}',masterpid:{3},selectedvalues:'{4}',param1:'{5}',on_after_change:'{6}',placeholder:'{7}'", strControlID, this.Entity, this.masterprefix, this.masterpid, strSelectedValues, this.Param1, this.Event_After_ChangeValue, this.PlaceHolder));
            _sb.Append("};");

            sb("");
            sb(string.Format("mycombochecklist_init(c{0});", strControlID));


            sb("");
            sb("</script>");


            sb(string.Format("<input type='hidden' id='{0}' name='{1}' value=\"{2}\" />", strControlID, this.For.Name, strSelectedValues));




            output.Content.AppendHtml(_sb.ToString());
        }

        private void sb(string s)
        {
            _sb.AppendLine(s);

        }
    }
}
