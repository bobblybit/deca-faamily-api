using System;
using System.Collections.Generic;
using System.Data;

namespace DatabaseBackup.Repository
{
    public interface IBackupRepository
    {
        Tuple<string, List<string>> GetTables(string connectionString);
        DataTable GetTableData(string tableName, string connectionString);
    }
}
