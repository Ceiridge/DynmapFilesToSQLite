using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Linq;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class SchemaVersionWriter : DynReader {
        public SchemaVersionWriter(DirectoryInfo myDir) : base(myDir) { }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            SqliteCommand cmd = new SqliteCommand();
            cmd.CommandText = "INSERT INTO SchemaVersion VALUES (@Version);";

            cmd.Parameters.AddWithValue("@Version", 1); // yes this is the old one, so on newer versions, an automatic conversion should hopefully take place? that's not my problem at least...

            cmd.Transaction = transaction;
            cmd.Connection = transaction.Connection;
            cmd.ExecuteNonQuery();

            Console.WriteLine("Schema version written");
        }
    }
}
