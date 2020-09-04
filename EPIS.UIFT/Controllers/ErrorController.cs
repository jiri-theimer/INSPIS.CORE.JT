using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace UIFT.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private static Dictionary<int, string> ErrorMessages = new Dictionary<int, string>
        {
            { 1, "Akce neexistuje." },
            { 2, "Akce je uzamkčena." },
            { 3, "Akce je zavřena." },
            { 4, "Uživatel s tímto loginem neexistuje." },
            { 5, "Uživatel s tímto loginem není platný." },
            { 7, "Security cookie je prázdna." },
            { 8, "Pole Security cookie nemá správný tvar." },
            { 9, "Neplatná IP adresa." },
            { 10, "Nejste zarazen v zadne roli." },
            { 11, "Formulář nebyl nalezen." },
            { 12, "Segment neexistuje." },
            { 13, "Otázka neexistuje." },
            { 14, "Neověřený uživatel" },
            { 15, "Časově omezená akce již není platná" },
            { 16, "Formulář v akci je již uzavřen" },
            { 17, "Formulář je uzavřen" },
            { 18, "V této akci nedisponujete oprávněním k vyplňování formulářů." },
            { 19, "Tato anketa je již uzamčena a nelze pořizovat její náhled." }
        };

        private readonly BL.TheTranslator Translator;
        private readonly AppConfiguration Configuration;

        public ErrorController(BL.TheTranslator translator, AppConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Translator = translator;
        }

        public ActionResult Index(int code)
        {
            ViewBag.Message = Translator.DoTranslate(ErrorMessages[code], Configuration.DefaultLanguage);

            return View("Index");
        }
    }
}
