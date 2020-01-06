using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class MarkerFilesReader : DynReader {
        public MarkerFilesReader(DirectoryInfo myDir) : base(myDir) { }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            var markers = myDir.EnumerateFiles().Where(file => file.Name.EndsWith(".json"));

            foreach (FileInfo markerInfo in markers) {
                SqliteCommand cmd = new SqliteCommand();
                cmd.CommandText = "INSERT INTO MarkerFiles VALUES (@Name, @ContentText);";

                string name = markerInfo.Name.Substring(0, markerInfo.Name.Length - 5);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@ContentText", File.ReadAllText(markerInfo.FullName, Encoding.UTF8));

                cmd.Transaction = transaction;
                cmd.Connection = transaction.Connection;
                cmd.ExecuteNonQuery();

                Console.WriteLine("Command executed for " + name + " marker");
            }
        }
    }
}
