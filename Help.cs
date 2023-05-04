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
            Console.WriteLine("-r = Repack a .pac file");
            Console.WriteLine("");
            Console.WriteLine("Examples with MARS game code:");
            Console.WriteLine("Zoe2PacTool.exe -mars " + "-u" + @" ""fileName.pac""");
            Console.WriteLine("Zoe2PacTool.exe -mars " + "-r" + @" ""extractedFolderName"" " + @"""fileName.pac""");
            Console.WriteLine("");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}