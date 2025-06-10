namespace Zoe2PacTool.Support
{
    internal class SharedMethods
    {
        public static void ErrorExit(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Console.ReadLine();
            Environment.Exit(0);
        }


        public static void IfFileOrDirExistsDel(string fileOrDir, bool isFile)
        {
            if (isFile)
            {
                if (File.Exists(fileOrDir))
                {
                    File.Delete(fileOrDir);
                }
            }
            else
            {
                if (Directory.Exists(fileOrDir))
                {
                    Directory.Delete(fileOrDir, true);
                }
            }
        }


        private static readonly ushort[] ZLIBHeaderValues = new ushort[] { 0x0178, 0x5E78, 0x9C78, 0xDA78 };

        public static bool CheckZlibHeader(BinaryReader fileReader)
        {
            if (ZLIBHeaderValues.Contains(fileReader.ReadUInt16()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}