﻿using System;

namespace UIFT
{
    /// <summary>
    /// Pokud je nastaven atribut na true, jedna se o akci v rezimu preview
    /// </summary>
    public class IsPreviewAttribute : Attribute
    {
        private bool _preview;

        public IsPreviewAttribute(bool isPreview)
        {
            _preview = isPreview;
        }

        public bool Preview
        {
            get { return _preview; }
        }
    }
}