# Dynmap Files to SQLite DB
This tool converts all saved dynmap tiles (the images of the world) to one sqlite database file, so it doesn't make your hard drive explode in terms of file count without having to rerender everything.

# Command Line Usage
```bash
DynmapFilesToSQLite.exe
--tilesFolder "YourServerPath\plugins\dynmap\web\tiles"
--outputFile "YourServerPath\plugins\dynmap\dynmap.db"
--isJPG true/false
```

All arguments are required. The release is compiled for Windows x86 .NET Framework 4.7.2.

## Warning
Depending of your map size, it can take hours, especially on slow hard drives.