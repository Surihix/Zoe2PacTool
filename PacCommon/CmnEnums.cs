namespace Zoe2PacTool.PacCommon
{
    internal class CmnEnums
    {
        public enum ExistsCheckType
        {
            file,
            folder
        }

        public enum VersionsList
        {
            mars,
            hd
        }

        public enum Endianness
        {
            LittleEndian,
            BigEndian
        }

        public enum CompressionType
        {
            Zlib,
            Deflate
        }
    }
}