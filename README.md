# Zoe2PacTool

This small program allows you to unpack and repack the .pac archive files that are present inside the atlas folder of the game Zone of the Enders 2. the program should be launched from command prompt with a few argument switches to perform a function. the list of valid argument switches are given below:

**Game Codes:**
<br>``-hd`` For the Hexadrive updated HD collection version
<br>``-mars`` For the PC and PS4 versions

<br>**Tool actions:**
<br>``-u`` Unpack all files stored in a pac file
<br>``-uf`` Unpack a sinlge file stored in a pac file
<br>``-un`` Unpack all filenames in a pac file, to a text file
<br>``-r`` Repack a folder containing valid extracted files into a pac file
<br>``-?`` or ``-h`` Display the help page
<br>

## Important notes
- You can run the program with just the `-?` or `-h` switches. for example `Zoe2PacTool -?`

- The gamecode switch determines how this program unpacks and repacks the files. so make sure to specify the correct switch according to the game version of the .pac file.

- If you want to unpack a single file, then provide the filename of that file after the pac file path argument. for more info, refer to the app's help page which can be accessed with the `-?` or `-h` switches.

- The unpacked .dat file will be renamed with a different extension which can be .dds, if its the HD collection or PC version and .gnf, if its the PS4 version. <br>Do note that the extension is determined based on the header data found in the .dat file and if in case a header data that is not DDS or GNF is found, then the file extension will remain the same as the one given in the .pac file. refer to the `!!pacEntries.json` file to map which .dds or .gnf file corresponds to a .dat file.

- When repacking the .pac file, you have to provide the extracted folder path as the argument after the `-r` switch. remember that the extracted folder should contain all of the files that were extracted from the .pac file and should also contain the `!!pacEntries.json` file. if this json file or any one of the extracted files inside the folder is missing, then this program will stop the repacking process midway with an error message.

- If the repacking succeeds, then a new .pac file will be created while the old .pac file will be renamed with an .old extension.

## For Developers
Refer to the format structure of the .pac file from [here](FormatStruct.md).
