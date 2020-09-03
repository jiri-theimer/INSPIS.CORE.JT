using System;
using System.Collections.Generic;

namespace UIFT.Models
{
    public class Sekce
    {
        public BO.f18FormSegment Base;

        /// <summary>
        /// tru pokud ma byt sekce skryta - neobsahuje zadne viditelne otazky
        /// </summary>
        public bool IsHidden { get; set; }

        public List<Sekce> SubSekce { get; set; }

        /// <summary>
        /// Pokud je true, znamena to ze RO validacni expression daneho segmentu byla vyhodnocena kladne - segment ma tedy byt RO.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Seznam vsech otazek v sekci
        /// </summary>
        public List<IOtazka> Otazky { get; set; }

        /// <summary>
        /// Prilohy k sekci
        /// </summary>
        public List<BO.o27Attachment> Prilohy { get; set; }

        /// <summary>
        /// Seznam best practices k segmentu
        /// </summary>
        //TODO bestPractices
        //public IEnumerable<BO.f30BestPracticesToSegment> BestPractices { get; set; }
        public IEnumerable<object> BestPractices { get; set; }
    }
}