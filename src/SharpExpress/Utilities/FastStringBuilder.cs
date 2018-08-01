using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Utilities
{
    /// <summary>
    /// Allocation free string builder for fast parsing binary data
    /// </summary>
    public unsafe class FastStringBuilder : IDisposable
    {
        private const int ChunkSize = 2048;
        private const int LC_NUMERIC = 4;

        private bool disposed_ = false;
        private List<char[]> chunks_ = new List<char[]>();
        private GCHandle currentChunkHandle_;
        private char* currentChunk_ = null;
        private int currentChunkIdx_ = 0;
        private int idx_ = 0;
        private IntPtr locale_;

        public FastStringBuilder()
        {
            locale_ = CreateLocale(LC_NUMERIC, "C");

            AllocateChunk();

            currentChunkHandle_ = GCHandle.Alloc(chunks_[0], GCHandleType.Pinned);

            currentChunk_ = (char*)currentChunkHandle_.AddrOfPinnedObject();
        }

        private void NextChunk()
        {
            currentChunkIdx_++;

            if (currentChunkIdx_ > chunks_.Count)
                AllocateChunk();

            currentChunkHandle_.Free();
            currentChunkHandle_ = GCHandle.Alloc(chunks_[currentChunkIdx_], GCHandleType.Pinned);

            currentChunk_ = (char*)currentChunkHandle_.AddrOfPinnedObject();
            idx_ = 0;
        }

        private void AllocateChunk()
        {
            chunks_.Add(new char[ChunkSize]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char character)
        {
            currentChunk_[idx_++] = character;

            if (RequireNextChunk())
                NextChunk();
        }

        public void Append(char* characters, int count)
        {
            for (char* ch = characters; ch < characters + count; ch++)
            {
                currentChunk_[idx_++] = *ch;

                if (RequireNextChunk())
                    NextChunk();
            }
        }

        public void Append(string str)
        {
            fixed (char* buffer = str)
            {
                char* ptr = buffer;
                do
                {
                    currentChunk_[idx_++] = *ptr++;

                    if (RequireNextChunk())
                        NextChunk();

                } while (ptr < buffer + str.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RequireNextChunk() => idx_ > ChunkSize - 1;


        public void Clear()
        {
            if (currentChunkIdx_ != 0)
            {
                currentChunkHandle_.Free();
                currentChunkHandle_ = GCHandle.Alloc(chunks_[0], GCHandleType.Pinned);
                currentChunk_ = (char*)currentChunkHandle_.AddrOfPinnedObject();
            }

            idx_ = 0;
            currentChunkIdx_ = 0;
        }

        public int Length => ((ChunkSize - 1) * currentChunkIdx_) + idx_;

        public double ParseDouble()
        {
            if (currentChunkIdx_ == 0)
                chunks_[0][idx_] = '\0';

            fixed (char* ptr = chunks_[0])
                return StringToDouble(ptr, locale_);
        }

        public unsafe int ParseInt()
        {
            if (currentChunkIdx_ > 0)
                throw new OverflowException();

            checked
            {
                int result = 0;

                fixed (char* chunk = chunks_[0])
                {
                    char* ch = chunk;
                    bool negative = false;
                    if (chunk[0] == '-')
                    {
                        negative = true;
                        ch++;
                    }
                    else if (chunk[0] == '+')
                        ch++;

                    for (; ch < chunk + idx_; ch++)
                    {
                        result *= 10;

                        int current = *ch - '0';
                        switch (current)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                                result += current;
                                break;
                            default:
                                throw new FormatException();
                        }
                    }

                    return negative ? -result : result;
                }
            }
        }

        public override string ToString()
        {
            if (currentChunkIdx_ == 0)
            {
                return new string(currentChunk_, 0, idx_);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public unsafe bool Equals(string str)
        {
            if (Length != str.Length)
                return false;

            fixed (char* buffer = str)
            {
                var chunk = 0;
                var chunkOffset = 0;
                char* end = buffer + str.Length;
                for (char* ch = buffer; ch < end; ch++)
                {
                    if (*ch != chunks_[chunk][chunkOffset++])
                        return false;

                    if (chunkOffset >= ChunkSize - 1)
                    {
                        chunk++;
                        chunkOffset = 0;
                    }
                }
            }

            return true;
        }

        public string ToStringInterned()
        {
            //TODO: intern trie
            return string.Intern(ToString());
        }

        [DllImport("msvcrt.dll", EntryPoint = "_wtof_l", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private extern unsafe static double StringToDouble(char* str, IntPtr locale);

        [DllImport("msvcrt.dll", EntryPoint = "_create_locale", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr CreateLocale(int category, string locale);

        [DllImport("msvcrt.dll", EntryPoint = "_free_locale", CallingConvention = CallingConvention.Cdecl)]
        private extern static void FreeLocale(IntPtr locale);

        #region IDisposable Support


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed_)
            {
                FreeLocale(locale_);

                disposed_ = true;
            }

            if(disposing)
            {
                currentChunkHandle_.Free();
            }
        }

        ~FastStringBuilder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
