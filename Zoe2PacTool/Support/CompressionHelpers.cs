using System.IO.Compression;

namespace Zoe2PacTool.Support
{
    internal class CompressionHelpers
    {
        public static byte[] DeflateDecompress(byte[] cmpData)
        {
            var dcmpData = Array.Empty<byte>();

            using (var cmpStream = new MemoryStream())
            {
                cmpStream.Seek(0, SeekOrigin.Begin);
                cmpStream.Write(cmpData, 0, cmpData.Length);
                cmpStream.Seek(0, SeekOrigin.Begin);

                using (var dcmpStream = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(cmpStream, CompressionMode.Decompress, true))
                    {
                        deflateStream.CopyTo(dcmpStream);
                    }

                    dcmpData = dcmpStream.ToArray();
                }
            }

            return dcmpData;
        }


        public static byte[] ZlibDecompress(byte[] cmpData)
        {
            var dcmpData = Array.Empty<byte>();

            using (var cmpStream = new MemoryStream())
            {
                cmpStream.Seek(0, SeekOrigin.Begin);
                cmpStream.Write(cmpData, 0, cmpData.Length);
                cmpStream.Seek(0, SeekOrigin.Begin);

                using (var dcmpStream = new MemoryStream())
                {
                    using (var deflateStream = new ZLibStream(cmpStream, CompressionMode.Decompress, true))
                    {
                        deflateStream.CopyTo(dcmpStream);
                    }

                    dcmpData = dcmpStream.ToArray();
                }
            }

            return dcmpData;
        }


        public static byte[] DeflateCompress(byte[] dataToCmp)
        {
            var cmpData = Array.Empty<byte>();

            using (var dataStream = new MemoryStream(dataToCmp))
            {
                using (var cmpStream = new MemoryStream())
                {
                    using (var compressor = new DeflateStream(cmpStream, CompressionLevel.Optimal, true))
                    {
                        dataStream.CopyTo(compressor);
                    }

                    cmpData = cmpStream.ToArray();
                }
            }

            return cmpData;
        }


        public static byte[] ZlibCompress(byte[] dataToCmp)
        {
            var cmpData = Array.Empty<byte>();

            using (var dataStream = new MemoryStream(dataToCmp))
            {
                using (var cmpStream = new MemoryStream())
                {
                    using (var compressor = new ZLibStream(cmpStream, CompressionLevel.Optimal, true))
                    {
                        dataStream.CopyTo(compressor);
                    }

                    cmpData = cmpStream.ToArray();
                }
            }

            return cmpData;
        }
    }
}