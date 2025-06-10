using Zoe2PacTool.Support;
using static Zoe2PacTool.Support.SharedEnums;

namespace Zoe2PacTool.Unpack
{
    internal class PacUnpack
    {
        #region Unpack type '-u'
        public static void UnpackFiles(string pacFile, GameVersions gameVersion)
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
                        using (var pacListWriter = new StreamWriter(Path.Combine(extractDir, "!!pacList.txt"), true))
                        {
                            pacListWriter.WriteLine($"PACVersion |:| {pacStructure.VersionRead}");
                            pacListWriter.WriteLine("");

                            fileTableReader.BaseStream.Position = 0;

                            for (int i = 0; i < pacStructure.FileCount; i++)
                            {
                                pacStructure.FileName = pacStructure.NamesList[i];
                                pacStructure.ExtractedPath = Path.Combine(extractDir, pacStructure.FileName);
                                pacListWriter.Write(pacStructure.FileName + " |:| ");

                                UnpackMethods.GetCurrentFileEntryData(fileTableReader, pacStructure);
                                UnpackMethods.UnpackProcess(pacStructure, pacReader, gameVersion);
                                pacListWriter.WriteLine(pacStructure.PackedState);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"Finished extracting {pacName}");
            Console.ReadLine();
        }
        #endregion


        #region Unpack type '-uf'
        public static void UnpackSingle(string pacFile, GameVersions gameVersion)
        {

        }
        #endregion


        #region Unpack type '-un'
        public static void UnpackFileNames(string pacFile, GameVersions gameVersion)
        {

        }
        #endregion
    }
}