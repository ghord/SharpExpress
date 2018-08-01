using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Utilities
{
    class ArrayPeekEnumerator<T> : IEnumerator<T>
    {
        private T[] array_;
        private int current_ = -1;

        public ArrayPeekEnumerator(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            array_ = array;

            Initialize();
        }

        private void Initialize()
        {
            current_ = -1;
        }

        public T Current => array_[current_];

        object IEnumerator.Current => Current;

        public bool TryPeek(out T result)
        {
            if (current_ + 1< array_.Length )
            {
                result = array_[current_ + 1];
                return true;
            }

            result = default(T);
            return false;
        }


        public bool TryPeek2(out T first, out T second)
        {
            if (current_ + 2< array_.Length)
            {
                first = array_[current_ + 1];
                second = array_[current_ + 2];
                return true;
            }

            first = second = default(T);
            return false;
        }

        public void Dispose()
        {
            array_ = null;
        }

        public bool MoveNext()
        {
            if (current_ + 1 < array_.Length)
            {
                current_++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            Initialize();
        }
    }
}
