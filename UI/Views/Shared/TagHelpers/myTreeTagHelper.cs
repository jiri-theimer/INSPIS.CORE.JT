﻿using System;
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
    [HtmlTargetElement("mytree")]
    public class myTreeTagHelper : TagHelper
    {
        private System.Text.StringBuilder _sb;
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        public string ClientID_RootUl { get; set; } = "tree1";

        [HtmlAttributeName("treestate")]
        public ModelExpression TreeState { get; set; }
       

        [HtmlAttributeName("expandall")]
        public bool ExpandAll { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            List<myTreeNode> lisModel = this.For.Model as List<myTreeNode>;
            
            

            _sb = new System.Text.StringBuilder();
            int intLastLevel = 0;

            sb(string.Format("<input type='hidden' id='treeState1' name='{0}' value='{1}'/>", this.TreeState.Name, this.TreeState.Model));

            sb(string.Format("<ul id='{0}' class='tree_ul'>", this.ClientID_RootUl));
            foreach (var rec in lisModel)
            {
                
                if (rec.TreeLevel< intLastLevel)
                {
                    for(int x = rec.TreeLevel; x < intLastLevel; x++)
                    {
                        sb("</ul>");
                        sb("</li>");
                    }
                    
                }
                if (rec.TreeIndexTo > rec.TreeIndexFrom)
                {
                    sb("<li>");
                    if (rec.CssClass == null)
                    {
                        rec.CssClass = "caret";
                    }
                    else
                    {
                        rec.CssClass += " caret";
                    }
                    sb(string.Format("<span id='rx{0}' class='{1}'>", rec.Pid,rec.CssClass));

                    
                    if (rec.Prefix != null && rec.Pid > 0)
                    {
                        sb(string.Format("<a class='cm' onclick='_cm(event,\"{0}\",{1})'>&#9776;</a>", rec.Prefix, rec.Pid));
                    }
                    if (rec.ImgUrl != null)
                    {
                        sb("<img style='margin:3px;' src='/Images/" + rec.ImgUrl + "'/>");
                    }
                    if (rec.Url != null)
                    {
                        sb(string.Format("<a href='{0}'>{1}</a>",rec.Url, rec.Text));
                    }
                    else
                    {
                        sb(rec.Text);
                    }


                    sb("</span>");


                    sb("<ul class='tree_ul branch_nested'>");
                    
                }
                else
                {                    
                    if (rec.TreeLevel == 1)
                    {
                        sb("<li class='root_nocaret'>");
                    }
                    else
                    {
                        sb("<li>");
                    }
                    

                    if (rec.Prefix != null && rec.Pid > 0)
                    {
                        sb(string.Format("<a class='cm' onclick='_cm(event,\"{0}\",{1})'>&#9776;</a>", rec.Prefix, rec.Pid));
                    }
                    if (rec.ImgUrl != null)
                    {
                        sb("<img style='margin:3px;' src='/Images/" + rec.ImgUrl + "'/>");
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
                            sb("<span class='"+rec.CssClass+"'>" + rec.Text + "</span>");
                        }
                        
                    }                    
                    sb("</li>");
                }
                intLastLevel = rec.TreeLevel;
            }
            for(int i = 1; i < intLastLevel; i++)
            {
                sb("</ul>");
                sb("</li>");
            }

            sb("</ul>");

            sb("<script type='text/javascript'>");
            sb("");
            

            sb("");
            if (this.ExpandAll)
            {
                sb("mytree_init(true);");
            }
            else
            {
                sb("mytree_init(false);");
            }
            


            sb("");
            sb("</script>");

            output.Content.AppendHtml(_sb.ToString());
        }

        private void sb(string s)
        {
            _sb.Append(s);
        }
    }
}
