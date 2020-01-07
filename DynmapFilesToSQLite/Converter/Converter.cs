using DynmapFilesToSQLite.Converter.Reader;
using DynmapFilesToSQLite.Converter.Reader.impl;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace DynmapFilesToSQLite.Converter {
    public class Converter {
        public static void Convert(DirectoryInfo tilesFolder, DirectoryInfo markersFolder, FileInfo outputFile, bool useJPG) {
            FileInfo saveStateFile = new FileInfo(outputFile.FullName + ".progress.bin");
            bool hasSaveState = saveStateFile.Exists;

            MapTypesReader mapTypesReader = new MapTypesReader(tilesFolder, hasSaveState);

            List<DynReader> readers = new List<DynReader>();
            if (!hasSaveState) { // Prevent filling the database again
                readers.Add(new MarkerIconsReader(markersFolder));
                readers.Add(new MarkerFilesReader(markersFolder));
                readers.Add(new FacesReader(new DirectoryInfo(Path.Combine(tilesFolder.FullName, "faces"))));
                readers.Add(new SchemaVersionWriter(null));
            }
            readers.Add(mapTypesReader);
            readers.Add(new TilesReader(tilesFolder, mapTypesReader, useJPG, saveStateFile, hasSaveState));

            SQLiteWriter writer = new SQLiteWriter(outputFile, !hasSaveState);
            if(!hasSaveState)
                writer.CreateTables();

            foreach(DynReader reader in readers) {
                SqliteTransaction transaction = writer.CreateTransaction();
                reader.ExecuteSqliteCommands(transaction);
                transaction.Commit();

                Console.WriteLine("Committed " + reader.GetType().Name);
            }

            writer.Close();
        }
    }
}
