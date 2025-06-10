using System.Text;
using System.Text.Json;
using Zoe2PacTool.Support;
using static Zoe2PacTool.Support.SharedEnums;

namespace Zoe2PacTool.Repack
{
    internal class PacRepack
    {
        public static void RepackFiles(string extractDir, GameCodes gameCode)
        {
            Console.WriteLine("Repacking files....");
            Console.WriteLine("");

            var pacName = Path.GetFileName(extractDir).Remove(0, 1);
            var outPacFile = Path.Combine(Path.GetDirectoryName(extractDir), pacName);

            if (File.Exists(outPacFile))
            {
                Console.WriteLine("Backing up old pac file....");
                Console.WriteLine("");

                File.Move(outPacFile, outPacFile + ".old");
            }

            //
            Console.WriteLine("Deserializing json data....");
            Console.WriteLine("");

            var pacEntriesJsonFile = Path.Combine(extractDir, "!!pacEntries.json");

            if (!File.Exists(pacEntriesJsonFile))
            {
                SharedMethods.ErrorExit("Missing '!!pacEntries.json' file inside the extracted folder!");
            }

            var pacEntriesJsonReader = new Utf8JsonReader(File.ReadAllBytes(pacEntriesJsonFile), new JsonReaderOptions() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
            _ = pacEntriesJsonReader.Read();

            var pacStructure = new PacStructure();
            pacStructure.VersionRead = JsonReaderHelpers.GetString(ref pacEntriesJsonReader, "version");

            if (pacStructure.VersionRead != pacStructure.Version1a)
            {
                if (pacStructure.VersionRead != pacStructure.Version1b)
                {
                    SharedMethods.ErrorExit("Invalid version value specified in '!!pacEntries.json' file!");
                }
            }

            pacStructure.FileCount = JsonReaderHelpers.GetUInt32(ref pacEntriesJsonReader, "fileCount");
            JsonReaderHelpers.AdvanceToJsonArrayStart(ref pacEntriesJsonReader, "files");

            var pacEntriesDict = new Dictionary<int, (string, string, string)>();

            for (int i = 0; i < pacStructure.FileCount; i++)
            {
                _ = pacEntriesJsonReader.Read();

                pacStructure.FileName = JsonReaderHelpers.GetString(ref pacEntriesJsonReader, "fileName");
                pacStructure.ExtractedPath = JsonReaderHelpers.GetString(ref pacEntriesJsonReader, "extractedName");
                pacStructure.PackedState = JsonReaderHelpers.GetString(ref pacEntriesJsonReader, "packedState");
                pacEntriesDict.Add(i, new(pacStructure.FileName, pacStructure.ExtractedPath, pacStructure.PackedState));

                _ = pacEntriesJsonReader.Read();
            }

            //
            Console.WriteLine("Generating NameTable....");
            Console.WriteLine("");

            byte[] nameTableData;

            using (var nameTableStream = new MemoryStream())
            {
                using (var nameTableWriter = new BinaryWriter(nameTableStream))
                {
                    byte[] stringData;
                    int padLength;
                    for (int i = 0; i < pacStructure.FileCount; i++)
                    {
                        stringData = Encoding.UTF8.GetBytes(pacEntriesDict[i].Item1);
                        padLength = 32 - stringData.Length;

                        nameTableWriter.Write(stringData);
                        nameTableStream.PadNull(padLength);
                    }

                    nameTableData = nameTableStream.ToArray();
                }
            }

            //
            Console.WriteLine("Packing files....");
            Console.WriteLine("");

            byte[] fileTableData;

            using (var pacStream = new FileStream(outPacFile, FileMode.Append, FileAccess.Write))
            {
                using (var pacWriter = new BinaryWriter(pacStream))
                {
                    pacWriter.Write(Encoding.UTF8.GetBytes(pacStructure.PacMagic + "\0"));

                    uint fileTableSize;
                    if (pacStructure.VersionRead == pacStructure.Version1a)
                    {
                        pacWriter.Write(Encoding.UTF8.GetBytes(pacStructure.VersionRead + "\0\0\0"));
                        pacWriter.WriteBytesUInt32(pacStructure.FileCount, true);

                        pacStructure.NameTableOffset1a = 32;
                        pacStructure.FileTableOffset1a = (uint)(32 + nameTableData.Length);
                        fileTableSize = pacStructure.FileCount * 8;
                        pacStructure.DataOffset1a = pacStructure.FileTableOffset1a + fileTableSize;
                        pacStructure.Reserved1a = 0;

                        pacWriter.WriteBytesUInt32(pacStructure.NameTableOffset1a, true);
                        pacWriter.WriteBytesUInt32(pacStructure.FileTableOffset1a, true);
                        pacWriter.WriteBytesUInt32(pacStructure.DataOffset1a, true);
                        pacWriter.WriteBytesUInt32(pacStructure.Reserved1a, false);
                    }
                    else
                    {
                        pacWriter.Write(Encoding.UTF8.GetBytes(pacStructure.VersionRead + "\0\0\0"));
                        pacWriter.WriteBytesUInt32(pacStructure.FileCount, false);

                        pacStructure.NameTableOffset1b = 48;
                        pacStructure.FileTableOffset1b = (uint)(48 + nameTableData.Length);
                        fileTableSize = pacStructure.FileCount * 16;
                        pacStructure.DataOffset1b = pacStructure.FileTableOffset1b + fileTableSize;
                        pacStructure.Reserved1b = 0;

                        pacWriter.WriteBytesUInt64(pacStructure.NameTableOffset1b, false);
                        pacWriter.WriteBytesUInt64(pacStructure.FileTableOffset1b, false);
                        pacWriter.WriteBytesUInt64(pacStructure.DataOffset1b, false);
                        pacWriter.WriteBytesUInt64(pacStructure.Reserved1b, false);
                    }

                    pacStream.Write(nameTableData);
                    pacStream.PadNull(fileTableSize);

                    using (var fileTableStream = new MemoryStream())
                    {
                        using (var fileTableWriter = new BinaryWriter(fileTableStream))
                        {
                            string currentFile;
                            pacStructure.DataStart1a = 0;
                            pacStructure.DataStart1b = 0;
                            byte[] cmpData;

                            for (int i = 0; i < pacStructure.FileCount; i++)
                            {
                                currentFile = Path.Combine(extractDir, pacEntriesDict[i].Item2);

                                if (!File.Exists(currentFile))
                                {
                                    SharedMethods.ErrorExit($"Missing file '{pacEntriesDict[i].Item2}' in the extracted folder");
                                }

                                if (pacStructure.VersionRead == pacStructure.Version1a)
                                {
                                    // 0.01a Code
                                    if (gameCode == GameCodes.hd)
                                    {
                                        pacStructure.PackedState = "Compressed";
                                        cmpData = CompressionHelpers.DeflateCompress(File.ReadAllBytes(currentFile));
                                        pacStructure.DataSize1a = (uint)cmpData.Length + 4;

                                        pacWriter.WriteBytesUInt32((uint)new FileInfo(currentFile).Length, false);
                                        pacStream.Write(cmpData);

                                    }
                                    else
                                    {
                                        if (pacEntriesDict[i].Item3 == "Compressed")
                                        {
                                            pacStructure.PackedState = "Compressed";
                                            cmpData = CompressionHelpers.ZlibCompress(File.ReadAllBytes(currentFile));
                                            pacStructure.DataSize1a = (uint)cmpData.Length + 4;

                                            pacWriter.WriteBytesUInt32((uint)new FileInfo(currentFile).Length, false);
                                            pacStream.Write(cmpData);
                                        }
                                        else
                                        {
                                            pacStructure.PackedState = "Copied";
                                            pacStructure.DataSize1a = (uint)new FileInfo(currentFile).Length;

                                            using (var currentFileStream = new FileStream(currentFile, FileMode.Open, FileAccess.Read))
                                            {
                                                currentFileStream.CopyTo(pacStream);
                                            }
                                        }
                                    }

                                    fileTableWriter.WriteBytesUInt32(pacStructure.DataSize1a, true);
                                    fileTableWriter.WriteBytesUInt32(pacStructure.DataStart1a, true);
                                    pacStructure.DataStart1a += pacStructure.DataSize1a;
                                }
                                else
                                {
                                    // 0.01b Code
                                    if (pacEntriesDict[i].Item3 == "Compressed")
                                    {
                                        pacStructure.PackedState = "Compressed";
                                        cmpData = CompressionHelpers.ZlibCompress(File.ReadAllBytes(currentFile));
                                        pacStructure.DataSize1b = (uint)cmpData.Length + 4;

                                        pacWriter.WriteBytesUInt32((uint)new FileInfo(currentFile).Length, false);
                                        pacStream.Write(cmpData);
                                    }
                                    else
                                    {
                                        pacStructure.PackedState = "Copied";
                                        pacStructure.DataSize1b = (uint)new FileInfo(currentFile).Length;

                                        using (var currentFileStream = new FileStream(currentFile, FileMode.Open, FileAccess.Read))
                                        {
                                            currentFileStream.CopyTo(pacStream);
                                        }
                                    }

                                    fileTableWriter.WriteBytesUInt64(pacStructure.DataSize1b, false);
                                    fileTableWriter.WriteBytesUInt64(pacStructure.DataStart1b, false);
                                    pacStructure.DataStart1b += pacStructure.DataSize1b;
                                }

                                Console.WriteLine($"Repacked {pacEntriesDict[i].Item1} ({pacStructure.PackedState})");
                            }

                            fileTableData = fileTableStream.ToArray();
                        }
                    }
                }
            }

            //
            Console.WriteLine("");
            Console.WriteLine("Inserting fileTable....");
            Console.WriteLine("");

            using (var updPacStream = new FileStream(outPacFile, FileMode.Open, FileAccess.Write))
            {
                if (pacStructure.VersionRead == pacStructure.Version1a)
                {
                    updPacStream.Seek(pacStructure.FileTableOffset1a, SeekOrigin.Begin);
                }
                else
                {
                    updPacStream.Seek((long)pacStructure.FileTableOffset1b, SeekOrigin.Begin);
                }

                updPacStream.Write(fileTableData);
            }

            Console.WriteLine($"Finished repacking extracted directory to '{pacName}' file");
            Console.ReadLine();
        }
    }
}