using System;
using System.IO;

namespace BO.Ginis
{
    public class GinisFile
    {
        private string m_sIdDokumentu;
        private string m_sIdSouboru;
        private string m_sJmenoOrigSouboru;
        private string m_sTypVazby;
        private string m_sPopisSouboru;
        private string m_sPodrobnyPopisSouboru;
        private string m_sJmenoTempSouboru;
        private int m_iVerze;
        private FileStream m_oData;
        private string m_sPoznamka;

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

        public string JmenoOrigSouboru
        {
            get
            {
                return m_sJmenoOrigSouboru;
            }
            set
            {
                m_sJmenoOrigSouboru = value;
            }
        }

        public string JmenoTempSouboru
        {
            get
            {
                return m_sJmenoTempSouboru;
            }
            set
            {
                m_sJmenoTempSouboru = value;
            }
        }

        public string TypVazby
        {
            get
            {
                return m_sTypVazby;
            }
            set
            {
                m_sTypVazby = value;
            }
        }

        public FileStream Data
        {
            get
            {
                return m_oData;
            }
            set
            {
                m_oData = value;
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

        public string PodrobnyPopisSouboru
        {
            get
            {
                return m_sPodrobnyPopisSouboru;
            }
            set
            {
                m_sPodrobnyPopisSouboru = value;
            }
        }

        public string Poznamka
        {
            get
            {
                return m_sPoznamka;
            }
            set
            {
                m_sPoznamka = value;
            }
        }

        public string VerzeSouboru
        {
            get
            {
                return m_iVerze.ToString();
            }
            set
            {
                m_iVerze = Convert.ToInt32(value);

            }
        }
    }


}
