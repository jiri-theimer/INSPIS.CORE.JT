using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UIFT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace UIFT.Controllers
{
    /// <summary>
    /// Exportuje dotaznik do ruznych formatu
    /// </summary>
    //[AllowAnonymous]
    public class ExportController : BaseController
    {
        private readonly AppConfiguration _config;
        public ExportController(AppConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Obsah modalniho dialogu pro vyber sablony
        /// </summary>
        /// <returns>HTML</returns>
        public ActionResult WordDialog()
        {
            // URL exportu
            ViewBag.Url = Url.RouteUrl("export", new { a11id = this.PersistantData.a11id, action = "ToWord" });

            // sablony
            var model = JsonConvert.DeserializeObject<ExportTemplateFile[]>(UiRepository.BL.GlobalParams.LoadParam("UIFT_WordExportTemplate"));

            return PartialView(model);
        }

        /// <summary>
        /// Export do Wordu
        /// </summary>
        /// <param name="templateId">ID sablony</param>
        /// <returns>docx soubor streamovany na klienta</returns>
        public ActionResult ToWord(int templateId, bool showAnswers = true)
        {
            // sablony
            var templates = JsonConvert.DeserializeObject<List<ExportTemplateFile>>(UiRepository.BL.GlobalParams.LoadParam("UIFT_WordExportTemplate"));

            if (!templates.Exists(t => t.ID == templateId))
                return null;
            string filename = templates.First(t => t.ID == templateId).Filename;

            // vytvoreni instance formulare
            Models.Formular formular = this.UiRepository.GetFormular(this.PersistantData.f06id);

            byte[] bytes = null;
            using (var fs = System.IO.File.OpenRead(filename))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    ExportToWord export = new ExportToWord(this.UiRepository, _config);
                    export.Run(formular, showAnswers, ms);
                    bytes = ms.ToArray();
                }
            }

            return File(bytes, "application/vnd.ms-word", "export.docx");
        }
    }
}
