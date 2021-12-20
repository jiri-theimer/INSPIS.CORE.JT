using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Ginis
{
    public class GinisDocument
    {
        private string m_sIdDokumentu;
        private string m_sIdSpisu;
        //private string m_sPriznakpisu;
        private string m_sPriznakCj;
        private string m_sPriznakElObrazu;
        private string m_sPriznakFyzExistence;
        private bool m_sPriznakSpisu;
        private string m_sIdSouboru;
        private string m_sJmenoSouboru;
        private string m_sPopisSouboru;
        private string m_sIdTypuDokumentu;
        private List<GinisFile> m_oSeznamPriloh = new List<GinisFile>();
        private string m_sVec;
        private string m_sZnacka;

        public string IdDokumentu
        {
            get
            {
                return m_sIdDokumentu;
            }
            set
            {
                m_sIdDokumentu = value;
            }
        }

        public string IdSpisu
        {
            get
            {
                return m_sIdSpisu;
            }
            set
            {
                m_sIdSpisu = value;
            }
        }

        public bool PriznakSpisu
        {
            get
            {
                return m_sPriznakSpisu;
            }
            set
            {
                m_sPriznakSpisu = value;
            }
        }

        public string PriznakElObrazu
        {
            get
            {
                return m_sPriznakElObrazu;
            }
            set
            {
                m_sPriznakElObrazu = value;
            }
        }

        public string PriznakCj
        {
            get
            {
                return m_sPriznakCj;
            }
            set
            {
                m_sPriznakCj = value;
            }
        }

        public string PriznakFyzExistence
        {
            get
            {
                return m_sPriznakFyzExistence;
            }
            set
            {
                m_sPriznakFyzExistence = value;
            }
        }

        public string IdSouboru
        {
            get
            {
                return m_sIdSouboru;
            }
            set
            {
                m_sIdSouboru = value;
            }
        }

        public string JmenoSouboru
        {
            get
            {
                return m_sJmenoSouboru;
            }
            set
            {
                m_sJmenoSouboru = value;
            }
        }

        public string PopisSouboru
        {
            get
            {
                return m_sPopisSouboru;
            }
            set
            {
                m_sPopisSouboru = value;
            }
        }

        public string IdTypuDokumentu
        {
            get
            {
                return m_sIdTypuDokumentu;
            }
            set
            {
                m_sIdTypuDokumentu = value;
            }
        }

        public List<GinisFile> SeznamPriloh
        {
            get
            {
                return m_oSeznamPriloh;
            }
            set
            {
                m_oSeznamPriloh = value;
            }
        }

        public string Vec
        {
            get
            {
                return m_sVec;
            }
            set
            {
                m_sVec = value;
            }
        }

        public string Znacka
        {
            get
            {
                return m_sZnacka;
            }
            set
            {
                m_sZnacka = value;
            }
        }
    }

}
