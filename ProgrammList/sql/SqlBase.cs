using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace ProgrammList.sql {
    
    /**
     * Basic commands to connect to the Sqlight database
     */
    internal class SqlBase {

        SqliteConnection connection;
        string prgm_path;
        string filename;

        string[] valuenames = { "PCID", "DisplayName", "DisplayVersion", "InstallDate", "update_date", "APP_Architecture" };

        /**
         * Constructor
         * 
         */
        internal SqlBase(string prm_path, string file) {

            prgm_path = prm_path + filename;
            filename = file;

            //connection = new SqliteConnection("Data Source=" + prm_path + filename + " ;PRAGMA journal_mode=WAL;");
            connection = new SqliteConnection("Data Source=" + prm_path + filename);
            connection.Open();
        }

        internal void getAllData() {
            var command = connection.CreateCommand();
            command.CommandText =  @"SELECT * FROM list";

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    var dataLine = reader.GetString(0);
                }
            }
        }
        internal Boolean getSingleLine(string pcid, string program, string version) {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";


            bool result = command.ExecuteReader().Read();

            return result;
        }

        internal void checkTableExists() {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='list';";
            var name = command.ExecuteScalar();
            if (name != null && name.ToString() == "list") {
                return;
            }
            var cols = string.Join(" VARCHAR,", valuenames);
            cols = cols + " Varchar";
            command.CommandText = "CREATE TABLE list (" +cols + ")";
            command.ExecuteNonQuery();
        }


        internal void InsertData(Dictionary<string, string> valuesqlCommand) {
            var transaction = connection.BeginTransaction();

            string result = "";
            for (int i = 0; i < valuenames.Length; i++) {
                result += valuesqlCommand.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length -1) {
                    result += ",";
                }
            }


            var cols = String.Join(",", valuenames);

            string sqlCommand = "INSERT INTO list(" + cols + ")" + "VALUES(" + result + ")";

            var command = new SqliteCommand(sqlCommand, connection, transaction);
            Console.WriteLine(sqlCommand);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        internal void InsertOrUpdateData(Dictionary<string, string> value) {
            if (getSingleLine(value.GetValueOrDefault("PCID"), value.GetValueOrDefault("DisplayName"), value.GetValueOrDefault("DisplayVersion"))) {

                Console.WriteLine("Update");
                UpdateData(value);
            }
            else {

                Console.WriteLine("Insert");
                InsertData(value);
            }
        }

        internal void deleteOldData(string hostname) {
            var command = connection.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
        }

        internal void UpdateData(Dictionary<string, string> value) {
            var transaction = connection.BeginTransaction();
            string sqlCommand = @"Update list ";

            string result = "set ";
            for (int i = 0; i < valuenames.Length; i++) {
                result += valuenames[i] + " = " + value.GetValueOrDefault(valuenames[i]);

                if (i < valuenames.Length - 1) {
                    result += " ,";
                }
            }

            sqlCommand = sqlCommand + result;
           sqlCommand = sqlCommand +  " WHERE PCID = " + value.GetValueOrDefault("PCID") + 
                " and  DisplayName like " + value.GetValueOrDefault("DisplayName") + 
                " and  DisplayVersion like " + value.GetValueOrDefault("DisplayVersion");


            var command = new SqliteCommand(sqlCommand, connection, transaction);
            for (int i = 0; i < valuenames.Length; i++) {
                if (valuenames[i] != "PCID") {
                    command.Parameters.AddWithValue("$" + valuenames[i], value.GetValueOrDefault(valuenames[i]));
                }
            }

            Console.WriteLine(command.CommandText);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        internal void DeleteData(string id) {
            var command = connection.CreateCommand();
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
