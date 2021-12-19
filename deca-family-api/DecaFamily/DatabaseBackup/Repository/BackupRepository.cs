using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseBackup.Repository
{
    public class BackupRepository : IBackupRepository
    {
        public DataTable GetTableData(string tableName, string connectionString)
        {
            DataTable dt;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", con))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        dt = new DataTable();
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public Tuple<string, List<string>> GetTables(string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                List<string> tableNames = new List<string>();
                string databaseName = string.Empty;

                //open database connection
                con.Open();

                //Data database tables schema
                DataTable dbTableSchemas = con.GetSchema("Tables");

                //close database connection
                con.Close();

                //Get all table names
                foreach (DataRow row in dbTableSchemas.Rows)
                {
                    var name = (string)row[2];
                    if (name.ToLower().Contains("migration"))
                    {
                        continue;
                    }
                    tableNames.Add(name);
                    databaseName = (string)row[0];
                }

                //create map tablenames to database name using tuple
                var databaseTables = Tuple.Create(databaseName, tableNames);
                return databaseTables;
            }
        }
    }
}
