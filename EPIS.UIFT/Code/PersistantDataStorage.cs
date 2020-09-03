using System;

namespace UIFT
{
    /// <summary>
    /// Kontajner pro data zachovavana mezi jednotlivymi pozadavky na server
    /// </summary>
    public class PersistantDataStorage
    {
        /// <summary>
        /// ID udalosti
        /// </summary>
        public int a11id;

        /// <summary>
        /// ID udalosti
        /// </summary>
        public int a01id;

        /// <summary>
        /// ID formulare
        /// </summary>
        public int f06id;

        /// <summary>
        /// Moznosti uzamknuti formulare
        /// </summary>
        public BO.f06UserLockFlagEnum FormLockFlag;

        /// <summary>
        /// Zakladni layout formulare
        /// </summary>
        public FormularLayoutTypes Layout;

        /// <summary>
        /// Sablony zobrazeni formulare. Kazda sablona muze byt zobrazena v ruznych Layoutech
        /// </summary>
        public FormularTemplateTypes Template;

        /// <summary>
        /// True, pokud je formular v preview rezimu
        /// </summary>
        public bool IsPreview;

        public bool a11IsPoll;

        /// <summary>
        /// True, pokud uzivatel nema videt odpovedi v dotazniku (videt jen hvezdicky)
        /// </summary>
        public bool IsEncrypted;
    }
}