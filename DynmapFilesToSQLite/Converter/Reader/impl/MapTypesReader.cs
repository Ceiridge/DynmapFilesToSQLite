using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class MapTypesReader : DynReader {
        public List<MapType> mapTypes = new List<MapType>();

        public MapTypesReader(DirectoryInfo myDir) : base(myDir) { }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            List<DirectoryInfo> worldNames = new List<DirectoryInfo>();
            worldNames.AddRange(myDir.EnumerateDirectories().Where(dir => {
                return !dir.Name.Equals("_markers_") && !dir.Name.Equals("faces");
            }));

            foreach(DirectoryInfo world in worldNames) {
                foreach(DirectoryInfo mapId in world.EnumerateDirectories()) {
                    MapType type;
                    mapTypes.Add(type = new MapType {
                        mapId = mapId.Name,
                        worldName = world.Name,
                        variant = "STANDARD"
                    });

                    SqliteCommand cmd = new SqliteCommand();
                    cmd.CommandText = "INSERT INTO Maps VALUES (@ID, @WorldID, @MapID, @Variant);";

                    cmd.Parameters.AddWithValue("@ID", mapTypes.Count);
                    cmd.Parameters.AddWithValue("@WorldID", type.worldName);
                    cmd.Parameters.AddWithValue("@MapID", type.mapId);
                    cmd.Parameters.AddWithValue("@Variant", type.variant);

                    cmd.Transaction = transaction;
                    cmd.Connection = transaction.Connection;
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Added new map type: " + mapTypes.Count);
                }
            }
        }

        public int GetMapTypeId(MapType type) {
            int i = 1;
            foreach(MapType savedType in this.mapTypes) {
                if (savedType.worldName.Equals(type.worldName) && savedType.mapId.Equals(type.mapId) && savedType.variant.Equals(type.variant))
                    return i;
                i++;
            }

            return -1;
        }

        public MapType GetMapType(int id) {
            return this.mapTypes[id - 1];
        }


        public struct MapType {
            public string worldName, mapId, variant;
        }
    }
}
