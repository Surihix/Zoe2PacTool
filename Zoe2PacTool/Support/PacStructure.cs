namespace Zoe2PacTool.Support
{
    internal class PacStructure
    {
        // Header
        public readonly string PacMagic = "PAC";
        public readonly string Version1a = "0.01a";
        public readonly string Version1b = "0.01b";
        public string? VersionRead;
        public uint FileCount;

        // Header 0.0.1a
        public uint NameTableOffset1a;
        public uint FileTableOffset1a;
        public uint DataOffset1a;
        public uint Reserved1a;

        // Header 0.0.1b
        public ulong NameTableOffset1b;
        public ulong FileTableOffset1b;
        public ulong DataOffset1b;
        public ulong Reserved1b;


        // Name Table
        public string? FileName;


        // File Table 0.0.1a
        public uint DataSize1a;
        public uint DataStart1a;

        // File Table 0.0.1a
        public ulong DataSize1b;
        public ulong DataStart1b;

        
        // Custom
        public List<string> NamesList = new();
        public string? ExtractedPath;
        public string? PackedState;
        public string? UnpackedState;
    }
}