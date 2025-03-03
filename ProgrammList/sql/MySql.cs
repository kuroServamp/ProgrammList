using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ProgrammList.sql {


        public class Mysql {

        string[] valuenames = { "PCID", "DisplayName", "DisplayVersion", "InstallDate", "update_date", "APP_Architecture" };

        public MySqlConnection Connection;

            //private static DbConnection instance;
            public Mysql() {
                //string CnnStr = "Data Source=local;Initial Catalog=programlist;User Id=prgmlist;pwd=G0KaUM7TzgO7ZoPZCifs";
               // instance = new MySqlConnection(CnnStr);
        }

            public void Open() {
                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", "localhost", "programlist", "prgmlist", "G0KaUM7TzgO7ZoPZCifs");
                Connection = new MySqlConnection(connstring);
                Connection.Open();
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
        internal Boolean getSingleLine(string pcid, string program, string version) {
            Open();
            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";
            var result = command.ExecuteReader().Read();

            Close();
            return result;
        }

        internal void checkTableExists() {
            Open();
            var command = Connection.CreateCommand();
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


            var cols = String.Join(",", valuenames);

            string sqlCommand = "INSERT INTO list(" + cols + ")" + "VALUES(" + result + ")";

            var command = new MySqlCommand(sqlCommand, Connection, transaction);
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


            var command = new MySqlCommand(sqlCommand, Connection, transaction);
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
