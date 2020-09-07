using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UIFT.Controllers
{
    public class FormularController : BaseFormularController
    {
        public ActionResult Index(int sekce = 0, int otazka = 0, [Range(1,2)] int layout = 1, [Range(1,2)] int template = 1)
        {
            return base.BaseIndex(sekce, otazka, layout, template);
        }

        /// <summary>
        /// Zobrazeni uvodni stranky formulare
        /// </summary>
        public ActionResult Uvod()
        {
            // instance formulare
            Models.Formular model = this.UiRepository.GetFormularBase(this.PersistantData.f06id);

            // formular nebyl nalezen
            if (model == null)
                return RedirectToAction("Index", "Error", new { code = 11 });

            return View(model);
        }

        /// <summary>
        /// Shrnuti a ukonceni formulare
        /// </summary>
        public ActionResult Shrnuti()
        {
            // seznam povinnych nevyplnenych otazek
            Models.ShrnutiResult model = this.UiRepository.GetShrnuti(this.PersistantData.f06id);

            return View(model);
        }

        /// <summary>
        /// Uzamknuti formulare
        /// </summary>
        [HttpPost]
        public ActionResult Zamknuti()
        {
            if (this.PersistantData.FormLockFlag == BO.f06UserLockFlagEnum.NoLockOffer || this.PersistantData.FormLockFlag == BO.f06UserLockFlagEnum.NotSpecified)
            {
                return Json(new { success = false, message = UiRepository.BL.tra("Tento formulář nelze uzamknout!") });
            }
            else
            {
                bool b = true;
                if (this.PersistantData.FormLockFlag == BO.f06UserLockFlagEnum.LockOnlyIfValid)
                {
                    // seznam povinnych nevyplnenych otazek
                    Models.ShrnutiResult model = this.UiRepository.GetShrnuti(this.PersistantData.f06id);
                    b = model.Success;
                }

                // pokud lze uzavrit
                if (b)
                {
                    if (this.PersistantData.a11IsPoll) // pokud je to anketa, presmerovat na prihlasovaci obrazovku
                    {
                        this.UiRepository.BL.a11EventFormBL.LockUnLockPolls(this.PersistantData.a01id, true);
                        return Json(new { success = true, url = Url.Action("AnketaZamkcena", "Login") });
                    }
                    else
                    {
                        this.UiRepository.BL.a11EventFormBL.LockUnLockForm(this.PersistantData.a11id, true);
                        return Json(new { success = true, url = Url.RouteUrl("form", new { a11id = this.PersistantData.a11id }) });
                    }
                }
                else
                {
                    return Json(new { success = false, message = UiRepository.BL.tra("Nejsou vyplněny všechny povinné otázky!") });
                }
            }
        }
    }
}
