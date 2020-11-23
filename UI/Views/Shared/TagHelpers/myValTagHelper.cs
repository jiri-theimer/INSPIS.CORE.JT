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
    [HtmlTargetElement("myval")]

    public class myValTagHelper : TagHelper
    {        

        [HtmlAttributeName("value")]
        public object Value { get; set; }

        [HtmlAttributeName("datatype")]
        public string DataType { get; set; } = "string";

        [HtmlAttributeName("format")]
        public string Format { get; set; }
        [HtmlAttributeName("linkurl")]
        public string LinkUrl { get; set; }
        [HtmlAttributeName("linktarget")]
        public string LinkTarget { get; set; }

        [HtmlAttributeName("tooltip")]
        public string Tooltip { get; set; }

        [HtmlAttributeName("hoverprefix")]
        public string HoverPrefix { get; set; }
        
        [HtmlAttributeName("hoverpid")]
        public int HoverPid { get; set; }

        [HtmlAttributeName("hoverurl")]
        public string HoverUrl { get; set; }

        [HtmlAttributeName("hoverinfo")]
        public string HoverInfo { get; set; }

        [HtmlAttributeName("hoversymol")]
        public string HoverSymbol { get; set; }

        [HtmlAttributeName("cmprefix")]
        public string CmPrefix { get; set; }
        [HtmlAttributeName("cmdpid")]
        public int CmPid { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            if (this.HoverSymbol == null)
            {
                this.HoverSymbol = "ℹ";
                //this.HoverSymbol = "i";
                //this.HoverSymbol = "<img src='/Images/info.png'/>";
            }
            if (this.HoverPrefix !=null || this.HoverInfo != null || this.HoverUrl != null)
            {
                output.Attributes.SetAttribute("class", "val-readonly-wrap rowvalhover");                
            }
            else
            {
                output.Attributes.SetAttribute("class", "val-readonly");
            }
            
            
            if (this.Tooltip != null)
            {
                output.Attributes.SetAttribute("title", this.Tooltip);
            }
            if (this.CmPid > 0)
            {
                output.Content.AppendHtml(string.Format("<a class=\"cm h4\" onclick=\"_cm(event, '{0}',{1})\">☰</a>", this.CmPrefix, this.CmPid));
            }
            if (this.Value != null)
            {
                
                switch (this.DataType)
                {
                    case "date":
                        if (this.Format == null)
                        {
                            output.Content.Append(BO.BAS.ObjectDate2String(this.Value));
                        }
                        else
                        {
                            output.Content.Append(BO.BAS.ObjectDate2String(this.Value, this.Format));
                        }
                        break;
                    case "datetime":
                        if (this.Format == null)
                        {
                            output.Content.Append(BO.BAS.ObjectDateTime2String(this.Value));
                        }
                        else
                        {
                            output.Content.Append(BO.BAS.ObjectDateTime2String(this.Value, this.Format));
                        }
                        break;
                    case "num":
                        output.Content.Append(BO.BAS.Number2String(Convert.ToDouble(this.Value)));
                        break;
                    case "html":
                        output.Content.AppendHtml(this.Value.ToString());
                        break;
                    case "link":
                        if (this.LinkTarget == null)
                        {
                            output.Content.AppendHtml(string.Format("<a href=\"{0}\">{1}</a>", this.LinkUrl, this.Value));
                        }
                        else
                        {
                            output.Content.AppendHtml(string.Format("<a target='{2}' href=\"{0}\">{1}</a>", this.LinkUrl, this.Value,this.LinkTarget));
                        }
                        
                        break;
                    case "string":
                    default:
                        output.Content.Append(this.Value.ToString());

                        break;
                }
                
            }


            if (this.HoverPrefix != null)
            {
                //output.Content.AppendHtml(string.Format("<a class='valhover_tooltip' onclick=\"_zoom(event,'{0}',{1},900,'Info')\">{2}</a>", this.HoverPrefix, this.HoverPid, this.HoverSymbol));
                if (this.DataType == "html")
                {
                    output.Content.AppendHtml(string.Format("<a class='reczoom' data-rel='/{0}/Info?pid={1}&hover_by_reczoom=1'>{2}</a>", this.HoverPrefix, this.HoverPid, this.HoverSymbol));
                }
                else
                {
                    output.Content.AppendHtml(string.Format("<a class='reczoom' data-title='{0}' data-rel='/{1}/Info?pid={2}&hover_by_reczoom=1'>{3}</a>", this.Value, this.HoverPrefix, this.HoverPid, this.HoverSymbol));
                }
                
            }
            if (this.HoverInfo != null)
            {
                output.Content.AppendHtml(string.Format("<a class='valhover_tooltip' href=\"javascript: alert({0})\">{1}</a>", BO.BAS.GS(this.HoverInfo),this.HoverSymbol));
            }
            if (this.HoverUrl != null)
            {
                output.Content.AppendHtml(string.Format("<a class='valhover_tooltip' target='_blank' href=\"{0}\">{1}</a>", this.HoverUrl,this.HoverSymbol));
            }

        }
    }
}
