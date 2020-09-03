using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UIFT.Controllers
{
    public class FormularController : BaseController
    {
        public ActionResult Index(int sekce = 0, int otazka = 0, [Range(1,2)] int layout = 1, [Range(1,2)] int template = 1)
        {
            // vytvoreni instance formulare
            Models.Formular formular = this.UiRepository.GetFormular(this.PersistantData.f06id);

            // ulozit informace o formulari do persistent data
            PersistantDataStorage newStorage = this.PersistantData;
            newStorage.Layout = (FormularLayoutTypes)layout;
            newStorage.Template = (FormularTemplateTypes)template;
            this.PersistantData = newStorage;
            
            // pokud formular nebyl nalezen, presmeruj uzivatele na odpovidajici view
            if (formular == null)
            {
                return RedirectToAction("Index", "Error", new { code = 11 });
            }

            // predvybrana otazka
            if (otazka > 0)
            {
                Models.IOtazka selOtazka = formular.Otazky.Find(t => t.PID == otazka);
                if (selOtazka != null)
                {
                    formular.AktualniOtazka = selOtazka.PID;
                    formular.AktualniSekce = selOtazka.IdSekce;
                }
            }
            // predvybrana sekce
            else if (sekce > 0)
            {
                if (formular.Sekce.Exists(t => t.Base.pid == sekce))
                    formular.AktualniSekce = sekce;
            }

            // informace o layoutu
            ViewBag.Layout = Convert.ToInt32(this.PersistantData.Layout);

            // template formulare
            ViewBag.FormularTemplate = Convert.ToInt32(this.PersistantData.Template);

            return View(this.PersistantData.Template.ToString(), formular);
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
