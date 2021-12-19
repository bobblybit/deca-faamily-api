using System.Collections.Generic;

namespace DatabaseBackup.Settings
{
    public class ConnectionStringsConfig
    {
        public const string ConnectionStringsSettingsSection = "DatabaseConnectionStrings";
        public IEnumerable<string> ConnectionStrings { get; set; }
    }
}
