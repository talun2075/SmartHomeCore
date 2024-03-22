using System;

namespace Domain
{
    public class DatabaseOptions
    {
        public const string ConfigSection = "SQLite";
        public String PathBatttery { get; set; } = String.Empty;
        public String PathReceipt { get; set; } = String.Empty;
    }
}
