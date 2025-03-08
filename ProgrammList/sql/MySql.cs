using MySql.Data.MySqlClient;

namespace ProgrammList.sql {


    internal class Mysql : SqlBase {
        public string[] valuenames = { "PCID", "DisplayName", "DisplayVersion", "InstallDate", "update_date", "APP_Architecture" };
        public string connstring = null;
        public MySqlConnection mysqlcon = null;


        public Boolean GetSingleLine(string pcid, string program, string version) {
            Open();
            var command = mysqlcon.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";
            var result = command.ExecuteReader().Read();

            Close();
            return result;
        }


        public void Open() {
            mysqlcon.Open();
        }

        public void Close() {
            mysqlcon.Close();
        }

        public void CheckTableExists() {
            Open();
            var command = mysqlcon.CreateCommand();
            command.CommandText = @"SHOW TABLES LIKE 'list';";
            var name = command.ExecuteScalar();
            if (name != null && name.ToString() == "list") {
                return;
            }
            var cols = string.Join(" VARCHAR(255),", valuenames);
            cols = cols + " Varchar(255)";
            command.CommandText = "CREATE TABLE list (" + cols + ")";
            command.ExecuteNonQuery();
            Close();
        }


        public void InsertData(Dictionary<string, string> valuesqlCommand) {
            Open();
            var transaction = mysqlcon.BeginTransaction();

            string result = "";
            for (int i = 0; i < valuenames.Length; i++) {
                result += valuesqlCommand.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length - 1) {
                    result += ",";
                }
            }


            var cols = String.Join(",", valuenames);

            string sqlCommand = "INSERT INTO list(" + cols + ")" + "VALUES(" + result + ")";

            var command = new MySqlCommand(sqlCommand, mysqlcon, transaction);
            command.ExecuteNonQuery();
            transaction.Commit();
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

        public void UpdateData(Dictionary<string, string> value) {
            Open();
            var transaction = mysqlcon.BeginTransaction();
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


            var command = new MySqlCommand(sqlCommand, mysqlcon, transaction);
            for (int i = 0; i < valuenames.Length; i++) {
                if (valuenames[i] != "PCID") {
                    command.Parameters.AddWithValue("$" + valuenames[i], value.GetValueOrDefault(valuenames[i]));
                }
            }

            command.ExecuteNonQuery();
            transaction.Commit();
            Close();
        }


        public void DeleteOldData(string hostname) {
            Open();
            var command = mysqlcon.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
            Close();
        }
    }
}
