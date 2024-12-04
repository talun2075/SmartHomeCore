using System;

namespace Domain
{
    public class DatabaseOptions
    {
        public const string ConfigSection = "SQLite";
        public String PathReceipt { get; set; } = String.Empty;
    }
}
