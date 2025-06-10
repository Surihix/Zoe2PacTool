using System.Text.Json;
using Zoe2PacTool.Support;
using static Zoe2PacTool.Support.SharedEnums;

namespace Zoe2PacTool.Unpack
{
    internal class PacUnpack
    {
        #region Unpack type '-u'
        public static void UnpackFiles(string pacFile, GameCodes gameCode)
        {
            Console.WriteLine("Unpacking files....");
            Console.WriteLine("");

            var pacName = Path.GetFileName(pacFile);
            var extractDir = Path.Combine(Path.GetDirectoryName(pacFile), $"_{pacName}");
            SharedMethods.IfFileOrDirExistsDel(extractDir, false);
            Directory.CreateDirectory(extractDir);

            using (var pacReader = new BinaryReader(File.Open(pacFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var pacStructure = new PacStructure();

                UnpackMethods.GetHeaderData(pacReader, pacStructure);
                UnpackMethods.GetDataFromNameTable(pacReader, pacStructure);

                using (var fileTableStream = new MemoryStream())
                {
                    fileTableStream.Seek(0, SeekOrigin.Begin);
                    var fileTableSize = pacStructure.VersionRead == pacStructure.Version1a ? 8 * pacStructure.FileCount : 16 * pacStructure.FileCount;
                    pacReader.BaseStream.CopyStreamTo(fileTableStream, fileTableSize, false);

                    using (var fileTableReader = new BinaryReader(fileTableStream))
                    {
                        using (var pacEntriesJsonStream = new MemoryStream())
                        {
                            using (var pacEntriesJsonWriter = new Utf8JsonWriter(pacEntriesJsonStream, new JsonWriterOptions() { Indented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
                            {
                                pacEntriesJsonWriter.WriteStartObject();
                                pacEntriesJsonWriter.WriteString("version", pacStructure.VersionRead);
                                pacEntriesJsonWriter.WriteNumber("fileCount", pacStructure.FileCount);
                                pacEntriesJsonWriter.WriteStartArray("files");

                                fileTableReader.BaseStream.Position = 0;

                                for (int i = 0; i < pacStructure.FileCount; i++)
                                {
                                    pacStructure.FileName = pacStructure.NamesList[i];
                                    pacStructure.ExtractedPath = Path.Combine(extractDir, pacStructure.FileName);

                                    UnpackMethods.GetCurrentFileEntryData(fileTableReader, pacStructure);
                                    UnpackMethods.UnpackProcess(pacStructure, pacReader, gameCode);

                                    pacEntriesJsonWriter.WriteStartObject();
                                    pacEntriesJsonWriter.WriteString("fileName", pacStructure.FileName);
                                    pacEntriesJsonWriter.WriteString("extractedName", Path.GetFileName(pacStructure.ExtractedPath));
                                    pacEntriesJsonWriter.WriteString("packedState", pacStructure.PackedState);
                                    pacEntriesJsonWriter.WriteEndObject();
                                }

                                pacEntriesJsonWriter.WriteEndArray();
                                pacEntriesJsonWriter.WriteEndObject();
                            }

                            File.WriteAllBytes(Path.Combine(extractDir, "!!pacEntries.json"), pacEntriesJsonStream.ToArray());
                        }
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished extracting '{pacName}'");
            Console.ReadLine();
        }
        #endregion


        #region Unpack type '-uf'
        public static void UnpackSingle(string pacFile, string fileName, GameCodes gameCode)
        {
            Console.WriteLine("Unpacking file....");
            Console.WriteLine("");

            var pacName = Path.GetFileName(pacFile);
            var extractDir = Path.Combine(Path.GetDirectoryName(pacFile), $"_{pacName}");

            if (!Directory.Exists(extractDir))
            {
                Directory.CreateDirectory(extractDir);
            }

            using (var pacReader = new BinaryReader(File.Open(pacFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var pacStructure = new PacStructure();

                UnpackMethods.GetHeaderData(pacReader, pacStructure);
                UnpackMethods.GetDataFromNameTable(pacReader, pacStructure);

                using (var fileTableStream = new MemoryStream())
                {
                    fileTableStream.Seek(0, SeekOrigin.Begin);
                    var fileTableSize = pacStructure.VersionRead == pacStructure.Version1a ? 8 * pacStructure.FileCount : 16 * pacStructure.FileCount;
                    pacReader.BaseStream.CopyStreamTo(fileTableStream, fileTableSize, false);

                    using (var fileTableReader = new BinaryReader(fileTableStream))
                    {
                        fileTableReader.BaseStream.Position = 0;

                        for (int i = 0; i < pacStructure.FileCount; i++)
                        {
                            pacStructure.FileName = pacStructure.NamesList[i];
                            pacStructure.ExtractedPath = Path.Combine(extractDir, pacStructure.FileName);
                            UnpackMethods.GetCurrentFileEntryData(fileTableReader, pacStructure);

                            if (pacStructure.FileName == fileName)
                            {
                                UnpackMethods.UnpackProcess(pacStructure, pacReader, gameCode);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished extracting a file from '{pacName}'");
            Console.ReadLine();
        }
        #endregion


        #region Unpack type '-un'
        public static void UnpackNames(string pacFile, GameCodes gameVersion)
        {
            Console.WriteLine("Unpacking file....");
            Console.WriteLine("");

            var pacName = Path.GetFileName(pacFile);

            using (var pacReader = new BinaryReader(File.Open(pacFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var pacStructure = new PacStructure();

                UnpackMethods.GetHeaderData(pacReader, pacStructure);
                UnpackMethods.GetDataFromNameTable(pacReader, pacStructure);

                var pacNamesTxtFile = Path.Combine(Path.GetDirectoryName(pacFile), pacName + ".txt");
                SharedMethods.IfFileOrDirExistsDel(pacNamesTxtFile, true);

                using (var pacNamesWriter = new StreamWriter(pacNamesTxtFile, true))
                {
                    foreach (var name in pacStructure.NamesList)
                    {
                        pacNamesWriter.WriteLine(name);
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished unpacking names from '{pacName}'");
            Console.ReadLine();
        }
        #endregion
    }
}