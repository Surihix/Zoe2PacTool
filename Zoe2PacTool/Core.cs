using Zoe2PacTool.Repack;
using Zoe2PacTool.Support;
using Zoe2PacTool.Unpack;

namespace Zoe2PacTool
{
    internal class Core
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("");
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                // Parse args
                if (args.Length == 1)
                {
                    if (args[0].Contains("-h") || args[0].Contains("-?"))
                    {
                        Help.ShowAppCommands();
                    }
                }

                if (args.Length < 3)
                {
                    Console.WriteLine("Warning: Enough arguments not specified. Please use -? or -h switches for more information!");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                if (Enum.TryParse(args[0].Replace("-", ""), false, out SharedEnums.GameCodes gameCode) == false)
                {
                    Console.WriteLine("Warning: Specified game code was invalid!");
                    Help.ShowAppCommands();
                }

                if (Enum.TryParse(args[1].Replace("-", ""), false, out ToolActionSwitches toolActionSwitch) == false)
                {
                    Console.WriteLine("Warning: Specified tool action was invalid!");
                    Help.ShowAppCommands();
                }

                if (toolActionSwitch == ToolActionSwitches.r)
                {
                    if (!Directory.Exists(args[2]))
                    {
                        SharedMethods.ErrorExit("Specified pac file does not exist!");
                    }
                }
                else
                {
                    if (!File.Exists(args[2]))
                    {
                        SharedMethods.ErrorExit("Specified extracted directory does not exist!");
                    }
                }

                switch (toolActionSwitch)
                {
                    case ToolActionSwitches.u:
                        PacUnpack.UnpackFiles(args[2], gameCode);
                        break;

                    case ToolActionSwitches.uf:
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Warning: Enough arguments not specified for this action. Please use -? or -h switches for more information!");
                            Console.ReadLine();
                            Environment.Exit(0);
                        }
                        PacUnpack.UnpackSingle(args[2], args[3], gameCode);
                        break;

                    case ToolActionSwitches.un:
                        PacUnpack.UnpackNames(args[2], gameCode);
                        break;

                    case ToolActionSwitches.r:
                        PacRepack.RepackFiles(args[2], gameCode);
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine($"Exception: {ex}");
                Console.ReadLine();
                Environment.Exit(2);
            }
        }


        enum ToolActionSwitches
        {
            u,
            uf,
            un,
            r
        }
    }
}