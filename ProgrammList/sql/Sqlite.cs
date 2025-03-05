using Microsoft.Data.Sqlite;
using System.Configuration;

namespace ProgrammList.sql {
    public class Sqlite : SqlBaseAbstract {

        string filename;

        public Sqlite(string prm_path) {
            sqlitecon = new SqliteConnection("Data Source=" + prm_path + ConfigurationManager.AppSettings["filename"]);
            sqlitecon.Open();
        }

        public bool GetSingleLine(string pcid, string program, string version) {
            var command = sqlitecon.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";


            bool result = command.ExecuteReader().Read();

            return result;
        }

        public void CheckTableExists() {
            var command = sqlitecon.CreateCommand();
            command.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='list';";
            var name = command.ExecuteScalar();
            if (name != null && name.ToString() == "list") {
                return;
            }
            var cols = string.Join(" VARCHAR,", valuenames);
            cols = cols + " Varchar";
            command.CommandText = "CREATE TABLE list (" + cols + ")";
            command.ExecuteNonQuery();
        }


        public void InsertData(Dictionary<string, string> valuesqlCommand) {
            var transaction = sqlitecon.BeginTransaction();

            string result = "";
            for (int i = 0; i < valuenames.Length; i++) {
                result += valuesqlCommand.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length - 1) {
                    result += ",";
                }
            }


            var cols = String.Join(",", valuenames);

            string sqlCommand = "INSERT INTO list(" + cols + ")" + "VALUES(" + result + ")";
            SqliteConnection con;
            var command = new SqliteCommand(sqlCommand, sqlitecon, transaction);
            Console.WriteLine(sqlCommand);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void InsertOrUpdateData(Dictionary<string, string> value) {
            if (GetSingleLine(value.GetValueOrDefault("PCID"), value.GetValueOrDefault("DisplayName"), value.GetValueOrDefault("DisplayVersion"))) {

                Console.WriteLine("Update");
                UpdateData(value);
            }
            else {

                Console.WriteLine("Insert");
                InsertData(value);
            }
        }

        public void DeleteOldData(string hostname) {
            var command = sqlitecon.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
        }

        public void UpdateData(Dictionary<string, string> value) {
            var transaction = sqlitecon.BeginTransaction();
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


            var command = new SqliteCommand(sqlCommand, sqlitecon, transaction);
            for (int i = 0; i < valuenames.Length; i++) {
                if (valuenames[i] != "PCID") {
                    command.Parameters.AddWithValue("$" + valuenames[i], value.GetValueOrDefault(valuenames[i]));
                }
            }

            Console.WriteLine(command.CommandText);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void DeleteData(string id) {
            var command = sqlitecon.CreateCommand();
            command.CommandText = @"SELECT name FROM user WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    var name = reader.GetString(0);
                }
            }
        }

    }
}
