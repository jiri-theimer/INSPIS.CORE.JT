using Microsoft.AspNetCore.Mvc;
using System;

namespace UIFT.Controllers
{
    public class PreviewController : BaseController
    {
        /// <summary>
        /// Preview celeho formulare
        /// </summary>
        //[RefreshSecurityCookie(true)]
        [IsPreview(true)]
        public ActionResult Formular()
        {
            // set Formular/Index call as preview
            /*if (!this.HttpContext.Items.ContainsKey("SECURITY_PREVIEW"))
                this.HttpContext.Items.Add("SECURITY_PREVIEW", true);*/

            return RedirectToAction("Index", "Formular");
        }

        /// <summary>
        /// Zobrazeni sekce formulare
        /// </summary>
        /// <param name="id">f18id</param>
        //[RefreshSecurityCookie(true)]
        [IsPreview(true)]
        public ActionResult Sekce(int id)
        {
            Models.Sekce model = this.UiRepository.GetSekce(id, this.PersistantData.f06id, true);

            // sekce neexistuje
            if (model == null)
                return RedirectToAction("Index", "Error", new { code = 12 });

            return View(model);
        }

        /// <summary>
        /// Zobrazeni otazky ve formulare
        /// </summary>
        /// <param name="id">f19id</param>
        //[RefreshSecurityCookie(true)]
        [IsPreview(true)]
        public ActionResult Otazka(int id)
        {
            // !!!TODO
            Models.IOtazka model = this.UiRepository.GetOtazka(id, this.PersistantData.f06id, true, true);
            if (model == null)
                return RedirectToAction("Index", "Error", new { code = 13 });
            else
                return View(model);
        }
    }
}
