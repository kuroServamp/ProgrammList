using Microsoft.Win32;
using ProgrammList.ConfigManager;
using ProgrammList.sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ProgrammList.ListPrograms {
    public class ListPrograms {

        string prgm_path = Directory.GetCurrentDirectory() + "\\";
        string[] keyvaluenames = { "DisplayName", "DisplayVersion", "InstallDate" };
        private SqlBase sql = null;

        public void init() {
            string sqlType = PrmListConfigManager.GetSetting("DB_Type");
            if (sqlType == null) {
                Console.WriteLine("SQL Database not defined in app.conf, allowed types:");
                Console.WriteLine("MYSQL");
                Console.WriteLine("MARIADB");
                Console.WriteLine("MSSQL");
                Console.WriteLine("SQLITE");
                throw new ArgumentNullException();
            }

            if (sqlType.Equals("MYSQL", StringComparison.OrdinalIgnoreCase) || sqlType.Equals("MARIADB", StringComparison.OrdinalIgnoreCase)) {
                sql = new Mysql();
            }
            else if (sqlType.Equals("MSSQL", StringComparison.OrdinalIgnoreCase)) {
                sql = new Mssql();
            }
            else if (sqlType.Equals("SQLITE", StringComparison.OrdinalIgnoreCase)) {
                sql = new Sqlite(prgm_path);
            }
        }

        public void DeleteOldData() {
            sql.CheckTableExists();
            sql.DeleteOldData(Dns.GetHostName());
        }

        public void createList(string hkey, string bit) {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(hkey);
            try {
                if (key == null) {
                    return;
                }
                string[] programmIds = key.GetSubKeyNames();


                foreach (string programId in programmIds) {

                    RegistryKey programIdSubIds = key.OpenSubKey(programId);

                    string result = "";
                    int count = 0;
                    Dictionary<string, String> value = new Dictionary<string, string>();
                    for (var i = 0; i <= keyvaluenames.Length - 1; i++) {
                        value.Add(keyvaluenames[i], "'" + (string)programIdSubIds.GetValue(keyvaluenames[i]) + "'");
                        count++;
                    }

                    // result = String.Join("", value.ToArray());
                    // 
                    // if (result.EndsWith(",")) {
                    //     result = result.Remove(result.Length - 1);
                    // }

                    value.Add("PCID", "'" + Dns.GetHostName() + "'");
                    value.Add("update_date", "'" + DateTime.Now + "'");
                    value.Add("APP_Architecture", "'" + bit + "'");


                    sql.InsertOrUpdateData(value);
                }

            }
            catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
                System.Environment.Exit(1);
            }
        }
    }
}
