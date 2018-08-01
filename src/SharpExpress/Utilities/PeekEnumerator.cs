using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Utilities
{
    public struct PeekEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> enumerator_;
        private bool afterFirst_;
        private bool hasNext_;
        private T next_;
        private T current_;

        public PeekEnumerator(IEnumerable<T> enumerable) : this()
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            enumerator_ = enumerable.GetEnumerator();

            Initialize();
        }

        public PeekEnumerator(IEnumerator<T> enumerator) : this()
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            enumerator_ = enumerator;

            Initialize();
        }

        private void Initialize()
        {
            afterFirst_ = false;

            if (hasNext_ = enumerator_.MoveNext())
                next_ = enumerator_.Current;
        }


        public T Current
        {
            get
            {
                if (!afterFirst_)
                    throw new InvalidOperationException();

                return current_;
            }
        }

        object IEnumerator.Current => Current;

        public bool TryPeek(out T result)
        {
            if (hasNext_)
            {
                result = next_;
                return true;
            }

            result = default(T);
            return false;
        }

        public void Dispose()
        {
            enumerator_.Dispose();
        }

        public bool MoveNext()
        {
            if (!hasNext_)
                return false;

            current_ = next_;
            if (hasNext_ = enumerator_.MoveNext())
            {
                next_ = enumerator_.Current;
            }

            afterFirst_ = true;

            return true;
        }

        public void Reset()
        {
            enumerator_.Reset();

            Initialize();
        }
    }
}
