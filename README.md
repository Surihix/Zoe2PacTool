# Zoe2PacTool

This tool allows you to unpack and repack the .pac archive files that are present inside the atlas folder of Zone of the Enders 2. 
with this tool, you can extract the .dds texture files and the .tbl files (PC/PS4) that are present inside these archive files as well as repack the extracted
archive file. 

Use the following commands with this tool according to the game version: 
<br>For unpacking a file: ```Zoe2PacTool -mars -u "fileName.pac"```
<br>For repacking a file: ```Zoe2PacTool -mars -r "extractedFolderName" "oldPacFileName.pac"```

**Note:** The supported game version switches are ```-hd``` and ```-mars```. the first switch is for the Hexadrive updated HD collection version, while the second switch is for the MARS version i.e the PC and PS4 versions of the game. when repacking a file, you have to provide the old .pac file after the extracted folder name. remember that the extracted folder should contain all of the files that were extracted from the old .pac file and even if one file is missing, 
this tool will not repack the .pac file. if the repacking succeeds, you will get a new .pac file with the .new extension to it and you have rename the file to .pac to use it with the game.
