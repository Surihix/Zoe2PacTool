﻿using Zoe2PacTool;
using Zoe2PacTool.PacCommon;
using Zoe2PacTool.PacRepack;


static void ValidityCheck(string filePathVar)
{
    using (FileStream checkFile = new(filePathVar, FileMode.Open, FileAccess.Read))
    {
        using (BinaryReader checkFileReader = new(checkFile))
        {
            var zpacStr = "";
            CmnMethods.GenerateString(checkFileReader, 0, 4, ref zpacStr);

            if (!zpacStr.Contains("PAC"))
            {
                CmnMethods.ErrorExit("This is not a valid ZOE2 atlas pac file.");
            }
        }
    }
}


try
{
    if (args.Length < 3)
    {
        Console.WriteLine("Enough arguments not specified");
        Help.ShowAppCommands();
    }

    var specifiedVersion = args[0].Replace("-", "");
    var specifiedAction = args[1].Replace("-", "");
    var inFileFolder = args[2];

    var gameVersion = CmnEnums.VersionsList.mars;
    if (Enum.TryParse(specifiedVersion, false, out CmnEnums.VersionsList convertedGameVersion))
    {
        gameVersion = convertedGameVersion;
    }
    else
    {
        CmnMethods.ErrorExit("Specified game version was invalid. has to be either -mars or -hd");
    }

    var toolAction = ToolActionSwitches.u;
    if (Enum.TryParse(specifiedAction, false, out ToolActionSwitches convertedToolAction))
    {
        toolAction = convertedToolAction;
    }
    else
    {
        CmnMethods.ErrorExit("Specified tool action was invalid. has to be either -c or -d");
    }

    switch (toolAction)
    {
        case ToolActionSwitches.u:
            CmnMethods.FileFolderExistsCheck(inFileFolder, CmnEnums.ExistsCheckType.file);
            ValidityCheck(inFileFolder);
            PacUnpack.ArchiveUnpack(inFileFolder, gameVersion);
            break;

        case ToolActionSwitches.r:
            if (args.Length < 4)
            {
                CmnMethods.ErrorExit("atlas pac file to repack is not specified in the argument. specify this file along with the extracted folder");
            }
            var inPacFile = args[3];
            CmnMethods.FileFolderExistsCheck(inFileFolder, CmnEnums.ExistsCheckType.folder);
            CmnMethods.FileFolderExistsCheck(inPacFile, CmnEnums.ExistsCheckType.file);
            ValidityCheck(inPacFile);

            switch (gameVersion)
            {
                case CmnEnums.VersionsList.mars:
                    PacRpkMARS.ArchiveMars(inFileFolder, inPacFile);
                    break;

                case CmnEnums.VersionsList.hd:
                    PacRpkHD.ArchiveHd(inFileFolder, inPacFile);
                    break;
            }
            break;
    }
}
catch (Exception ex)
{
    CmnMethods.FileFolderExistsDel("CrashLog.txt", CmnEnums.ExistsCheckType.file);

    using (StreamWriter logWriter = new("CrashLog.txt"))
    {
        logWriter.WriteLine(ex);
    }

    Console.WriteLine("Crash details recorded in CrashLog.txt file");
    Console.WriteLine("");
    CmnMethods.ErrorExit("" + ex);
}

enum ToolActionSwitches
{
    u,
    r
}