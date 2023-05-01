using Zoe2PacTool;
using Zoe2PacTool.PacCommon;
using Zoe2PacTool.PacRepack;

try
{
    if (args.Length < 3)
    {
        CmnMethods.ErrorExit("Enough arguments not specified");
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

    var toolAction = ToolActionSwitches.d;
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
        case ToolActionSwitches.d:
            CmnMethods.FileFolderExistsCheck(inFileFolder, CmnEnums.ExistsCheckType.file);
            CmnMethods.ValidityCheck(inFileFolder);
            PacUnpack.ArchiveUnpack(inFileFolder, gameVersion);
            break;

        case ToolActionSwitches.c:
            if (args.Length < 4)
            {
                CmnMethods.ErrorExit("atlas pac file to repack is not specified in the argument. specify this file along with the extracted folder");
            }
            var inPacFile = args[3];
            CmnMethods.FileFolderExistsCheck(inFileFolder, CmnEnums.ExistsCheckType.folder);
            CmnMethods.FileFolderExistsCheck(inPacFile, CmnEnums.ExistsCheckType.file);
            CmnMethods.ValidityCheck(inPacFile);

            switch (gameVersion)
            {
                case CmnEnums.VersionsList.mars:
                    RepackMARS.ArchiveMars(inFileFolder, inPacFile);
                    break;

                case CmnEnums.VersionsList.hd:
                    RepackHD.ArchiveHd(inFileFolder, inPacFile);
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
    CmnMethods.ErrorExit("" + ex);
}

enum ToolActionSwitches
{
    c,
    d
}