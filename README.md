# Zoe2PacTool

This tool allows you to unpack and repack the .pac archive files that are present inside the atlas folder of Zone of the Enders 2. 
with this tool, you can extract the .dds texture files and the .tbl files (PC/PS4) that are present inside these archive files as well as repack the extracted
archive file. 

Use the following commands with this tool according to the game version: 
<br>For unpacking all file: ```Zoe2PacTool -mars -u "fileName.pac"```
<br>For unpacking a single file: ```Zoe2PacTool -mars -uf "fileName.pac" "fileName.dat"```
<br>For unpacking filenames: ```Zoe2PacTool -mars -un "fileName.pac"```
<br>For repacking a file: ```Zoe2PacTool -mars -r "extractedFolderName"```
<br>

**Important:** The supported game version switches are ```-hd``` and ```-mars```. the first switch is for the Hexadrive updated HD collection version, while the second switch is for the MARS version i.e the PC and PS4 versions of the game. these switches are important and decide how this tool unpacks or repacks the 
file. so make sure to specify the correct switch according to the game version from which you got the .pac file.

**Note:** When repacking a file, you have to provide the extracted folder name as the argument after the ``` -r``` switch. remember that the extracted folder should contain all of the files that were extracted from the old .pac file and should also contain the ```!!!pacEntries.json``` file. even if the json file or any one of the extracted files inside the folder is missing, then this tool will stop the repacking process midway. if the repacking succeeds, a new .pac file will be created, while the old .pac file will be renamed with a .old extension.

## For Developers:
Refer to the format structure of the .pac file from [here](FormatStruct.md).
