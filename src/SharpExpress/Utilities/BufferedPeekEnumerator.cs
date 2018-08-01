using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Utilities
{
    public class BufferedPeekEnumerator<T> : IEnumerator<T>
    {
        private T[] buffer_ = new T[] { default };
        private int currentIndex_;
        private int currentSegmentLength_;
        private IEnumerator<ArraySegment<T>> enumerator_;

        public BufferedPeekEnumerator(IEnumerator<ArraySegment<T>> enumerator)
        {
            enumerator_ = enumerator;

            ReadNextBuffer();
        }

        private bool ReadNextBuffer()
        {
            var lastItem = buffer_[currentSegmentLength_];

            if (enumerator_.MoveNext())
            {
                var newSegment = enumerator_.Current;
                currentSegmentLength_ = newSegment.Count;

                ResizeBufferIfNeeded();

                Array.Copy(newSegment.Array, 0, buffer_, 1, newSegment.Count);
                buffer_[0] = lastItem;
                currentIndex_ = 0;
                return true;
            }
            else
            {
                currentSegmentLength_ = 0;
                return false;
            }
        }

        private void ResizeBufferIfNeeded()
        {
            if (buffer_.Length <= currentSegmentLength_)
            {
                buffer_ = new T[currentSegmentLength_ + 1];
            }
        }

        public T Current => buffer_[currentIndex_];

        object IEnumerator.Current => buffer_[currentIndex_];

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            if (currentIndex_ < currentSegmentLength_)
            {
                currentIndex_++;
                return true;
            }
            else if (currentSegmentLength_ == 0)
            {
                return false;
            }
            else
                return ReadNextBuffer();
        }

        public void Reset()
        {
            enumerator_.Reset();
        }
    }
}
