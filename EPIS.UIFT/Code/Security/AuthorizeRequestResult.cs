using System;

namespace UIFT.Security
{
    public class AuthorizeRequestResult
    {
        public AuthorizeRequestResult()
        {
            this.FailedCode = 0;
            this.Success = false;
        }

        public PersistantDataStorage PersistantStorage;

        public UIFTUser User;

        /// <summary>
        /// True pokud je uzivatel autentikovan a ma pravo na dany formular
        /// </summary>
        public bool Success;

        /// <summary>
        /// Kod chyby pri neuspesne autentikaci
        /// </summary>
        public int FailedCode;
    }
}