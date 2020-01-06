using Mono.Options;

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

            
        }
    }
}
