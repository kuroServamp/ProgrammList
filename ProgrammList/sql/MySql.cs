using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ProgrammList.sql {


    public class Mysql : SqlBaseAbstract {


        public Boolean GetSingleLine(string pcid, string program, string version) {
            Open(DB.MYSQL);
            var command = mysqlcon.CreateCommand();
            command.CommandText = @"SELECT * FROM list where PCID like "
                + pcid + " and  DisplayName like "
                + program + " and  DisplayVersion like " + version + ";";
            var result = command.ExecuteReader().Read();

            Close();
            return result;
        }

        public void CheckTableExists() {
            Open(DB.MYSQL);
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
            Open(DB.MYSQL);
            var transaction = mysqlcon.BeginTransaction();

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

            var command = new MySqlCommand(sqlCommand, mysqlcon, transaction);
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

        public void UpdateData(Dictionary<string, string> value) {
            Open(DB.MYSQL);
            var transaction = mysqlcon.BeginTransaction();
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


            var command = new MySqlCommand(sqlCommand, mysqlcon, transaction);
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


        public void DeleteOldData(string hostname) {
            Open(DB.MYSQL);
            var command = mysqlcon.CreateCommand();
            string sqlCommand = @"delete from list where PCID = '" + hostname + "';";
            command.CommandText = sqlCommand;
            command.ExecuteReader();
            Close();
        }
    }
}
