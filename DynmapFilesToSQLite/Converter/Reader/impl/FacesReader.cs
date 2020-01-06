using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class FacesReader : DynReader {
        public FacesReader(DirectoryInfo myDir) : base(myDir) { }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            string myDirPath = myDir.FullName;

            Dictionary<int, string> faceTypes = new Dictionary<int, string>();
            faceTypes.Add(0, "8x8");
            faceTypes.Add(1, "16x16");
            faceTypes.Add(2, "32x32");
            faceTypes.Add(3, "body");

            foreach (int faceType in faceTypes.Keys) {
                string faceTypeStr = faceTypes[faceType];
                DirectoryInfo faceDir = new DirectoryInfo(Path.Combine(myDirPath, faceTypeStr));

                if (faceDir.Exists) {
                    foreach(FileInfo image in faceDir.EnumerateFiles().Where(file => file.Name.EndsWith(".png"))) {
                        SqliteCommand cmd = new SqliteCommand();
                        cmd.CommandText = "INSERT INTO Faces VALUES (@Name, @Type, @ImageBytes);";

                        string name = image.Name.Substring(0, image.Name.Length - 4);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Type", faceType);
                        cmd.Parameters.AddWithValue("@ImageBytes", File.ReadAllBytes(image.FullName));

                        cmd.Transaction = transaction;
                        cmd.Connection = transaction.Connection;
                        cmd.ExecuteNonQuery();

                        Console.WriteLine("Command executed for " + name + " " + faceTypeStr);
                    }
                }
            }
        }
    }
}
