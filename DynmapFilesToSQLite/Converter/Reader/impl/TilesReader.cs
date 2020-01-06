using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class TilesReader : DynReader {
        private MapTypesReader mapTypesReader;
        private Dictionary<TileChunk, byte[]> tileHashes = new Dictionary<TileChunk, byte[]>();
        private bool isJpeg;

        public TilesReader(DirectoryInfo myDir, MapTypesReader mapTypesReader, bool isJpeg) : base(myDir) {
            this.mapTypesReader = mapTypesReader;
            this.isJpeg = isJpeg;
        }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            int x = 0;
            List<DirectoryInfo> worldNames = new List<DirectoryInfo>();
            worldNames.AddRange(myDir.EnumerateDirectories().Where(dir => {
                return !dir.Name.Equals("_markers_") && !dir.Name.Equals("faces");
            }));

            foreach (DirectoryInfo world in worldNames) {
                string worldName = world.Name;

                foreach (FileInfo hashFile in world.EnumerateFiles()) {
                    if (!hashFile.Name.EndsWith(".hash"))
                        continue;

                    string hashFileName = hashFile.Name;

                    byte[] hashBytes = File.ReadAllBytes(hashFile.FullName);
                    string mapId = hashFileName.Split('_')[0];
                    int tcx = int.Parse(hashFileName.Split('_')[1]);
                    int tcy = int.Parse(hashFileName.Split('_')[2].Split('.')[0]);

                    tileHashes.Add(new TileChunk {
                        tcx = tcx,
                        tcy = tcy,
                        type = new MapTypesReader.MapType {
                            mapId = mapId,
                            worldName = worldName,
                            variant = "STANDARD"
                        }
                    }, hashBytes);
                }

                foreach (DirectoryInfo mapId in world.EnumerateDirectories()) {
                    string mapIdName = mapId.Name;
                    MapTypesReader.MapType mapType = new MapTypesReader.MapType {
                        mapId = mapIdName,
                        worldName = worldName,
                        variant = "STANDARD"
                    };
                    int mapTypeId = this.mapTypesReader.GetMapTypeId(mapType);

                    foreach(DirectoryInfo chunkId in mapId.EnumerateDirectories()) {
                        string chunkIdName = chunkId.Name;
                        int tcx = int.Parse(chunkIdName.Split('_')[0]);
                        int tcy = int.Parse(chunkIdName.Split('_')[1]);

                        foreach(FileInfo tile in chunkId.EnumerateFiles()) {
                            string tileName = tile.Name;
                            int zoom = 0;
                            int tx = 0;
                            int ty = 0;

                            if (tileName.StartsWith("z")) {
                                zoom = tileName.Count(ch => ch == 'z');
                            }
                            tx = int.Parse(tileName.Split('_')[zoom > 0 ? 1 : 0]);
                            ty = int.Parse(tileName.Split('_')[zoom > 0 ? 2 : 1].Split('.')[0]);

                            long crc = GetCRCFromTile(new TileChunk {
                                tcx = tcx,
                                tcy = tcy,
                                type = mapType
                            }, tx, ty);
                            long lastUpdate = (long)File.GetLastWriteTimeUtc(tile.FullName).Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000L;

                            SqliteCommand cmd = new SqliteCommand();
                            cmd.CommandText = "INSERT INTO Tiles VALUES (@MapId, @TX, @TY, @Zoom, @Hash, @LastUpdate, @Format, @ImageBytes);";

                            cmd.Parameters.AddWithValue("@MapId", mapTypeId);
                            cmd.Parameters.AddWithValue("@TX", tx);
                            cmd.Parameters.AddWithValue("@TY", ty);
                            cmd.Parameters.AddWithValue("@Zoom", zoom);
                            cmd.Parameters.AddWithValue("@Hash", crc);
                            cmd.Parameters.AddWithValue("@LastUpdate", lastUpdate);
                            cmd.Parameters.AddWithValue("@Format", isJpeg ? 1 : 0);
                            cmd.Parameters.AddWithValue("@ImageBytes", File.ReadAllBytes(tile.FullName));

                            cmd.Transaction = transaction;
                            cmd.Connection = transaction.Connection;
                            cmd.ExecuteNonQuery();

                        }
                        Console.WriteLine("Added new chunk tile: " + " " + tcx + " " + tcy + " " + worldName + " " + mapIdName);
                    }
                }
            }
        }

        // Transpiled from https://github.com/webbukkit/dynmap/blob/49ea99b7038bc855d7c70ae952c52399a3f82d21/DynmapCore/src/main/java/org/dynmap/storage/filetree/TileHashManager.java#L104
        private long GetCRCFromTile(TileChunk chunk, int tx, int ty) {
            byte[] crcbuf = tileHashes[chunk];

            int off = (128 * (ty & 0x1F)) + (4 * (tx & 0x1F));
            long crc = 0;
            for (int i = 0; i < 4; i++)
                crc = (crc << 8) + (0xFF & crcbuf[off + i]);
            return crc;
        }

        private struct TileChunk {
            public int tcx, tcy;
            public MapTypesReader.MapType type;
        }
    }
}
