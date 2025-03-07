using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ProgrammList.sql {

    public abstract class SqlBaseAbstract : SqlBase {


        public string[] valuenames = { "PCID", "DisplayName", "DisplayVersion", "InstallDate", "update_date", "APP_Architecture" };
        public string connstring = null;
        public MySqlConnection mysqlcon = null;
        public SqlConnection sqlcon = null;
        public SqliteConnection sqlitecon = null;

        public void Close() {
            if (mysqlcon != null) {
                mysqlcon.Close();
            }
            else if (sqlcon != null) {
                sqlcon.Close();
            }
            else if (sqlitecon != null) {
                sqlitecon.Close();
            }
        }


        public void Open(DB type) {
            switch (type) {
                case DB.MYSQL:
                case DB.MARIADB:
                    mysqlcon = new MySqlConnection(connstring);
                    try {
                        mysqlcon.Open();
                    }
                    catch (Exception ex) {
                        Console.Write(ex.ToString());
                    }
                    break;
                case DB.MSSQL:
                    sqlcon = new SqlConnection(connstring);
                    try {
                        mysqlcon.Open();
                    }
                    catch (Exception ex) {
                        Console.Write(ex.ToString());
                    }
                    break;
                case DB.SQLITE:
                    sqlitecon = new SqliteConnection(connstring);
                    try {
                        mysqlcon.Open();
                    }
                    catch (Exception ex) {
                        Console.Write(ex.ToString());
                    }
                    break;
            }
        }

        void SqlBase.CheckTableExists() {
            throw new NotImplementedException();
        }

        void SqlBase.DeleteOldData(string hostname) {
            throw new NotImplementedException();
        }

        bool SqlBase.GetSingleLine(string pcid, string program, string version) {
            throw new NotImplementedException();
        }

        void SqlBase.InsertData(Dictionary<string, string> valuesqlCommand) {
            throw new NotImplementedException();
        }

        void SqlBase.InsertOrUpdateData(Dictionary<string, string> value) {
            throw new NotImplementedException();
        }

        void SqlBase.UpdateData(Dictionary<string, string> value) {
            throw new NotImplementedException();
        }
    }
}
