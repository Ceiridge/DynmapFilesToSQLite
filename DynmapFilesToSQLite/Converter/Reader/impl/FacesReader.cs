using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynmapFilesToSQLite.Converter.Reader.impl {
    public class FacesReader : DynReader {
        public FacesReader(DirectoryInfo myDir) : base(myDir) { }

        public override void ExecuteSqliteCommands(SqliteTransaction transaction) {
            
        }
    }
}
