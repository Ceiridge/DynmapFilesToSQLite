using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

// Word Wrap is recommended
namespace DynmapFilesToSQLite.Converter {
    class SQLiteWriter {
        private const string CREATE_TABLES_COMMANDS = "CREATE TABLE Faces (PlayerName STRING NOT NULL, TypeID INT NOT NULL, Image BLOB, PRIMARY KEY(PlayerName, TypeID));CREATE TABLE Maps (ID INTEGER PRIMARY KEY AUTOINCREMENT, WorldID STRING NOT NULL, MapID STRING NOT NULL, Variant STRING NOT NULL);CREATE TABLE MarkerFiles (FileName STRING PRIMARY KEY NOT NULL, Content CLOB);CREATE TABLE MarkerIcons (IconName STRING PRIMARY KEY NOT NULL, Image BLOB);CREATE TABLE SchemaVersion (level INT PRIMARY KEY NOT NULL);CREATE TABLE Tiles (MapID INT NOT NULL, x INT NOT NULL, y INT NOT NULL, zoom INT NOT NULL, HashCode INT NOT NULL, LastUpdate INT NOT NULL, Format INT NOT NULL, Image BLOB, PRIMARY KEY(MapID, x, y, zoom));";

        public FileInfo sqliteFile;
        public SqliteConnection sql;

        public SQLiteWriter(FileInfo sqliteFile) {
            this.sqliteFile = sqliteFile;

            if(sqliteFile.Exists) {
                Console.WriteLine("Deleting old database...");
                sqliteFile.Delete();
            }
            sqliteFile.Create().Close();

            SqliteConnectionStringBuilder connectionBuilder = new SqliteConnectionStringBuilder();
            connectionBuilder.DataSource = sqliteFile.FullName;
            connectionBuilder.Cache = SqliteCacheMode.Private;
            this.sql = new SqliteConnection(connectionBuilder.ToString());
            this.sql.Open();
        }

        public void CreateTables() {
            SqliteCommand createCommands = new SqliteCommand(CREATE_TABLES_COMMANDS, this.sql);
            createCommands.ExecuteReader();
        }

        public SqliteTransaction CreateTransaction() {
            this.sql.Open();
            return this.sql.BeginTransaction();
        }

        public void Close() {
            this.sql.Close();
        }
    }
}
