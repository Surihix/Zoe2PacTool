using Zoe2PacTool.PacCommon;

namespace Zoe2PacTool.PacRepack
{
    internal class RepackHD
    {
        public static void ArchiveHd(string inFolderVar, string inPacFileVar)
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
                            break;

                        default:
                            CmnMethods.ErrorExit("Detected unknown version of pac file. this is not supported by this tool");
                            break;
                    }

                    uint fileNamesStartPos = 0;
                    CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReader, 16, ref fileNamesStartPos);

                    uint fileTableStartPos = 0;
                    CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReader, 20, ref fileTableStartPos);

                    uint fileDataStartPos = 0;
                    CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, pacFileReader, 24, ref fileDataStartPos);

                    using (FileStream newPacFileStream = new(newPacFile, FileMode.Append, FileAccess.Write))
                    {
                        pacFile.CopyTo(newPacFileStream, 0, fileDataStartPos);

                        using (BinaryWriter newPacFileWriter = new(newPacFileStream))
                        {
                            using (MemoryStream fileNames = new())
                            {
                                using (BinaryReader fileNamesReader = new(fileNames))
                                {
                                    var fileNamesSize = fileTableStartPos - fileNamesStartPos;
                                    pacFile.CopyTo(fileNames, fileNamesStartPos, fileNamesSize);

                                    using (MemoryStream fileTable = new())
                                    {
                                        using (BinaryReader fileTableReader = new(fileTable))
                                        {
                                            var fileTableSize = fileDataStartPos - fileTableStartPos;
                                            pacFile.CopyTo(fileTable, fileTableStartPos, fileTableSize);


                                            uint fileNamesReadPosInMem = 0;
                                            uint fileTableReadPosInMem = 0;
                                            uint fileSize = 0;
                                            uint fileStart = 0;
                                            uint newDataSize = 0;
                                            uint newDataStart = 0;
                                            for (int i = 0; i < totalFileCount; i++)
                                            {
                                                var currentFileName = "";
                                                CmnMethods.GenerateString(fileNamesReader, fileNamesReadPosInMem, 32, ref currentFileName);

                                                CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, fileTableReader, fileTableReadPosInMem, ref fileSize);
                                                CmnMethods.ReadBytes32(CmnEnums.Endianness.BigEndian, fileTableReader, fileTableReadPosInMem + 4, ref fileStart);

                                                var repackState = "(Compressed)";

                                                using (FileStream outDataChunkFile = new(tmpDataChunkFile, FileMode.Append, FileAccess.Write))
                                                {
                                                    using (BinaryWriter outDataChunkWriter = new(outDataChunkFile))
                                                    {
                                                        var currentTxtrFile = "";
                                                        currentTxtrFile = extractedDir + "\\" + Path.GetFileNameWithoutExtension(currentFileName) + ".dds";

                                                        var uncompressedSize = (uint)new FileInfo(currentTxtrFile).Length;
                                                        newDataStart = (uint)outDataChunkFile.Length;

                                                        using (FileStream txtrFileToCompress = new(currentTxtrFile, FileMode.Open, FileAccess.Read))
                                                        {
                                                            CmnMethods.WriteActualSize(CmnEnums.Endianness.BigEndian, outDataChunkWriter, newDataStart, uncompressedSize);
                                                            CmnMethods.CompressData(CmnEnums.CompressionType.Deflate, txtrFileToCompress, outDataChunkFile);
                                                            newDataSize = (uint)new FileInfo(tmpDataChunkFile).Length - newDataStart;
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
                            using (FileStream dataChunkToCopy = new(tmpDataChunkFile, FileMode.Open, FileAccess.Read))
                            {
                                dataChunkToCopy.CopyTo(newPacFileStream);
                            }
                            Console.WriteLine("Assembling complete");
                        }
                    }

                    File.Delete(tmpDataChunkFile);

                    Console.WriteLine("");
                    Console.WriteLine("Finished repacking files to " + Path.GetFileName(newPacFile));
                    Console.ReadLine();
                }
            }
        }
    }
}