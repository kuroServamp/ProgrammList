using Microsoft.Data.Sqlite;

namespace ProgrammList.sql {
    public class Sqlite : SqlBase {

        string filename;
        SqliteConnection sqlitecon;

        public string[] valuenames = { "PCID", "DisplayName", "DisplayVersion", "InstallDate", "update_date", "APP_Architecture" };
        public string connstring = null;

        public Sqlite(string prm_path, string filename) {
            sqlitecon = new SqliteConnection("Data Source=" + prm_path + filename);
        }

        public void Open() {
            sqlitecon.Open();
        }

        public void Close() {
            sqlitecon.Close();
        }


        public void Open(DB type) {
            sqlitecon = new SqliteConnection(connstring);
            sqlitecon.Open();
        }


        public Sqlite(string prm_path) {
            string setting = ConfigManager.PrmListConfigManager.GetSetting("filename");
            if (setting != null && setting != "") {
                sqlitecon = new SqliteConnection("Data Source=" + prm_path + setting);
            }
            else {
                sqlitecon = new SqliteConnection("Data Source=" + prm_path + "sqlite.db");
            }
        }

        public bool GetSingleLine(string pcid, string program, string version) {
            Open();
            var command = sqlitecon.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";


            bool result = command.ExecuteReader().Read();
            Close();
            return result;
        }

        public void CheckTableExists() {
            Open();
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
            Close();
        }


        public void InsertData(Dictionary<string, string> valuesqlCommand) {
            Open();
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

        public void DeleteOldData(string hostname) {
            Open();
            var command = sqlitecon.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
            Close();
        }

        public void UpdateData(Dictionary<string, string> value) {
            Open();
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

            command.ExecuteNonQuery();
            transaction.Commit();
            Close();
        }

        public void DeleteData(string id) {
            Open();
            var command = sqlitecon.CreateCommand();
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
