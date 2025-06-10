namespace Zoe2PacTool
{
    internal class Help
    {
        public static void ShowAppCommands()
        {
            Console.WriteLine("");
            Console.WriteLine("Game codes:");
            Console.WriteLine("-hd = Sets the tool compatibility for the HD collection version");
            Console.WriteLine("-mars = Sets the tool compatibility for the MARS version");
            Console.WriteLine("");
            Console.WriteLine("Tool actions:");
            Console.WriteLine("-u = Unpack a .pac file");
            Console.WriteLine("-uf Unpack a single file out of a .pac file");
            Console.WriteLine("-un Unpack filenames in a .pac file to a txt file");
            Console.WriteLine("-r = Repack a folder to a .pac file");
            Console.WriteLine("");
            Console.WriteLine("Examples with MARS game code:");
            Console.WriteLine("Zoe2PacTool.exe -mars " + "-u" + @" ""atlas_pc_Zlib.pac""");
            Console.WriteLine("Zoe2PacTool.exe -mars " + "-uf" + @" ""atlas_pc_Zlib.pac""" + @" ""@af01g_17b7b18e.dat"" ");
            Console.WriteLine("Zoe2PacTool.exe -mars " + "-un" + @" ""atlas_pc_Zlib.pac""");
            Console.WriteLine("Zoe2PacTool.exe -mars " + "-r" + @" ""_atlas_pc_Zlib.pac"" ");
            Console.WriteLine("");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}