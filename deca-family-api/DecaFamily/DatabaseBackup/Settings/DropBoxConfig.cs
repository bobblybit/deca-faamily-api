using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseBackup.Settings
{
    public class DropBoxConfig
    {
        public const string DropBoxSettings = "DropBoxSettings";
        public string AccessToken { get; set; }
    }
}
