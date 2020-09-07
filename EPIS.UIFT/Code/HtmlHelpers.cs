using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace UIFT.HtmlHelpers
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Vytiskne retezec identifikujici verzi aplikaci (casovou)
        /// </summary>
        public static HtmlString AppVersion(this IHtmlHelper helper)
        {
            DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
            return new HtmlString(buildDate.ToString("d") + " " + buildDate.ToString("t"));
        }

        public static IHtmlContent EditorForOtazkaBasic<TModel>(this IHtmlHelper helper, TModel model)
        {
            return helper.Partial("~/Views/Shared/EditorTemplates/OtazkaBasic.cshtml", model, new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()));
        }

        public static IHtmlContent EditorForOtazkaBasic<TModel>(this IHtmlHelper helper, TModel model, string label)
        {
            if (model == null)
            {
                return new HtmlString(string.Format("Model {0} not found.", model));
            }
            return helper.Partial("~/Views/Shared/EditorTemplates/OtazkaBasic.cshtml", model, new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { { "label", label } });
        }

        /// <summary>
        /// Vytvari radek s uploadovanym souborem pro otazku typu fileupload
        /// </summary>
        /// <param name="guid">File Guid</param>
        /// <param name="fileLength">Delka souboru</param>
        /// <param name="fileName">Nazev souboru</param>
        public static HtmlString FileUploadRow(this IHtmlHelper helper, int a11id, string guid, int fileLength, string fileName)
        {
            return FileUploadRow(helper.ViewContext.HttpContext, a11id, guid, fileLength, fileName);
        }

        public static HtmlString FileUploadRow(HttpContext context, int a11id, string guid, int fileLength, string fileName)
        {
            var fac = (UIFT.Repository.RepositoryFactory)context.RequestServices.GetService(typeof(UIFT.Repository.RepositoryFactory));
            var generator = (LinkGenerator)context.RequestServices.GetService(typeof(LinkGenerator));
            var url = generator.GetPathByAction("Get", "Files", new { a11id = a11id, id = guid });

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendFormat("<div class=\"uploadFile attachments\" data-guid=\"{0}\">", guid);
            sbHtml.AppendFormat("<button type=\"button\" class=\"buttonSmall buttonIcon btnDelete\"><span></span>{0}</button>", fac.Get().BL.tra("Odstranit soubor"));
            sbHtml.Append("<span class=\"iconFile\"></span>");
            sbHtml.AppendFormat("<a href=\"{0}\">{2}</a> <span>({1} KB)</span>", url, Math.Round(fileLength / 1000d, 1), fileName);
            sbHtml.Append("</div>");

            return new HtmlString(sbHtml.ToString());
        }

        /// <summary>
        /// Helper na vytvoreni stromove struktury sekci.
        /// </summary>
        /// <param name="sekce">List sekci na 1 urovni stromu</param>
        public static HtmlString SekceTree(this IHtmlHelper helper, List<Models.Sekce> sekce, int selectedId = 0)
        {
            var fac = (UIFT.Repository.RepositoryFactory)helper.ViewContext.HttpContext.RequestServices.GetService(typeof(UIFT.Repository.RepositoryFactory));
            var bl = fac.Get().BL;
            var generator = (LinkGenerator)helper.ViewContext.HttpContext.RequestServices.GetService(typeof(LinkGenerator));
            var storage = (PersistantDataStorage)helper.ViewContext.HttpContext.Items["PersistantData"];

            StringBuilder html = new StringBuilder("<ul id=\"segmentsTree\" class=\"level-0\">");

            // prvni uzel ve stromu - odkaz na uvod dotazniku
            html.AppendFormat("<li id=\"menusekce-0\" data-id=\"0\"{1}><a href=\"{0}\">{2}</a></li>", generator.GetPathByAction("Uvod", "Formular", new { a11id = storage.a11id }), selectedId == 0 ? " class=\"sel\"" : "", bl.tra("Úvod (informace k vyplňování)"));

            // vytvorit strom
            SekceTreeRecursiveHelper(generator, html, sekce, storage, ref selectedId);

            // posledni uzel ve stromu - odkaz na shrnuti
            html.AppendFormat("<li id=\"menusekce-00\" data-id=\"00\"><a href=\"{0}\">{1}</a></li>", generator.GetPathByAction("Shrnuti", "Formular", new { a11id = storage.a11id }), bl.tra("Kontrola formuláře"));

            html.Append("</ul>");

            return new HtmlString(html.ToString());
        }

        /// <summary>
        /// Pomocna funkce pro SekceTree()
        /// </summary>
        /// <param name="html">Aktualne vytvoreny HTML string se stromem</param>
        /// <param name="node">Seznam sekci pro aktualni cykl</param>
        private static void SekceTreeRecursiveHelper(LinkGenerator generator, StringBuilder html, List<Models.Sekce> node, PersistantDataStorage storage, ref int selectedId)
        {
            foreach (Models.Sekce sekce in node)
            {
                // wrapper LI
                html.AppendFormat("<li id=\"menusekce-{0}\" data-id=\"{0}\"", sekce.Base.pid);
                if (selectedId == sekce.Base.pid)
                    html.Append(" class=\"sel\"");
                if (sekce.IsHidden)
                    html.Append(" style=\"display:none;\"");
                html.Append(">");

                // pokud neobsahuje otazky, nezobrazovat jako odkaz
                if (sekce.Otazky.Count == 0)
                    html.AppendFormat("<strong>{0}</strong>", sekce.Base.f18Name);
                else // jinka udelat jako odkaz na sekci
                {
                    string url = generator.GetPathByAction(storage.IsPreview ? "Preview" : "Index", "Sekce", new { a11id = storage.a11id, id = sekce.Base.pid });
                    html.AppendFormat("<a href=\"{1}\"><span></span>{0}</a>", sekce.Base.f18Name, url);
                }

                // obsahuje podsekce
                if (sekce.SubSekce.Count > 0)
                {
                    html.AppendFormat("<ul class=\"level-{0}{1}\">", sekce.Base.f18TreeLevel, sekce.Otazky.Count == 0 ? " noBorder" : "");
                    SekceTreeRecursiveHelper(generator, html, sekce.SubSekce, storage, ref selectedId);
                    html.Append("</ul>");
                }
                html.Append("</li>");
            }
        }
    }
}