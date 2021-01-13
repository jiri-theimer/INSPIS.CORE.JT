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
    [HtmlTargetElement("mycombo")]
    public class myComboTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("selectedtext")]
        public ModelExpression SelectedText { get; set; }

        [HtmlAttributeName("entity")]
        public string Entity { get; set; }

        [HtmlAttributeName("event_after_changevalue")]
        public string Event_After_ChangeValue { get; set; }

        [HtmlAttributeName("placeholder")]
        public string PlaceHolder { get; set; }
        public string Param1 { get; set; }

        [HtmlAttributeName("masterpid")]
        public int masterpid { get; set; }

        [HtmlAttributeName("masterprefix")]
        public string masterprefix { get; set; }

        public int ViewFlag { get; set; }

        [HtmlAttributeName("filter-flag")]
        public string FilterFlag { get; set; }

        private int _SelectedValue { get; set; }


        private System.Text.StringBuilder _sb;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {


            output.TagMode = TagMode.StartTagAndEndTag;

            if (this.For.Model != null)
            {
                _SelectedValue = Convert.ToInt32(this.For.Model);

            }
            else
            {
                _SelectedValue = 0;
            }
            if (string.IsNullOrEmpty(this.FilterFlag) == true)
            {
                this.FilterFlag = "0";
            }
            _sb = new System.Text.StringBuilder();
            var strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");


            sb(string.Format("<div id='divDropdownContainer{0}' class='dropdown'>", strControlID));

            sb(string.Format("<button type='button' id='cmdCombo{0}' class='btn dropdown-toggle form-control' data-bs-toggle='dropdown' aria-expanded='false' style='border: solid 1px #C8C8C8; border-radius: 3px;width:100%;text-align:left;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;'>{1}</button>", strControlID, this.PlaceHolder));

            sb(string.Format("<button type='button' id='cmdClear{0}' class='btn btn btn-outline-secondary' tabindex='-1' style='position:absolute;top:0;right:0;border: solid 1px #C8C8C8; border-radius: 3px;'>", strControlID));
            sb("&times;");
            sb("</button>");



            sb(string.Format("<div class='dropdown-menu' aria-labelledby='cmdCombo{0}' style='width:100%;' tabindex='-1'>", strControlID));

          
            sb(string.Format("<input id='searchbox{0}' autocomplete='off' type='text' class='form-control' placeholder='🔍...'>", strControlID));

            sb("");


            sb(string.Format("<div id='divData{0}' style='height:220px;overflow:auto;z-index:500;'>", strControlID));
            sb("</div>");


            sb("</div>");   //dropdown-menu
            sb("</div>");   //dropdown





            sb("");



            sb(string.Format("<input type='hidden' value='{0}' data-id='text_{1}' name='{2}'/>", this.SelectedText.Model, strControlID, this.SelectedText.Name));

            sb(string.Format("<input type='hidden' value ='{0}' id='{1}' data-id='value_{1}' name='{2}'/>", _SelectedValue.ToString(), strControlID, this.For.Name));   //asp-for pro hostitelské view


            sb("");
            sb("");
            sb("<script type='text/javascript'>");
            _sb.Append(string.Format("var c{0}=", strControlID));
            _sb.Append("{");
            _sb.Append(string.Format("controlid: '{0}',posturl: '/TheCombo/GetHtml4TheCombo',entity:'{1}',param1: '{2}',defvalue: '{3}',deftext: '{4}',on_after_change: '{5}',viewflag: '{6}',filterflag: '{7}',placeholder: '{8}',masterprefix:'{9}',masterpid:{10}", strControlID, this.Entity, this.Param1, _SelectedValue.ToString(), this.SelectedText.Model, this.Event_After_ChangeValue, this.ViewFlag, this.FilterFlag, this.PlaceHolder, this.masterprefix, this.masterpid));
            _sb.Append("};");
            sb("");
            sb("");
            sb(string.Format("mycombo_init(c{0})", strControlID));

            sb("</script>");

            output.Content.AppendHtml(_sb.ToString());
        }

        private void sb(string s)
        {
            _sb.AppendLine(s);

        }
    }
}
