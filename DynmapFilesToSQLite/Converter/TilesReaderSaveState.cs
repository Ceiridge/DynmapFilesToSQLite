using DynmapFilesToSQLite.Converter.Reader.impl;
using System;
using System.IO;

namespace DynmapFilesToSQLite.Converter {
    public class TilesReaderSaveState {
        public FileInfo saveStateFile;

        string chunkId;
        int tcx;
        int tcy;
        MapTypesReader.MapType mapType;

        public TilesReaderSaveState(FileInfo saveStateFile) {
            this.saveStateFile = saveStateFile;
        }

        public void WriteFile() {
            if (saveStateFile.Exists)
                saveStateFile.Delete();

            FileStream stream = saveStateFile.Open(FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(mapType.worldName);
            writer.Write(mapType.mapId);
            writer.Write(mapType.variant);

            writer.Write(chunkId);
            writer.Write(tcx);
            writer.Write(tcy);

            writer.Close();
            stream.Close();
        }

        public void ReadFile() {
            if (!saveStateFile.Exists)
                throw new Exception("File does not even exist");

            FileStream stream = saveStateFile.Open(FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            string worldName = reader.ReadString();
            string mapId = reader.ReadString();
            string variant = reader.ReadString();
            this.mapType = new MapTypesReader.MapType {
                worldName = worldName,
                mapId = mapId,
                variant = variant
            };

            this.chunkId = reader.ReadString();
            this.tcx = reader.ReadInt32();
            this.tcy = reader.ReadInt32();

            reader.Close();
            stream.Close();
        }
    }
}
