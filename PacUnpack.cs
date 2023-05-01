using Zoe2PacTool.PacCommon;

namespace Zoe2PacTool
{
    internal class PacUnpack
    {
        public static void ArchiveUnpack(string inFileVar, CmnEnums.VersionsList gameVersionVar)
        {
            var filePath = Path.GetFullPath(inFileVar);
            var extractDir = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(inFileVar);

            Console.WriteLine("Unpacking....");
            Console.WriteLine("");

            CmnMethods.FileFolderExistsDel(extractDir, CmnEnums.ExistsCheckType.folder);
            Directory.CreateDirectory(extractDir);

            using (FileStream pacFile = new(inFileVar, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader pacFileReader = new(pacFile))
                {
                    uint totalFileCount = 0;

                    var versionId = "";
                    CmnMethods.GenerateString(pacFileReader, 4, 5, ref versionId);

                    switch (versionId)
                    {
                        case "0.01a":
                            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReader, 12, ref totalFileCount);
                            Archive1a(pacFile, pacFileReader, totalFileCount, gameVersionVar, extractDir);
                            break;

                        case "0.01b":
                            CmnMethods.ReadBytes32(CmnEnums.Endianness.LittleEndian, pacFileReader, 12, ref totalFileCount);
                            Archive1b(pacFile, pacFileReader, totalFileCount, extractDir);
                            break;
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Adding texture extensions....");

            string[] extractedTxtrDir = Directory.GetFiles(extractDir, "*.dat", SearchOption.TopDirectoryOnly);
            foreach (var txtr in extractedTxtrDir)
            {
                var foundHeader = "";
                using (FileStream currentTxtr = new(txtr, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader currentTxtrReader = new(currentTxtr))
                    {
                        CmnMethods.GenerateString(currentTxtrReader, 0, 4, ref foundHeader);
                    }
                }

                switch (foundHeader)
                {
                    case "DDS ":
                        File.Move(txtr, extractDir + "\\" + Path.GetFileNameWithoutExtension(txtr) + ".dds");
                        break;

                    case "GNF ":
                        File.Move(txtr, extractDir + "\\" + Path.GetFileNameWithoutExtension(txtr) + ".gnf");
                        break;
                }
            }

            Console.WriteLine("Added texture extensions");

            Console.WriteLine("");
            Console.WriteLine("Finished extracting " + Path.GetFileName(inFileVar));
            Console.ReadLine();
        }

        static void Archive1a(FileStream pacStreamNameVar, BinaryReader pacReaderNameVar, uint totalFileCountVar, CmnEnums.VersionsList gameVersionVar, string extractDirVar)
        {
            uint fileNamesStartPos = 0;
            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacReaderNameVar, 16, ref fileNamesStartPos);

            uint fileTableStartPos = 0;
            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacReaderNameVar, 20, ref fileTableStartPos);

            uint fileDataStartPos = 0;
            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacReaderNameVar, 24, ref fileDataStartPos);

            using (MemoryStream fileNamesMem = new())
            {
                using (BinaryReader fileNamesReader = new(fileNamesMem))
                {
                    var fileNamesSize = fileTableStartPos - fileNamesStartPos;
                    pacStreamNameVar.CopyTo(fileNamesMem, fileNamesStartPos, fileNamesSize);

                    using (MemoryStream fileTableMem = new())
                    {
                        using (BinaryReader fileTableReader = new(fileTableMem))
                        {
                            var fileTableSize = fileDataStartPos - fileTableStartPos;
                            pacStreamNameVar.CopyTo(fileTableMem, fileTableStartPos, fileTableSize);


                            uint fileNamesReadPos = 0;
                            uint fileTableReadPos = 0;
                            uint fileDataReadPos = fileDataStartPos;
                            uint fileSize = 0;
                            uint fileStart = 0;
                            for (int i = 0; i < totalFileCountVar; i++)
                            {
                                var currentFileName = "";
                                CmnMethods.GenerateString(fileNamesReader, fileNamesReadPos, 32, ref currentFileName);
                                CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, fileTableReader, fileTableReadPos, ref fileSize);
                                CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, fileTableReader, fileTableReadPos + 4, ref fileStart);

                                var extractedState = "(Copied)";

                                using (FileStream outFile = new(extractDirVar + "\\" + currentFileName, FileMode.CreateNew, FileAccess.Write))
                                {
                                    using (MemoryStream tempDataHolder = new())
                                    {
                                        using (BinaryReader tempDataHolderReader = new(tempDataHolder))
                                        {
                                            pacStreamNameVar.CopyTo(tempDataHolder, fileDataReadPos + fileStart, fileSize);

                                            switch (gameVersionVar)
                                            {
                                                case CmnEnums.VersionsList.mars:
                                                    bool isZlibHeader = false;
                                                    CmnMethods.CheckZlibHeader(pacReaderNameVar, fileDataStartPos + fileStart + 4, ref isZlibHeader);
                                                    switch (isZlibHeader)
                                                    {
                                                        case true:
                                                            extractedState = "(Decompressed)";
                                                            using (MemoryStream cmpDataStream = new())
                                                            {
                                                                tempDataHolder.Seek(4, SeekOrigin.Begin);
                                                                tempDataHolder.CopyTo(cmpDataStream);

                                                                cmpDataStream.Seek(0, SeekOrigin.Begin);
                                                                CmnMethods.DecompressData(CmnEnums.CompressionType.Zlib, cmpDataStream, outFile);
                                                            }
                                                            break;

                                                        case false:
                                                            tempDataHolder.CopyTo(outFile, 0, fileSize);
                                                            break;
                                                    }
                                                    break;

                                                case CmnEnums.VersionsList.hd:
                                                    extractedState = "(Decompressed)";
                                                    using (MemoryStream cmpDataStream = new())
                                                    {
                                                        tempDataHolder.Seek(4, SeekOrigin.Begin);
                                                        tempDataHolder.CopyTo(cmpDataStream);

                                                        cmpDataStream.Seek(0, SeekOrigin.Begin);
                                                        CmnMethods.DecompressData(CmnEnums.CompressionType.Deflate, cmpDataStream, outFile);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }

                                Console.WriteLine("Unpacked " + currentFileName + " " + extractedState);

                                fileNamesReadPos += 32;
                                fileTableReadPos += 8;
                            }
                        }
                    }
                }
            }
        }

        static void Archive1b(FileStream pacStreamNameVar, BinaryReader pacReaderNameVar, uint totalFileCountVar, string extractDirVar)
        {
            ulong fileNamesStartPos = 0;
            CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, pacReaderNameVar, 16, ref fileNamesStartPos);

            ulong fileTableStartPos = 0;
            CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, pacReaderNameVar, 24, ref fileTableStartPos);

            ulong fileDataStartPos = 0;
            CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, pacReaderNameVar, 32, ref fileDataStartPos);

            using (MemoryStream fileNamesMem = new())
            {
                using (BinaryReader fileNamesReader = new(fileNamesMem))
                {
                    var fileNamesSize = fileTableStartPos - fileNamesStartPos;
                    pacStreamNameVar.CopyTo(fileNamesMem, (long)fileNamesStartPos, (long)fileNamesSize);

                    using (MemoryStream fileTableMem = new())
                    {
                        using (BinaryReader fileTableReader = new(fileTableMem))
                        {
                            var fileTableSize = fileDataStartPos - fileTableStartPos;
                            pacStreamNameVar.CopyTo(fileTableMem, (long)fileTableStartPos, (long)fileTableSize);


                            uint fileNamesReadPos = 0;
                            uint fileTableReadPos = 0;
                            ulong fileDataReadPos = fileDataStartPos;
                            ulong fileSize = 0;
                            ulong fileStart = 0;
                            for (int i = 0; i < totalFileCountVar; i++)
                            {
                                var currentFileName = "";
                                CmnMethods.GenerateString(fileNamesReader, fileNamesReadPos, 32, ref currentFileName);
                                CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, fileTableReader, fileTableReadPos, ref fileSize);
                                CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, fileTableReader, fileTableReadPos + 8, ref fileStart);

                                var extractedState = "(Copied)";

                                using (FileStream outFile = new(extractDirVar + "\\" + currentFileName, FileMode.CreateNew, FileAccess.Write))
                                {
                                    using (MemoryStream tempDataHolder = new())
                                    {
                                        using (BinaryReader tempDataHolderReader = new(tempDataHolder))
                                        {
                                            pacStreamNameVar.CopyTo(tempDataHolder, (long)fileDataReadPos + (long)fileStart, (long)fileSize);

                                            bool isZlibHeader = false;
                                            CmnMethods.CheckZlibHeader(pacReaderNameVar, fileDataStartPos + fileStart + 4, ref isZlibHeader);

                                            switch (isZlibHeader)
                                            {
                                                case true:
                                                    extractedState = "(Decompressed)";
                                                    using (MemoryStream cmpDataStream = new())
                                                    {
                                                        tempDataHolder.Seek(4, SeekOrigin.Begin);
                                                        tempDataHolder.CopyTo(cmpDataStream);

                                                        cmpDataStream.Seek(0, SeekOrigin.Begin);
                                                        CmnMethods.DecompressData(CmnEnums.CompressionType.Zlib, cmpDataStream, outFile);
                                                    }
                                                    break;

                                                case false:
                                                    tempDataHolder.CopyTo(outFile, 0, (uint)fileSize);
                                                    break;
                                            }
                                        }
                                    }
                                }

                                Console.WriteLine("Unpacked " + currentFileName + " " + extractedState);

                                fileNamesReadPos += 32;
                                fileTableReadPos += 16;
                            }
                        }
                    }
                }
            }
        }
    }
}