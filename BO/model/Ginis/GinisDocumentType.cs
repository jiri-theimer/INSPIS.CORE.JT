using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Ginis
{
    public class GinisDocumentType
    {
        private string m_sIdTypuDokumentu;
        private string m_sIdNazevTypuDokumentu;

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

        public string IdNazevTypuDokumentu
        {
            get
            {
                return m_sIdNazevTypuDokumentu;
            }
            set
            {
                m_sIdNazevTypuDokumentu = value;
            }
        }
    }
}
