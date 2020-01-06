using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Linq;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class MarkerIconsReader : DynReader {
        public MarkerIconsReader(DirectoryInfo myDir) : base(myDir) { }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            var images = myDir.EnumerateFiles().Where(file => file.Name.EndsWith(".png"));

            foreach(FileInfo image in images) {
                SqliteCommand cmd = new SqliteCommand();
                cmd.CommandText = "INSERT INTO MarkerIcons VALUES (@Name, @FileBytes);";

                string name = image.Name.Substring(0, image.Name.Length - 4);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@FileBytes", File.ReadAllBytes(image.FullName));

                cmd.Transaction = transaction;
                cmd.Connection = transaction.Connection;
                cmd.ExecuteNonQuery();

                Console.WriteLine("Command executed for " + name);
            }
        }
    }
}
