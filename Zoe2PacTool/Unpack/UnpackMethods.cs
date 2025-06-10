using System.Text;
using Zoe2PacTool.Support;

namespace Zoe2PacTool
{
    internal class UnpackMethods
    {
        public static void GetHeaderData(BinaryReader pacReader, PacStructure pacStructure)
        {
            Console.WriteLine("Parsing header....");
            Console.WriteLine("");

            if (pacReader.BaseStream.Length < 12)
            {
                SharedMethods.ErrorExit("File is too small!");
            }

            pacReader.BaseStream.Position = 0;
            var pacMagic = pacReader.ReadBytesString(4, false);

            if (pacMagic != pacStructure.PacMagic)
            {
                SharedMethods.ErrorExit("Not a valid ZOE2 pac file!");
            }

            pacStructure.VersionRead = pacReader.ReadBytesString(5, false);

            if (pacStructure.VersionRead != pacStructure.Version1a)
            {
                if (pacStructure.VersionRead != pacStructure.Version1b)
                {
                    SharedMethods.ErrorExit("A valid version was not found in the pac file!");
                }
            }

            pacReader.BaseStream.Position += 3;

            if (pacStructure.VersionRead == pacStructure.Version1a)
            {
                pacStructure.FileCount = pacReader.ReadBytesUInt32(true);
                pacStructure.NameTableOffset1a = pacReader.ReadBytesUInt32(true);
                pacStructure.FileTableOffset1a = pacReader.ReadBytesUInt32(true);
                pacStructure.DataOffset1a = pacReader.ReadBytesUInt32(true);
                pacStructure.Reserved1a = pacReader.ReadBytesUInt32(true);
            }
            else
            {
                pacStructure.FileCount = pacReader.ReadUInt32();
                pacStructure.NameTableOffset1b = pacReader.ReadUInt64();
                pacStructure.FileTableOffset1b = pacReader.ReadUInt64();
                pacStructure.DataOffset1b = pacReader.ReadUInt64();
                pacStructure.Reserved1b = pacReader.ReadUInt64();
            }
        }


        public static void GetDataFromNameTable(BinaryReader pacReader, PacStructure pacStructure)
        {
            Console.WriteLine("Getting filenames....");
            Console.WriteLine("");

            for (int i = 0; i < pacStructure.FileCount; i++)
            {
                pacStructure.NamesList.Add(pacReader.ReadBytesString(32, false));
            }
        }


        public static void GetCurrentFileEntryData(BinaryReader fileTableReader, PacStructure pacStructure)
        {
            if (pacStructure.VersionRead == pacStructure.Version1a)
            {
                pacStructure.DataSize1a = fileTableReader.ReadBytesUInt32(true);
                pacStructure.DataStart1a = fileTableReader.ReadBytesUInt32(true);
            }
            else
            {
                pacStructure.DataSize1b = fileTableReader.ReadUInt64();
                pacStructure.DataStart1b = fileTableReader.ReadUInt64();
            }
        }


        public static void UnpackProcess(PacStructure pacStructure, BinaryReader pacReader, SharedEnums.GameCodes gameCode)
        {
            if (pacStructure.FileName.EndsWith("dat"))
            {
                pacStructure.ExtractedPath = pacStructure.ExtractedPath.Remove(pacStructure.ExtractedPath.Length - 3, 3);
            }

            if (pacStructure.VersionRead == pacStructure.Version1a)
            {
                pacReader.BaseStream.Seek(pacStructure.DataOffset1a + pacStructure.DataStart1a, SeekOrigin.Begin);

                if (gameCode == SharedEnums.GameCodes.hd)
                {
                    pacStructure.PackedState = "Compressed";
                    pacStructure.UnpackedState = "Decompressed";

                    pacReader.BaseStream.Position += 4;
                    var dcmpData = CompressionHelpers.DeflateDecompress(pacReader.ReadBytes((int)pacStructure.DataSize1a));

                    pacStructure.ExtractedPath += "dds";
                    SharedMethods.IfFileOrDirExistsDel(pacStructure.ExtractedPath, true);

                    using (var outFileStream = new FileStream(pacStructure.ExtractedPath, FileMode.Create, FileAccess.Write))
                    {
                        outFileStream.Write(dcmpData);
                    }
                }
                else
                {
                    UnpackFileMARS(pacReader, pacStructure, pacStructure.DataSize1a);
                }
            }
            else
            {
                pacReader.BaseStream.Seek((long)(pacStructure.DataOffset1b + pacStructure.DataStart1b), SeekOrigin.Begin);
                UnpackFileMARS(pacReader, pacStructure, pacStructure.DataSize1b);
            }

            Console.WriteLine($"Unpacked {pacStructure.FileName} ({pacStructure.UnpackedState})");
        }

        private static readonly ushort[] ZlibHeaders = new ushort[] { 0x0178, 0x5E78, 0x9C78, 0xDA78 };

        private static void UnpackFileMARS(BinaryReader pacReader, PacStructure pacStructure, ulong dataSize)
        {
            pacReader.BaseStream.Position += 4;

            if (ZlibHeaders.Contains(pacReader.ReadUInt16()))
            {
                pacReader.BaseStream.Position -= 2;
                pacStructure.PackedState = "Compressed";
                pacStructure.UnpackedState = "Decompressed";

                var dcmpData = CompressionHelpers.ZlibDecompress(pacReader.ReadBytes((int)dataSize - 4));

                if (pacStructure.FileName.EndsWith("dat"))
                {
                    pacStructure.ExtractedPath += DeriveDatExtension(dcmpData);
                }

                SharedMethods.IfFileOrDirExistsDel(pacStructure.ExtractedPath, true);

                using (var outFileStream = new FileStream(pacStructure.ExtractedPath, FileMode.CreateNew, FileAccess.Write))
                {
                    outFileStream.Write(dcmpData);
                }
            }
            else
            {
                pacReader.BaseStream.Position -= 6;
                pacStructure.PackedState = "Uncompressed";
                pacStructure.UnpackedState = "Copied";

                if (pacStructure.FileName.EndsWith("dat"))
                {
                    pacStructure.ExtractedPath += DeriveDatExtension(pacReader.ReadBytes(4));
                    pacReader.BaseStream.Position -= 4;
                }

                SharedMethods.IfFileOrDirExistsDel(pacStructure.ExtractedPath, true);

                using (var outFileStream = new FileStream(pacStructure.ExtractedPath, FileMode.CreateNew, FileAccess.Write))
                {
                    pacReader.BaseStream.CopyStreamTo(outFileStream, (long)dataSize, false);
                }
            }
        }

        private static string DeriveDatExtension(byte[] data)
        {
            var headerData = new byte[4];
            headerData[0] = data[0];
            headerData[1] = data[1];
            headerData[2] = data[2];
            headerData[3] = data[3];

            switch (Encoding.UTF8.GetString(headerData))
            {
                case "DDS ":
                    return "dds";

                case "GNF ":
                    return "gnf";

                default:
                    return "dat";
            }
        }
    }
}