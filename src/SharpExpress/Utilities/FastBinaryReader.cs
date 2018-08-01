using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public unsafe class FastBinaryReader : IDisposable
    {
        private Stream stream_;
        private bool multiThreaded_;
        const int BufferSize = 4095;
        const int BufferCount = 10;
        private int current_ = 1;
        private int length_;
        private GCHandle currentBufferHandle_;
        private byte* currentBufferPtr_;
        private byte[] buffer_ = new byte[BufferSize + 1];
        private BlockingCollection<byte[]> bufferPool_;
        private BlockingCollection<ArraySegment<byte>> buffers_;
#if DEBUG
        private int line_ = 1;
        private int column_ = 1;
#endif

        private Thread thread_;
        private CancellationTokenSource cts_;

        public FastBinaryReader(string path, bool multiThreaded = true)
            : this(new FileStream(path, FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite,
                        BufferSize,
                        FileOptions.SequentialScan), multiThreaded)
        {

        }

        public FastBinaryReader(Stream stream, bool multiThreaded = true)
        {
            stream_ = stream;
            multiThreaded_ = multiThreaded;

            AllocateBuffers();

            if (multiThreaded_)
                StartReadingThread();

            ReadNextChunk();
        }

        private void StartReadingThread()
        {
            cts_ = new CancellationTokenSource();

            thread_ = new Thread(() =>
            {
                try
                {
                    using (stream_)
                    {
                        while (!cts_.IsCancellationRequested)
                        {
                            var nextBuffer = bufferPool_.Take(cts_.Token);

                            int bytes = stream_.Read(nextBuffer, 0, BufferSize);

                            buffers_.Add(new ArraySegment<byte>(nextBuffer, 0, bytes));

                            if (bytes != BufferSize)
                                break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {

                }
            });

            thread_.Start();
        }

        private void AllocateBuffers()
        {
            currentBufferHandle_ = GCHandle.Alloc(buffer_, GCHandleType.Pinned);
            currentBufferPtr_ = (byte*)currentBufferHandle_.AddrOfPinnedObject();

            if (multiThreaded_)
            {
                bufferPool_ = new BlockingCollection<byte[]>();
                buffers_ = new BlockingCollection<ArraySegment<byte>>();

                for (int i = 0; i < BufferCount; i++)
                {
                    bufferPool_.Add(new byte[BufferSize]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Read()
        {
            if (current_ >= length_)
            {
                if (length_ == BufferSize)
                {
                    ReadNextChunk();
                    current_ = 0;
                }
                else if (current_ > length_)
                {
                    return -1;
                }
            }

            var result = currentBufferPtr_[current_++];

#if DEBUG
            if (result == '\n')
            {
                line_++;
                column_ = 0;
            }

            column_++;
#endif

            return result;
        }

#if DEBUG
        public (int line, int column) LineInfo => (line_, column_);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Peek()
        {
            if (current_ <= length_)
                return currentBufferPtr_[current_];

            return -1;
        }

        private void ReadNextChunk()
        {
            currentBufferPtr_[0] = currentBufferPtr_[BufferSize];

            if (multiThreaded_)
            {
                var chunk = buffers_.Take();
                length_ = chunk.Count;

                fixed (byte* chunkPtr = chunk.Array)
                {
                    Buffer.MemoryCopy(chunkPtr + chunk.Offset, currentBufferPtr_ + 1, BufferSize, length_);
                }

                bufferPool_.Add(chunk.Array);
            }
            else
            {
                length_ = stream_.Read(buffer_, 1, BufferSize);
            }
        }

        public void Dispose()
        {
            currentBufferHandle_.Free();

            if (multiThreaded_)
            {
                cts_.Cancel();
                thread_.Join();
            }
        }
    }
}
