using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fasetto.Word
{


        /// <summary>
        /// A generic base class that wraps and exposes a data model for API transport responses.
        /// </summary>
        public abstract class BaseModelResult<T> where T : new()
        {
            protected readonly T mElement;

            /// <summary>
            /// Exposes the underlying data model directly.
            /// </summary>
            public T Model => mElement;

            protected BaseModelResult()
            {
                mElement = new T();
            }

            protected BaseModelResult(T element)
            {
                mElement = element ?? new T();
            }
        }
    }
