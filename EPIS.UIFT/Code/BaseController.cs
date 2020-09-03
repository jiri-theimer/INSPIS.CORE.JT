using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UIFT
{
    /// <summary>
    /// Zakladni layouty pro formulare
    /// </summary>
    public enum FormularLayoutTypes
    {
        Default = 1,
        NoHeaders = 2
    }

    /// <summary>
    /// Sablony zobrazeni formulare. Kazda sablona muze byt zobrazena v ruznych Layoutech
    /// </summary>
    public enum FormularTemplateTypes
    {
        Default = 1,
        ModalWindow = 2
    }

    /// <summary>
    /// Vychozi controller pro vsechny ostatni controllery v projektu
    /// </summary>
    [Authorize]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Instance prihlaseneho uzivatele
        /// </summary>
        public new Security.UIFTUser User
        {
            get;
            private set;
        }

        /// <summary>
        /// Data o aktualni udalosti a formulari pretrvavajici mezi jednotlivymi pozadavky.
        /// </summary>
        public PersistantDataStorage PersistantData
        {
            get { return _PersistantData; }
            set
            {
                HttpContext.Items["PersistantData"] = _PersistantData = value;
            }
        }
        private PersistantDataStorage _PersistantData;

        /// <summary>
        /// Instance databazoveho repository
        /// </summary>
        public Repository.Repository UiRepository
        {
            get
            {
                if (_uiRepository == null)
                {
                    var fac = (UIFT.Repository.RepositoryFactory)this.HttpContext.RequestServices.GetService(typeof(UIFT.Repository.RepositoryFactory));
                    _uiRepository = this.PersistantData == null ? fac.Get() : fac.Get(this.PersistantData.a11id);
                }
                return _uiRepository;
            }
        }
        private Repository.Repository _uiRepository;

        /// <summary>
        /// Autorizace uzivatele
        /// </summary>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            this.User = HttpContext.User as Security.UIFTUser;
            this._PersistantData = this.HttpContext.Items["PersistantData"] as PersistantDataStorage;
        }
    }
}
