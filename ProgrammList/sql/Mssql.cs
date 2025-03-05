

using Microsoft.Data.SqlClient;

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
                result += valuesqlCommand.GetValueOrDefault(valuenames[i]);

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
            if (GetSingleLine(value.GetValueOrDefault("PCID"), value.GetValueOrDefault("DisplayName"), value.GetValueOrDefault("DisplayVersion"))) {
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
                result += valuenames[i] + " = " + value.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length - 1) {
                    result += " ,";
                }
            }

            sqlCommand = sqlCommand + result;
            sqlCommand = sqlCommand + " WHERE PCID = " + value.GetValueOrDefault("PCID") +
                 " and  DisplayName like " + value.GetValueOrDefault("DisplayName") +
                 " and  DisplayVersion like " + value.GetValueOrDefault("DisplayVersion");


            var command = new SqlCommand(sqlCommand, sqlcon, transaction);
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
    }
}
