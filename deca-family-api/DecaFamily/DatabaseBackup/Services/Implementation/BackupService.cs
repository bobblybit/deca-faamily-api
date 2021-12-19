using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

using DatabaseBackup.Repository;
using DatabaseBackup.Services.Interface;
using DatabaseBackup.Settings;

using Microsoft.Extensions.Options;

namespace DatabaseBackup.Services.Implementation
{
    public class BackupService: IBackupService
    {
        private readonly IEnumerable<string> _connectionStrings;
        private readonly IFilesService _filesService;
        private readonly IBackupRepository _backupReporitory;
        public BackupService(IOptions<ConnectionStringsConfig> connectionStrings, IFilesService filesService, IBackupRepository backupReporitory)
        {
            _connectionStrings = connectionStrings.Value?.ConnectionStrings;
            _filesService = filesService;
            _backupReporitory = backupReporitory;
        }

        public bool CreateBackup()
        {
            if (_connectionStrings?.Count() < 1)
                return false;

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "backup");
            var zipFile = Path.Combine(Directory.GetCurrentDirectory(), "backup.zip");
            _filesService.DeleteDirectory(dir);
            _filesService.DeleteFile(zipFile);

            foreach (var conString in _connectionStrings)
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    
                    //create map tablenames to database name using tuple
                    var databaseTables = _backupReporitory.GetTables(conString);

                    foreach (var tablename in databaseTables.Item2)
                    {
                        //get table data
                        using (var dataTable = _backupReporitory.GetTableData(tablename, conString))
                        {
                            StringBuilder csvData = new StringBuilder();

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                //add column headers
                                csvData.Append(column.ColumnName + ',');
                            }

                            //Add new line.
                            csvData.Append("\r\n");

                            foreach (DataRow row in dataTable.Rows)
                            {
                                foreach (DataColumn column in dataTable.Columns)
                                {
                                    //transform the date to a commer separated data and add to row
                                    csvData.Append(row[column.ColumnName].ToString().Replace(",", ";") + ',');
                                }

                                //Add new line.
                                csvData.Append("\r\n");

                                var dirInfo = Directory.CreateDirectory(dir + $"/backup_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}/{databaseTables.Item1}");
                                var filename = dirInfo.FullName + $"/{tablename}.csv";
                                _filesService.CreateFile(filename, csvData.ToString());
                            }

                        }
                    }
                }
            }

            _filesService.CreateZipFile(dir, Directory.GetCurrentDirectory()+"/backup.zip");
            return true;
        }
    }
}
