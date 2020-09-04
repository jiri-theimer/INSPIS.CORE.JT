using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFT.Controllers
{
    public class OtazkaController : BaseController
    {
        /// <summary>
        /// Ulozeni zmeny komentare k otazce
        /// </summary>
        /// <param name="f19id">ID otazky</param>
        /// <param name="f21id"></param>
        /// <param name="value">Text komentare</param>
        /// <returns>
        /// JSON: 
        /// { success = b, message = this.UiRepository.LastError }
        /// OR
        /// { f19id = f19id, success = true, defaultValues = defaultVals }
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Komentar(int f19id, int f21id, string value)
        {
            bool b = this.UiRepository.SaveKomentar(ref f19id, ref f21id, ref value);

            if (b)
            {
                // projed default values pro vsechny otazky
                // musi byt pro vsechny otazky, protoze otazka v aktualnim segmentu muze zaviset na otazce v jinem segmentu
                var defaultVals = this.UiRepository.FormularGenerovatDefaultValues(this.PersistantData.f06id)
                    .Select(t => new { f19id = t.f19ID, f21id = t.f21ID, value = t.Value });

                return Json(new
                {
                    f19id = f19id,
                    success = true,
                    defaultValues = defaultVals
                });
            }
            else
                return Json(new { success = false, message = this.UiRepository.LastError });
        }

        /// <summary>
        /// Ulozeni zmeny v publikovani otazky
        /// </summary>
        /// <param name="f19id">ID otazky</param>
        /// <param name="value">true / false</param>
        /// <returns>JSON: { success = b, message = this.UiRepository.LastError }</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PublikovaniOtazky(int f19id, int f25id, int f26id, bool value)
        {
            // zkus publikovat otazku
            bool b = this.UiRepository.PublikovaniOtazky(f25id, f26id, f19id, value);

            return Json(new { success = b, message = this.UiRepository.LastError });
        }

        /// <summary>
        /// Smazani vsech odpovedi na otazku
        /// </summary>
        /// <param name="f19id">ID otazky</param>
        /// <returns>JSON: { success = b, message = this.UiRepository.LastError }</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vycisteni(int f19id, int f26id = 0)
        {
            bool b = false;
         
            if (f19id > 0) // normalni otazka
                b = this.UiRepository.VycisteniOtazky(f19id);
            else if (f26id > 0) // baterie
                b = this.UiRepository.VycisteniBaterie(f26id);

            return Json(new { success = b, message = this.UiRepository.LastError });
        }

        /// <summary>
        /// Ulozeni odpovedi na otazku
        /// </summary>
        /// <param name="inputID">Client ID elementu, ktery akci vyvolal</param>
        /// <param name="value">Hodnota odpovedi</param>
        /// <param name="alias">Text odpovedi v pripade EvalListu</param>
        /// <returns>JSON: { inputID = inputID, success = ret, message = this.UiRepository.LastError }</returns>
        //[ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ulozeni(string inputID, int f19id, int f21id, string value, bool? filledByEval, string alias = "")
        {
            // instance otazky
            Models.IOtazka otazka = this.UiRepository.GetOtazka(f19id, this.PersistantData.f06id);

            // pokud otazka neexistuje
            if (otazka == null)
                return Json(new { 
                    inputID = inputID, 
                    success = false, 
                    message = UiRepository.BL.tra("Otázka neexistuje")
                });

            // zkontrolovat zda otazka neni RO
            if (this.UiRepository.IsQuestionReadOnly(otazka))
            {
                return Json(new
                {
                    inputID = inputID, 
                    success = false,
                    message = UiRepository.BL.tra("Otázka je pouze pro čtení")
                });
            }

            // zkontrolovat hondotu validacniho regex vyrazu
            if (!string.IsNullOrEmpty(otazka.Regex))
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(otazka.Regex);
                if (!regex.IsMatch(value))
                {
                    return Json(new
                    {
                        inputID = inputID, 
                        success = false,
                        message = UiRepository.BL.tra("Odpověď nemá správný formát: ") + otazka.Regex
                    });
                }
            }

            // pokud se vrati false, znamena to ze zadana odpoved nevyhovuje nastaven otazky
            bool ret = this.UiRepository.SaveOtazka(otazka, ref f21id, ref value, alias, filledByEval.HasValue ? filledByEval.Value : false);
            
            if (ret) // pokud se povedlo ulozeni
            {
                // zjisti ktere otazky se maji skryt/zobrazit
                // musi byt pro vsechny segmenty, protoze je mozne ze nektere segmenty u kterych budou skryte vsechny otazky bude nutne skryt
                int[] skryteSegmenty; // ID segmentu, ktere nemaji zadne viditelne otazky - musi tedy byt skryty
                List<int> skryteOtazky = this.UiRepository.FormularSkryteOtazky(this.PersistantData.f06id, out skryteSegmenty);

                // notifikovat BL se seznamem skrytych otazek
                this.UiRepository.BL.f32FilledValueBL.SaveHiddenQuestionsInForm(this.PersistantData.a11id, skryteOtazky);
                
                // projed default values pro vsechny otazky
                // musi byt pro vsechny otazky, protoze otazka v aktualnim segmentu muze zaviset na otazce v jinem segmentu
                var defaultVals = this.UiRepository.FormularGenerovatDefaultValues(this.PersistantData.f06id)
                    .Select(t => new { f19id = t.f19ID, f21id = t.f21ID, value = t.Value });

                // zjistit readonly otazky v aktualnim segmentu
                var readonlyOtazky = this.UiRepository.SegmentReadonlyOtazky(otazka.IdSekce, this.PersistantData.f06id);

                // zjistit required otazky v aktualnim segmentu
                var requiredOtazky = this.UiRepository.SegmentRequiredOtazky(otazka.IdSekce, this.PersistantData.f06id);

                return Json(new {
                    f19id = f19id,
                    inputID = inputID, 
                    success = ret, 
                    hiddenSegments = skryteSegmenty, 
                    hiddenQuestions = skryteOtazky, 
                    defaultValues = defaultVals,
                    readOnly = readonlyOtazky,
                    required = requiredOtazky
                });
            }
            else
            {
                return Json(new { 
                    inputID = inputID, 
                    success = ret, 
                    message = this.UiRepository.LastError 
                });
            }
        }

        /// <summary>
        /// Kliknuti na otazku typu button
        /// </summary>
        /// <param name="f19id"></param>
        public ActionResult Button(int f19id, string inputID)
        {
            // instance otazky
            Models.IOtazka otazka = this.UiRepository.GetOtazka(f19id);

            // pokud otazka neexistuje
            if (otazka == null)
                return Json(new { inputID = inputID, success = false, message = UiRepository.BL.tra("Otázka neexistuje") });

            // URL po zavolani cudliku
            string url = ((Models.Otazka)otazka).Base.f19LinkURL;

            // ulozit data volani do tempboxu
            string guid = Guid.NewGuid().ToString();
            BO.p85Tempbox box = new BO.p85Tempbox()
            {
                p85GUID = guid,
                p85DataPID = this.PersistantData.a11id,
                p85OtherKey1 = f19id,
                p85FreeText01 = Url.Action("Index", "Formular", new { a11id = this.PersistantData.a11id, sekce = otazka.IdSekce, otazka = otazka.PID })
            };
            this.UiRepository.BL.p85TempboxBL.Save(box);

            return Json(new { success = true, guid = guid, url = url });
        }
    }
}
