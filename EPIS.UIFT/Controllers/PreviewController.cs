using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UIFT.Controllers
{
    public class PreviewController : BaseFormularController
    {
        /// <summary>
        /// Preview celeho formulare
        /// </summary>
        [IsPreview]
        public ActionResult Formular()
        {
            return base.BaseIndex(0, 0, 1, 1);
        }

        /// <summary>
        /// Zobrazeni sekce formulare
        /// </summary>
        /// <param name="id">f18id</param>
        [IsPreview]
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
        [IsPreview]
        public ActionResult Otazka(int id)
        {
            // !!!TODO
            Models.IOtazka model = this.UiRepository.GetOtazka(id, this.PersistantData.f06id, true, true);
            if (model == null)
                return RedirectToAction("Index", "Error", new { code = 13 });
            else
                return View(model);
        }

        [IsPreview]
        public ActionResult Uvod()
        {
            // instance formulare
            Models.Formular model = this.UiRepository.GetFormularBase(this.PersistantData.f06id);

            // formular nebyl nalezen
            if (model == null)
                return RedirectToAction("Index", "Error", new { code = 11 });

            return View("~/Views/Formular/Uvod.cshtml", model);
        }

        [IsPreview]
        public ActionResult Shrnuti()
        {
            // seznam povinnych nevyplnenych otazek
            Models.ShrnutiResult model = this.UiRepository.GetShrnuti(this.PersistantData.f06id);

            return View("~/Views/Formular/Shrnuti.cshtml", model);
        }
    }
}
