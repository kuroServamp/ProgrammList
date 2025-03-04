

using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProgrammList.sql {
    internal class Mssql {
        string[] valuenames = { "PCID", "DisplayName", "DisplayVersion", "InstallDate", "update_date", "APP_Architecture" };

        public SqlConnection Connection;
        string connstring;

        //private static DbConnection instance;
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

        public void Open() {
            Connection = new SqlConnection(connstring);
            try {
                Connection.Open();
            }
            catch (Exception ex) {
                Console.Write(ex.ToString());
            }
        }

        public void Close() {
            Connection.Close();
        }


        internal void getAllData() {
            Open();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM list";

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    var dataLine = reader.GetString(0);
                }
            }
            Close();
        }
        internal bool getSingleLine(string pcid, string program, string version) {
            Open();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";
            bool result = command.ExecuteReader().Read();

            Close();
            return result;
        }

        internal void checkTableExists() {
            Open();
            var command = Connection.CreateCommand();
            command.CommandText = "IF OBJECT_ID('list', 'U') IS NOT NULL BEGIN PRINT '0' END ELSE BEGIN PRINT '1' END";
            var returncode = command.ExecuteScalar();
            if (returncode != null && returncode.ToString() == "1") {
                Console.WriteLine("Creating table...");
            }
            var cols = string.Join(" VARCHAR(255),", valuenames);
            cols = cols + " Varchar(255)";
            command.CommandText = "CREATE TABLE list (" + cols + ")";
            command.ExecuteNonQuery();
            Close();
        }


        internal void InsertData(Dictionary<string, string> valuesqlCommand) {
            Open();
            var transaction = Connection.BeginTransaction();

            string result = "";
            for (int i = 0; i < valuenames.Length; i++) {
                result += valuesqlCommand.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length - 1) {
                    result += ",";
                }
            }


            var cols = string.Join(",", valuenames);

            string sqlCommand = "INSERT INTO list(" + cols + ")" + "VALUES(" + result + ")";

            var command = new SqlCommand(sqlCommand, Connection, transaction);
            command.ExecuteNonQuery();
            transaction.Commit();
            Console.WriteLine(sqlCommand);
            Close();
        }

        internal void InsertOrUpdateData(Dictionary<string, string> value) {
            if (getSingleLine(value.GetValueOrDefault("PCID"), value.GetValueOrDefault("DisplayName"), value.GetValueOrDefault("DisplayVersion"))) {
                UpdateData(value);
            }
            else {
                InsertData(value);
            }
        }

        internal void deleteOldData(string hostname) {
            Open();
            var command = Connection.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
            Close();
        }

        internal void UpdateData(Dictionary<string, string> value) {
            Open();
            var transaction = Connection.BeginTransaction();
            string sqlCommand = @"Update list ";

            string result = "set ";
            for (int i = 0; i < valuenames.Length; i++) {
                result += valuenames[i] + " = " + value.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length - 1) {
                    result += " ,";
                }
            }

            sqlCommand = sqlCommand + result;
            sqlCommand = sqlCommand + " WHERE PCID = " + value.GetValueOrDefault("PCID") +
                 " and  DisplayName like " + value.GetValueOrDefault("DisplayName") +
                 " and  DisplayVersion like " + value.GetValueOrDefault("DisplayVersion");


            var command = new SqlCommand(sqlCommand, Connection, transaction);
            for (int i = 0; i < valuenames.Length; i++) {
                if (valuenames[i] != "PCID") {
                    command.Parameters.AddWithValue("$" + valuenames[i], value.GetValueOrDefault(valuenames[i]));
                }
            }
            Console.WriteLine(sqlCommand);
            command.ExecuteNonQuery();
            transaction.Commit();
            Close();
        }

        internal void DeleteData(string id) {
            Open();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT name FROM user WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    var name = reader.GetString(0);
                }
            }
            Close();
        }
    }
}
