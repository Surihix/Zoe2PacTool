## Format Structure

**Important:** Version 0.01a's offsets has to be read in Big Endian and version 0.01b's offsets has to be read in Little Endian.

#### Header Section (applies for both versions)
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4  | String | PAC, header |
| 0x4 | 0x5 | String | Version |
| 0x9 | 0x3 | UInt32 | Reserved, always null |
| 0xC | 0x4 | UInt32 | FileCount offset |
<br>

### Version 0.01a
#### Offset Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x10 | 0x4 | UInt32 | FileNames start offset |
| 0x14 | 0x4 | UInt32 | File table start offset |
| 0x18 | 0x4 | UInt32 | Data start offset |

#### File table section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | Total size of the file |
| 0x4 | 0x4 | UInt32 | Data start position, relative |
<br>

### Version 0.01b
#### Offset Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x10 | 0x8 | UInt64 | FileNames start offset |
| 0x18 | 0x8 | UInt64 | File table start offset |
| 0x20 | 0x8 | UInt64 | Data start offset |
| 0x28 | 0x8 | UInt64 | Reserved, always null |

#### File table section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x8 | UInt64 | Total size of the file |
| 0x8 | 0x8 | UInt64 | Data start position, relative |
<br>

### Notes
- The total size value in the File table section gives the total size of the file's data in the pac file and if the data is compressed, then an 
additional 4 bytes (uint32) with the uncompressed size value would be present before the compressed data begins.
- In the MARS version's pac file, this uncompressed size value has to be read in Little Endian irrespective of the pac file version. for the HD collection 
version's pac file, this value has to be read in Big Endian.
- MARS version uses Zlib compression, while the HD collection version uses raw Deflate. 
- In the MARS version, there are .tbl files stored along with the texture files and only the texture file's data is compressed. but this may not always be 
consistent as the pac file from PS4 version's does not have some of the texture file's data compressed. during unpacking and repacking this tool checks if the zlib header is present 4 bytes after the data and then proceeds to either decompress or compress 
the data.
