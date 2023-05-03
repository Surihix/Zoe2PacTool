using Zoe2PacTool.PacCommon;

namespace Zoe2PacTool.PacRepack
{
    internal class PacRpkMARS
    {
        public static void ArchiveMars(string inFolderVar, string inPacFileVar)
        {
            var oldPacFilePath = Path.GetFullPath(inPacFileVar);
            var oldPacFileFolder = Path.GetDirectoryName(oldPacFilePath);
            var extractedDir = oldPacFileFolder + "\\" + Path.GetFileName(inFolderVar);
            var newPacFile = oldPacFileFolder + "\\" + Path.GetFileName(inPacFileVar) + ".new";
            var tmpDataChunkFile = extractedDir + "\\!TmpDataChunkFile";

            Console.WriteLine("Repacking....");
            Console.WriteLine("");

            CmnMethods.FileFolderExistsDel(newPacFile, CmnEnums.ExistsCheckType.file);
            CmnMethods.FileFolderExistsDel(tmpDataChunkFile, CmnEnums.ExistsCheckType.file);

            using (FileStream pacFile = new(inPacFileVar, FileMode.Open, FileAccess.Read))
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
                            Archive1a(pacFile, pacFileReader, newPacFile, totalFileCount, tmpDataChunkFile, extractedDir);
                            break;

                        case "0.01b":
                            CmnMethods.ReadBytes32(CmnEnums.Endianness.LittleEndian, pacFileReader, 12, ref totalFileCount);
                            Archive1b(pacFile, pacFileReader, newPacFile, totalFileCount, tmpDataChunkFile, extractedDir);
                            break;
                    }
                }
            }

            File.Delete(tmpDataChunkFile);

            Console.WriteLine("");
            Console.WriteLine("Finished repacking files to " + Path.GetFileName(newPacFile));
            Console.ReadLine();
        }

        static void Archive1a(FileStream pacFileStreamVar, BinaryReader pacFileReaderVar, string newPacFileVar, uint totalFileCountVar, string tmpDataChunkFileVar, string extractedDirVar)
        {
            uint fileNamesStartPos = 0;
            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReaderVar, 16, ref fileNamesStartPos);

            uint fileTableStartPos = 0;
            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReaderVar, 20, ref fileTableStartPos);

            uint fileDataStartPos = 0;
            CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReaderVar, 24, ref fileDataStartPos);

            using (FileStream newPacFileStream = new(newPacFileVar, FileMode.Append, FileAccess.Write))
            {
                pacFileStreamVar.CopyTo(newPacFileStream, 0, fileDataStartPos);

                using (BinaryWriter newPacFileWriter = new(newPacFileStream))
                {
                    using (MemoryStream fileNames = new())
                    {
                        using (BinaryReader fileNamesReader = new(fileNames))
                        {
                            var fileNamesSize = fileTableStartPos - fileNamesStartPos;
                            pacFileStreamVar.CopyTo(fileNames, fileNamesStartPos, fileNamesSize);

                            Console.WriteLine("Checking extracted files....");
                            string[] extractedDirToCheck = Directory.GetFiles(extractedDirVar, "*.*", SearchOption.TopDirectoryOnly);
                            
                            CmnMethods.CheckExtractedFiles(extractedDirToCheck, totalFileCountVar, fileNamesReader);
                            Console.WriteLine("Finished checking extracted files");
                            Console.WriteLine("");

                            using (MemoryStream fileTable = new())
                            {
                                using (BinaryReader fileTableReader = new(fileTable))
                                {
                                    var fileTableSize = fileDataStartPos - fileTableStartPos;
                                    pacFileStreamVar.CopyTo(fileTable, fileTableStartPos, fileTableSize);


                                    uint fileNamesReadPosInMem = 0;
                                    uint fileTableReadPosInMem = 0;
                                    uint fileSize = 0;
                                    uint fileStart = 0;
                                    uint newDataSize = 0;
                                    uint newDataStart = 0;
                                    for (int i = 0; i < totalFileCountVar; i++)
                                    {
                                        var currentFileName = "";
                                        CmnMethods.GenerateString(fileNamesReader, fileNamesReadPosInMem, 32, ref currentFileName);

                                        CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, fileTableReader, fileTableReadPosInMem, ref fileSize);
                                        CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, fileTableReader, fileTableReadPosInMem + 4, ref fileStart);

                                        var repackState = "(Copied)";

                                        using (FileStream outDataChunkFile = new(tmpDataChunkFileVar, FileMode.Append, FileAccess.Write))
                                        {
                                            using (BinaryWriter outDataChunkWriter = new(outDataChunkFile))
                                            {

                                                if (currentFileName.EndsWith(".dat"))
                                                {
                                                    var currentTxtrFile = "";
                                                    currentTxtrFile = extractedDirVar + "\\" + Path.GetFileNameWithoutExtension(currentFileName) + ".dds";

                                                    if (!File.Exists(currentTxtrFile))
                                                    {
                                                        currentTxtrFile = extractedDirVar + "\\" + Path.GetFileNameWithoutExtension(currentFileName) + ".gnf";
                                                    }

                                                    bool isZlibHeader = false;
                                                    CmnMethods.CheckZlibHeader(pacFileReaderVar, fileDataStartPos + fileStart + 4, ref isZlibHeader);

                                                    var uncompressedSize = (uint)new FileInfo(currentTxtrFile).Length;
                                                    newDataStart = (uint)outDataChunkFile.Length;

                                                    switch (isZlibHeader)
                                                    {
                                                        case true:
                                                            repackState = "(Compressed)";
                                                            using (FileStream txtrFileToCompress = new(currentTxtrFile, FileMode.Open, FileAccess.Read))
                                                            {
                                                                CmnMethods.WriteActualSize(CmnEnums.Endianness.LittleEndian, outDataChunkWriter, newDataStart, uncompressedSize);
                                                                CmnMethods.CompressData(CmnEnums.CompressionType.Zlib, txtrFileToCompress, outDataChunkFile);
                                                                newDataSize = (uint)new FileInfo(tmpDataChunkFileVar).Length - newDataStart;
                                                            }
                                                            break;

                                                        case false:
                                                            using (FileStream txtrFileToCopy = new(currentTxtrFile, FileMode.Open, FileAccess.Read))
                                                            {
                                                                txtrFileToCopy.CopyTo(outDataChunkFile, 0, uncompressedSize);
                                                                newDataSize = (uint)new FileInfo(currentTxtrFile).Length;

                                                            }
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    using (FileStream fileToCopy = new(extractedDirVar + "\\" + currentFileName, FileMode.Open, FileAccess.Read))
                                                    {
                                                        newDataStart = (uint)outDataChunkFile.Length;

                                                        fileToCopy.Seek(0, SeekOrigin.Begin);
                                                        fileToCopy.CopyTo(outDataChunkFile);

                                                        newDataSize = (uint)new FileInfo(extractedDirVar + "\\" + currentFileName).Length;
                                                    }
                                                }
                                            }
                                        }

                                        CmnMethods.WriteBytes32(CmnEnums.Endianness.BigEndian, newPacFileWriter, fileTableStartPos, newDataSize);
                                        CmnMethods.WriteBytes32(CmnEnums.Endianness.BigEndian, newPacFileWriter, fileTableStartPos + 4, newDataStart);

                                        fileTableStartPos += 8;
                                        fileTableReadPosInMem += 8;
                                        fileNamesReadPosInMem += 32;

                                        Console.WriteLine("Repacked " + currentFileName + " " + repackState);
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Assembling archive....");
                    using (FileStream dataChunkToCopy = new(tmpDataChunkFileVar, FileMode.Open, FileAccess.Read))
                    {
                        dataChunkToCopy.CopyTo(newPacFileStream);
                    }
                    Console.WriteLine("Assembling complete");
                }
            }
        }

        static void Archive1b(FileStream pacFileStreamVar, BinaryReader pacFileReaderVar, string newPacFileVar, uint totalFileCountVar, string tmpDataChunkFileVar, string extractedDirVar)
        {
            ulong fileNamesStartPos = 0;
            CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, pacFileReaderVar, 16, ref fileNamesStartPos);

            ulong fileTableStartPos = 0;
            CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, pacFileReaderVar, 24, ref fileTableStartPos);

            ulong fileDataStartPos = 0;
            CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, pacFileReaderVar, 32, ref fileDataStartPos);

            using (FileStream newPacFileStream = new(newPacFileVar, FileMode.Append, FileAccess.Write))
            {
                pacFileStreamVar.CopyTo(newPacFileStream, 0, (long)fileDataStartPos);

                using (BinaryWriter newPacFileWriter = new(newPacFileStream))
                {
                    using (MemoryStream fileNames = new())
                    {
                        using (BinaryReader fileNamesReader = new(fileNames))
                        {
                            var fileNamesSize = fileTableStartPos - fileNamesStartPos;
                            pacFileStreamVar.CopyTo(fileNames, (long)fileNamesStartPos, (long)fileNamesSize);

                            Console.WriteLine("Checking extracted files....");
                            string[] extractedDirToCheck = Directory.GetFiles(extractedDirVar, "*.*", SearchOption.TopDirectoryOnly);

                            CmnMethods.CheckExtractedFiles(extractedDirToCheck, totalFileCountVar, fileNamesReader);
                            Console.WriteLine("Finished checking extracted files");
                            Console.WriteLine("");

                            using (MemoryStream fileTable = new())
                            {
                                using (BinaryReader fileTableReader = new(fileTable))
                                {
                                    var fileTableSize = fileDataStartPos - fileTableStartPos;
                                    pacFileStreamVar.CopyTo(fileTable, (long)fileTableStartPos, (long)fileTableSize);


                                    ulong fileNamesReadPosInMem = 0;
                                    ulong fileTableReadPosInMem = 0;
                                    ulong fileSize = 0;
                                    ulong fileStart = 0;
                                    ulong newDataSize = 0;
                                    ulong newDataStart = 0;
                                    for (int i = 0; i < totalFileCountVar; i++)
                                    {
                                        var currentFileName = "";
                                        CmnMethods.GenerateString(fileNamesReader, (uint)fileNamesReadPosInMem, 32, ref currentFileName);

                                        CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, fileTableReader, fileTableReadPosInMem, ref fileSize);
                                        CmnMethods.ReadBytes64(CmnEnums.Endianness.LittleEndian, fileTableReader, fileTableReadPosInMem + 8, ref fileStart);

                                        var repackState = "(Copied)";

                                        using (FileStream outDataChunkFile = new(tmpDataChunkFileVar, FileMode.Append, FileAccess.Write))
                                        {
                                            using (BinaryWriter outDataChunkWriter = new(outDataChunkFile))
                                            {

                                                if (currentFileName.EndsWith(".dat"))
                                                {
                                                    var currentTxtrFile = "";
                                                    currentTxtrFile = extractedDirVar + "\\" + Path.GetFileNameWithoutExtension(currentFileName) + ".dds";

                                                    if (!File.Exists(currentTxtrFile))
                                                    {
                                                        currentTxtrFile = extractedDirVar + "\\" + Path.GetFileNameWithoutExtension(currentFileName) + ".gnf";
                                                    }

                                                    bool isZlibHeader = false;
                                                    CmnMethods.CheckZlibHeader(pacFileReaderVar, fileDataStartPos + fileStart + 4, ref isZlibHeader);

                                                    var uncompressedSize = (uint)new FileInfo(currentTxtrFile).Length;
                                                    newDataStart = (ulong)outDataChunkFile.Length;

                                                    switch (isZlibHeader)
                                                    {
                                                        case true:
                                                            repackState = "(Compressed)";
                                                            using (FileStream txtrFileToCompress = new(currentTxtrFile, FileMode.Open, FileAccess.Read))
                                                            {
                                                                CmnMethods.WriteActualSize(CmnEnums.Endianness.LittleEndian, outDataChunkWriter, newDataStart, uncompressedSize);
                                                                CmnMethods.CompressData(CmnEnums.CompressionType.Zlib, txtrFileToCompress, outDataChunkFile);
                                                                newDataSize = (ulong)new FileInfo(tmpDataChunkFileVar).Length - newDataStart;
                                                            }
                                                            break;

                                                        case false:
                                                            using (FileStream txtrFileToCopy = new(currentTxtrFile, FileMode.Open, FileAccess.Read))
                                                            {
                                                                txtrFileToCopy.CopyTo(outDataChunkFile, 0, uncompressedSize);
                                                                newDataSize = (ulong)new FileInfo(currentTxtrFile).Length;
                                                            }
                                                            break;
                                                    }

                                                }
                                                else
                                                {
                                                    using (FileStream fileToCopy = new(extractedDirVar + "\\" + currentFileName, FileMode.Open, FileAccess.Read))
                                                    {
                                                        newDataStart = (ulong)outDataChunkFile.Length;

                                                        fileToCopy.Seek(0, SeekOrigin.Begin);
                                                        fileToCopy.CopyTo(outDataChunkFile);

                                                        newDataSize = (ulong)new FileInfo(extractedDirVar + "\\" + currentFileName).Length;
                                                    }
                                                }
                                            }
                                        }

                                        CmnMethods.WriteBytes64(newPacFileWriter, fileTableStartPos, newDataSize);
                                        CmnMethods.WriteBytes64(newPacFileWriter, fileTableStartPos + 8, newDataStart);

                                        fileTableStartPos += 16;
                                        fileTableReadPosInMem += 16;
                                        fileNamesReadPosInMem += 32;

                                        Console.WriteLine("Repacked " + currentFileName + " " + repackState);
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Assembling archive....");
                    using (FileStream dataChunkToCopy = new(tmpDataChunkFileVar, FileMode.Open, FileAccess.Read))
                    {
                        dataChunkToCopy.CopyTo(newPacFileStream);
                    }
                    Console.WriteLine("Assembling complete");
                }
            }
        }
    }
}