using Mono.Options;
using System;
using System.IO;

namespace DynmapFilesToSQLite {
    class Program {
        static void Main(string[] args) {
            string tilesFolderPath = "";
            bool useJPGs = false;

            OptionSet options = new OptionSet() {
                { "tilesFolder=", arg => tilesFolderPath = arg },
                { "useJPG", arg => useJPGs = arg != null }
            };
            options.Parse(args);

            DirectoryInfo tilesFolder = new DirectoryInfo(tilesFolderPath);
            if(!tilesFolder.Exists) {
                Console.WriteLine("Tiles folder not found");
                return;
            }

            DirectoryInfo markersFolder = new DirectoryInfo(Path.Combine(tilesFolder.FullName, "_markers_")));
            if (!markersFolder.Exists) {
                Console.WriteLine("Tiles folder invalid");
                return;
            }

            Converter.Converter.Convert(tilesFolder, markersFolder, useJPGs);
        }
    }
}
