using System;

namespace Domain
{
    public class DatabaseOptions
    {
        public const string ConfigSection = "SQLite";
        public String Path { get; set; } = String.Empty;
    }
}
