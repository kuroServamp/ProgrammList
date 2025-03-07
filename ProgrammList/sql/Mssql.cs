

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ProgrammList.sql {
    public class Mssql : SqlBaseAbstract {

        //private static Dbconnection instance;
        public Mssql() {
            var builder = new SqlConnectionStringBuilder {
                DataSource = "localhost",
                UserID = "sa",
                Password = "2677890E23",
                InitialCatalog = "prgmlist",
                TrustServerCertificate = true
            };

            connstring = builder.ToString();
        }

        public void GetAllData() {
            Open(DB.MSSQL);
            var command = sqlcon.CreateCommand();
            command.CommandText = @"SELECT * FROM list";

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    var dataLine = reader.GetString(0);
                }
            }
            Close();
        }
        public bool GetSingleLine(string pcid, string program, string version) {
            Open(DB.MSSQL);
            var command = sqlcon.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";
            bool result = command.ExecuteReader().Read();

            Close();
            return result;
        }

        public void CheckTableExists() {
            Open(DB.MSSQL);
            var command = sqlcon.CreateCommand();
            command.CommandText = "select case when exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'prgmlist' AND  TABLE_NAME = 'list') then 1 else 0 end";

            Console.WriteLine("Executing: " + command.CommandText);
            var returncode = command.ExecuteScalar();
            if (returncode != null) {
                int rc;
                bool convertingResult = Int32.TryParse(returncode.ToString(), out rc);

                if (!convertingResult) {
                    throw new FormatException();
                }

                if (rc > 0) {
                    Console.WriteLine("Creating table...");
                }
                else {
                    return;
                }
            }
            var cols = string.Join(" VARCHAR(255),", valuenames);
            cols = cols + " Varchar(255)";
            command.CommandText = "CREATE TABLE list (" + cols + ")";
            command.ExecuteNonQuery();
            Close();
        }


        public void InsertData(Dictionary<string, string> valuesqlCommand) {
            Open(DB.MSSQL);
            var transaction = sqlcon.BeginTransaction();

            string result = "";
            for (int i = 0; i < valuenames.Length; i++) {
                string val = "";
                valuesqlCommand.TryGetValue(valuenames[i], out val);
                result += val;

                if (i < valuenames.Length - 1) {
                    result += ",";
                }
            }


            var cols = string.Join(",", valuenames);

            string sqlCommand = "INSERT INTO list(" + cols + ")" + "VALUES(" + result + ")";

            var command = new SqlCommand(sqlCommand, sqlcon, transaction);
            command.ExecuteNonQuery();
            transaction.Commit();
            Console.WriteLine(sqlCommand);
            Close();
        }

        public void InsertOrUpdateData(Dictionary<string, string> value) {
            string pcid = "";
            value.TryGetValue("PCID", out pcid);
            string displayName = "";
            value.TryGetValue("DisplayName", out displayName);
            string displayVersion = "";
            value.TryGetValue("DisplayVersion", out displayVersion);
            if (GetSingleLine(pcid, displayName, displayVersion)) {
                UpdateData(value);
            }
            else {
                InsertData(value);
            }
        }

        public void DeleteOldData(string hostname) {
            Open(DB.MSSQL);
            var command = sqlcon.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
            Close();
        }

        public void UpdateData(Dictionary<string, string> value) {
            Open(DB.MSSQL);
            var transaction = sqlcon.BeginTransaction();
            string sqlCommand = @"Update list ";

            string result = "set ";
            for (int i = 0; i < valuenames.Length; i++) {
                string val = "";
                value.TryGetValue(valuenames[i], out val);
                result += valuenames[i] + " = " + val;

                if (i < valuenames.Length - 1) {
                    result += " ,";
                }
            }

            sqlCommand = sqlCommand + result;
            string pcid = "";
            value.TryGetValue("PCID", out pcid);
            string displayName = "";
            value.TryGetValue("DisplayName", out displayName);
            string displayVersion = "";
            value.TryGetValue("DisplayVersion", out displayVersion);
            sqlCommand = sqlCommand + " WHERE PCID = " + pcid +
                 " and  DisplayName like " + displayName +
                 " and  DisplayVersion like " + displayVersion;


            var command = new SqlCommand(sqlCommand, sqlcon, transaction);
            for (int i = 0; i < valuenames.Length; i++) {
                if (valuenames[i] != "PCID") {
                    string itemValue = "";
                    value.TryGetValue(valuenames[i], out itemValue);
                    command.Parameters.AddWithValue("$" + valuenames[i], itemValue);
                }
            }
            Console.WriteLine(sqlCommand);
            command.ExecuteNonQuery();
            transaction.Commit();
            Close();
        }
    }
}
