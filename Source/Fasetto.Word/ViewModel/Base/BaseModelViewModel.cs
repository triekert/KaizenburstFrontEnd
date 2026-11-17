using System;
using System.Collections.Generic;
using Fasetto.Word.Core;

namespace Fasetto.Word
{
    /// <summary>
    /// A generic base view model that wraps and exposes a data model instance.
    /// </summary>
    public abstract class BaseModelViewModel<T> : BaseViewModel where T : new()
    {
        protected readonly T mElement;

        /// <summary>
        /// Exposes the underlying data model directly for XAML data binding.
        /// </summary>
        public T Model => mElement;

        protected BaseModelViewModel()
        {
            mElement = new T();
        }

        protected BaseModelViewModel(T element)
        {
            mElement = element ?? new T();
        }
    }
}