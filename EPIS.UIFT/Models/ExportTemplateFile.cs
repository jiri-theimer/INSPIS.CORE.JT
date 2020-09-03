using System;

namespace UIFT.Models
{
    /// <summary>
    /// Informace o sablone pro exportovani dotazniku
    /// </summary>
    public class ExportTemplateFile
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }
    }
}