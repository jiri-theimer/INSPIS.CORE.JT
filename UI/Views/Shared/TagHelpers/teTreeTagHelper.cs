using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UI.Models;


namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("tetree")]
    public class teTreeTagHelper : TagHelper
    {
        private System.Text.StringBuilder _sb;
        
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("clientid")]
        public string ClientID_RootUl { get; set; } = "tree1";

        [HtmlAttributeName("treestate")]
        public ModelExpression TreeState { get; set; }


        [HtmlAttributeName("expandall")]
        public bool ExpandAll { get; set; }

        [HtmlAttributeName("auto_client_init")]
        public bool AutoJsInit { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            List<myTreeNode> lisModel = this.For.Model as List<myTreeNode>;



            _sb = new System.Text.StringBuilder();
            int intLastLevel = 0;

            if (this.TreeState != null)
            {
                sb(string.Format("<input type='hidden' id='treeState1' name='{0}' value='{1}'/>", this.TreeState.Name, this.TreeState.Model));
            }
            

            sb(string.Format("<ul id='{0}'>", this.ClientID_RootUl));
            foreach (var rec in lisModel)
            {

                if (rec.TreeLevel < intLastLevel)
                {
                    for (int x = rec.TreeLevel; x < intLastLevel; x++)
                    {
                        sb("</ul>");
                        sb("</li>");
                    }

                }
                if (rec.TreeIndexTo > rec.TreeIndexFrom)
                {
                    sb("<li");
                    if (rec.Expanded)
                    {
                        sb(" data-expanded='true'");
                    }
                    sb(">");

                    if (string.IsNullOrEmpty(rec.Prefix)==false && rec.Pid > 0)
                    {
                        sb(string.Format("<a class='cm' onclick='_cm(event,\"{0}\",{1})'>&#9776;</a>", rec.Prefix, rec.Pid));
                    }
                    if (rec.ImgUrl != null)
                    {
                        sb("<img src='" + rec.ImgUrl + "'/>");
                    }
                    if (rec.Url != null)
                    {
                        sb(string.Format("<a href='{0}'>{1}</a>", rec.Url, rec.Text));
                    }
                    else
                    {
                        if (rec.CssClass == null)
                        {
                            sb(rec.Text);
                        }
                        else
                        {
                            sb("<span class='" + rec.CssClass + "'>" + rec.Text + "</span>");
                        }
                    }


                    

                    sb("<ul>");

                }
                else
                {
                    sb("<li>");
                   


                    if (string.IsNullOrEmpty(rec.Prefix)==false && rec.Pid > 0)
                    {
                        sb(string.Format("<a class='cm' onclick='_cm(event,\"{0}\",{1})'>&#9776;</a>", rec.Prefix, rec.Pid));
                    }
                    if (rec.ImgUrl != null)
                    {
                        sb("<img src='" + rec.ImgUrl + "'/>");
                    }

                    if (rec.Url != null)
                    {
                        sb(string.Format("<a href='{0}'>{1}</a>", rec.Url, rec.Text));
                    }
                    else
                    {
                        if (rec.CssClass == null)
                        {
                            sb(rec.Text);
                        }
                        else
                        {
                            sb("<span class='" + rec.CssClass + "'>" + rec.Text + "</span>");
                        }

                    }
                    sb("</li>");
                }
                intLastLevel = rec.TreeLevel;
            }
            for (int i = 1; i < intLastLevel; i++)
            {
                sb("</ul>");
                sb("</li>");
            }

            sb("</ul>");

            
            if (this.AutoJsInit)
            {
                sbl("");
                sbl("<script type='text/javascript'>");
                sbl("");

                sbl("$(document).ready(function () {");
                sbl(string.Format("$('#{0}').kendoTreeView();", this.ClientID_RootUl));
                sbl("});</script>");

                sbl("");
                sbl("</script>");
            }

            output.Content.AppendHtml(_sb.ToString());

        }


        private void sb(string s)
        {
            _sb.Append(s);
        }
        private void sbl(string s)
        {
            _sb.AppendLine(s);
        }
    }
}
