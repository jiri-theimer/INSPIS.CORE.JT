namespace UIFT.Repository
{
    public partial class Repository
    {
        // ulozena instance BL factory tridy
        private readonly BL.Factory factory;

        // instance kesovaci tridy
        internal Cache cache;

        // ID udalosti
        public int a11id
        {
            get;
            private set;
        }

        /// <summary>
        /// Instance BL
        /// </summary>
        public BL.Factory BL
        {
            get
            {
                return factory;
            }
        }

        /// <summary>
        /// Posledni chyba pri vykonavani funkce
        /// </summary>
        public string LastError
        {
            get
            {
                return _LastError;
            }
        }
        private string _LastError;

        /// <summary>
        /// Konstruktor
        /// </summary>
        internal Repository(BL.Factory fac, int a11id)
        {
            this.a11id = a11id;
            
            // vytvorit instanci BL tovarny
            this.factory = fac;
            
            // vytvorit instanci kesovaci tridy
            this.cache = new Cache(fac, false);
        }
    }
}