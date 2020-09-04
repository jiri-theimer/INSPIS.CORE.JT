using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFT.Controllers
{
    public class SekceController : BaseController
    {
        /// <summary>
        /// Zobrazeni jednotlive sekce ve formulari
        /// </summary>
        /// <param name="id">f18id</param>
        public ActionResult Index(int id)
        {
            List<Models.Sekce> model = new List<Models.Sekce>(1);
            Models.Sekce sekce = this.UiRepository.GetSekce(id, this.PersistantData.f06id, this.PersistantData.IsPreview);

            // sekce neexistuje
            if (sekce == null)
                return RedirectToAction("Index", "Error", new { code = 12 });

            model.Add(sekce);

            // kontrola zda jsou v sekci nejake kryptovane otazky, pokud ano a uzivatel ma pravo videt jejich obsah, tak to zalogovat
            if (!this.PersistantData.IsEncrypted && sekce.Otazky.Any(t => t.IsEncrypted))
            {
                List<BO.f32FilledValue> encList = new List<BO.f32FilledValue>();

                // jen otazky, ktere nejsou skryte a maji odpovedi
                foreach (var otazka in sekce.Otazky.Where(t => t.IsEncrypted && !t.IsHidden && t.VyplneneOdpovedi != null))
                {
                    if (otazka.VyplneneOdpovedi.Count > 0)
                    {
                        BO.f32FilledValue value = new BO.f32FilledValue
                        {
                            f19ID = otazka.PID,
                            a11ID = this.PersistantData.a11id,
                            f32ID = otazka.VyplneneOdpovedi[0].pid
                        };

                        encList.Add(value);
                    }
                }

                if (encList.Count > 0)
                {
                    // ulozit do logu (pokud je co)
                    this.UiRepository.LogEncryptedQuestionsView(encList, this.Request.GetDisplayUrl());
                }
            }

            // template sekce
            string template = string.IsNullOrEmpty(model[0].Base.f18RazorTemplate) ? "Index" : string.Format("~/Views/Sekce/CustomTemplates/{0}.cshtml", model[0].Base.f18RazorTemplate);

            return View(template, model);
        }
    }
}
