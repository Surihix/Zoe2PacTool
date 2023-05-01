using System.Buffers.Binary;
using System.IO.Compression;

namespace Zoe2PacTool.PacCommon
{
    internal class CmnMethods
    {
        public static void ErrorExit(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void FileFolderExistsCheck(string inFileOrFolderVar, CmnEnums.ExistsCheckType existsCheckTypeVar)
        {
            switch (existsCheckTypeVar)
            {
                case CmnEnums.ExistsCheckType.file:
                    if (!File.Exists(inFileOrFolderVar))
                    {
                        ErrorExit("Specified file does not exist");
                    }
                    break;

                case CmnEnums.ExistsCheckType.folder:
                    if (!Directory.Exists(inFileOrFolderVar))
                    {
                        ErrorExit("Specified unpacked folder does not exist");
                    }
                    break;
            }
        }

        public static void ValidityCheck(string filePathVar)
        {
            using (FileStream checkFile = new(filePathVar, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader checkFileReader = new(checkFile))
                {
                    var zpacStr = "";
                    GenerateString(checkFileReader, 0, 4, ref zpacStr);

                    if (!zpacStr.Contains("PAC"))
                    {
                        ErrorExit("This is not a valid ZOE2 atlas pac file.");
                    }
                }
            }
        }

        public static void FileFolderExistsDel(string inFileFolderVar, CmnEnums.ExistsCheckType existsCheckTypeVar)
        {
            switch (existsCheckTypeVar)
            {
                case CmnEnums.ExistsCheckType.file:
                    if (File.Exists(inFileFolderVar))
                    {
                        File.Delete(inFileFolderVar);
                    }
                    break;

                case CmnEnums.ExistsCheckType.folder:
                    if (Directory.Exists(inFileFolderVar))
                    {
                        Directory.Delete(inFileFolderVar, true);
                    }
                    break;
            }
        }

        public static void GenerateString(BinaryReader readerNameVar, uint readerPosVar, int readCountVar, ref string outStringVar)
        {
            readerNameVar.BaseStream.Position = readerPosVar;
            var getStringChars = readerNameVar.ReadChars(readCountVar);
            outStringVar = string.Join("", getStringChars).Replace("\0", "");
        }

        public static void ReadBytes32(CmnEnums.Endianness endiannessVar, BinaryReader readerNameVar, uint readerPosVar, ref uint valToReadVar)
        {
            readerNameVar.BaseStream.Position = readerPosVar;
            var valToReadBytes = readerNameVar.ReadBytes((int)readerNameVar.BaseStream.Length);

            switch (endiannessVar)
            {
                case CmnEnums.Endianness.LittleEndian:
                    valToReadVar = BinaryPrimitives.ReadUInt32LittleEndian(valToReadBytes.AsSpan());
                    break;

                case CmnEnums.Endianness.BigEndian:
                    valToReadVar = BinaryPrimitives.ReadUInt32BigEndian(valToReadBytes.AsSpan());
                    break;
            }
        }

        public static void ReadBytes64(CmnEnums.Endianness endiannessVar, BinaryReader readerNameVar, ulong readerPosVar, ref ulong valToReadVar)
        {
            readerNameVar.BaseStream.Position = (long)readerPosVar;
            var valToReadBytes = readerNameVar.ReadBytes((int)readerNameVar.BaseStream.Length);

            switch (endiannessVar)
            {
                case CmnEnums.Endianness.LittleEndian:
                    valToReadVar = BinaryPrimitives.ReadUInt64LittleEndian(valToReadBytes.AsSpan());
                    break;

                case CmnEnums.Endianness.BigEndian:
                    valToReadVar = BinaryPrimitives.ReadUInt64BigEndian(valToReadBytes.AsSpan());
                    break;
            }
        }

        public static void DecompressData(CmnEnums.CompressionType cmpTypeVar, Stream streamToDecompress, Stream streamToHoldDcmpData)
        {
            switch (cmpTypeVar)
            {
                case CmnEnums.CompressionType.Deflate:
                    using (DeflateStream deflateDataDcmp = new(streamToDecompress, CompressionMode.Decompress))
                    {
                        deflateDataDcmp.CopyTo(streamToHoldDcmpData);
                    }
                    break;

                case CmnEnums.CompressionType.Zlib:
                    using (ZLibStream zlibDataDcmp = new(streamToDecompress, CompressionMode.Decompress))
                    {
                        zlibDataDcmp.CopyTo(streamToHoldDcmpData);
                    }
                    break;
            }
        }

        public static void CheckZlibHeader(BinaryReader readerNameVar, ulong readerPosVar, ref bool validateVar)
        {
            string[] ZlibHeadersList = { "7801", "785E", "789C", "78DA" };

            readerNameVar.BaseStream.Position = (long)readerPosVar;
            var getReadVar = readerNameVar.ReadBytes((int)readerNameVar.BaseStream.Length);
            var readZlibVal = BinaryPrimitives.ReadUInt16BigEndian(getReadVar.AsSpan());
            var foundZlibHeader = readZlibVal.ToString("X");

            if (ZlibHeadersList.Contains(foundZlibHeader))
            {
                validateVar = true;
            }
        }

        public static void WriteBytes32(CmnEnums.Endianness endiannessVar, BinaryWriter writerNameVar, uint writerPosVar, uint writeValueVar)
        {
            writerNameVar.BaseStream.Position = writerPosVar;
            var writeByteValVar = new byte[4];
            switch (endiannessVar)
            {
                case CmnEnums.Endianness.LittleEndian:
                    BinaryPrimitives.WriteUInt32LittleEndian(writeByteValVar, writeValueVar);
                    break;

                case CmnEnums.Endianness.BigEndian:
                    BinaryPrimitives.WriteUInt32BigEndian(writeByteValVar, writeValueVar);
                    break;
            }
            writerNameVar.Write(writeByteValVar);
        }

        public static void WriteBytes64(BinaryWriter writerNameVar, ulong writerPosVar, ulong writeValueVar)
        {
            writerNameVar.BaseStream.Position = (long)writerPosVar;
            var writeByteValVar = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(writeByteValVar, writeValueVar);
            writerNameVar.Write(writeByteValVar);
        }

        public static void CompressData(CmnEnums.CompressionType compressionTypeVar, Stream streamToCompress, Stream streamToHoldDcmpData)
        {
            switch (compressionTypeVar)
            {
                case CmnEnums.CompressionType.Zlib:
                    using (ZLibStream zlibCompressor = new(streamToHoldDcmpData, CompressionLevel.Optimal))
                    {
                        streamToCompress.CopyTo(zlibCompressor);
                    }
                    break;

                case CmnEnums.CompressionType.Deflate:
                    using (DeflateStream deflateCompressor = new(streamToHoldDcmpData, CompressionLevel.Optimal))
                    {
                        streamToCompress.CopyTo(deflateCompressor);
                    }
                    break;
            }
        }

        public static void WriteActualSize(CmnEnums.Endianness endiannessVar, BinaryWriter writerNameVar, ulong writerPosVar, uint writeValueVar)
        {
            writerNameVar.BaseStream.Position = (long)writerPosVar;
            var writeByteValVar = new byte[4];
            switch (endiannessVar)
            {
                case CmnEnums.Endianness.LittleEndian:
                    BinaryPrimitives.WriteUInt32LittleEndian(writeByteValVar, writeValueVar);
                    break;

                case CmnEnums.Endianness.BigEndian:
                    BinaryPrimitives.WriteUInt32BigEndian(writeByteValVar, writeValueVar);
                    break;
            }
            writerNameVar.Write(writeByteValVar);
        }
    }
}