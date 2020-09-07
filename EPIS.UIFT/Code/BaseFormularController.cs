using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UIFT.Controllers
{
    public abstract class BaseFormularController : BaseController
    {
        protected ActionResult BaseIndex(int sekce, int otazka, int layout, int template)
        {
            // vytvoreni instance formulare
            UIFT.Models.Formular formular = this.UiRepository.GetFormular(this.PersistantData.f06id);

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
                UIFT.Models.IOtazka selOtazka = formular.Otazky.Find(t => t.PID == otazka);
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

            return View($"~/Views/Formular/{this.PersistantData.Template.ToString()}.cshtml", formular);
        }
    }
}
