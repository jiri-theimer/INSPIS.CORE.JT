using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFT.Models
{
    public class Formular
    {
        public BO.f06Form Base;

        public List<Sekce> Sekce;

        public List<Sekce> Tree;

        public List<IOtazka> Otazky;

        public int AktualniSekce;

        public int AktualniOtazka;

        public List<BO.o27Attachment> Prilohy;
    }
}