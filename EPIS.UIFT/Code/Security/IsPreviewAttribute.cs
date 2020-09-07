using System;

namespace UIFT
{
    /// <summary>
    /// Pokud je nastaven atribut na true, jedna se o akci v rezimu preview
    /// </summary>
    public class IsPreviewAttribute : Attribute
    {
        public IsPreviewAttribute()
        {
        }
    }
}