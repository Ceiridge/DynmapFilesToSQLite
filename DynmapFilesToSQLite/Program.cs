using Mono.Options;
using System;
using System.IO;

namespace DynmapFilesToSQLite {
    class Program {
        static void Main(string[] args) {
            string tilesFolderPath = "", outputFilePath = "";
            bool useJPGs = false;

            OptionSet options = new OptionSet() {
                { "tilesFolder=", arg => tilesFolderPath = arg },
                { "isJPG", arg => useJPGs = arg != null },
                { "outputFile=", arg => outputFilePath = arg }
            };
            options.Parse(args);

            if(tilesFolderPath.Length == 0 || outputFilePath.Length == 0) {
                Console.WriteLine("Not all arguments provided");
                return;
            }

            DirectoryInfo tilesFolder = new DirectoryInfo(tilesFolderPath);
            if(!tilesFolder.Exists) {
                Console.WriteLine("Tiles folder not found");
                return;
            }

            DirectoryInfo markersFolder = new DirectoryInfo(Path.Combine(tilesFolder.FullName, "_markers_"));
            if (!markersFolder.Exists) {
                Console.WriteLine("Tiles folder invalid");
                return;
            }

            FileInfo outputFile = new FileInfo(outputFilePath);
            Converter.Converter.Convert(tilesFolder, markersFolder, outputFile, useJPGs);

            Console.ReadKey();
        }
    }
}
