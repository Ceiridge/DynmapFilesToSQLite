using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace DynmapFilesToSQLite.Converter.Reader {
    public abstract class DynReader {
        protected DirectoryInfo myDir;

        public DynReader(DirectoryInfo myDir) {
            this.myDir = myDir;
        }

        public abstract void ExecuteSqliteCommands(SqliteTransaction transaction);
    }
}
