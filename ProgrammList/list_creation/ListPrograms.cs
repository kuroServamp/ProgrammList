using Microsoft.Win32;
using ProgrammList.ConfigManager;
using ProgrammList.sql;
using System.Net;

namespace ProgrammList.ListPrograms {
    internal class ListPrograms {

        string prgm_path = Directory.GetCurrentDirectory() + "\\";
        string[] keyvaluenames = { "DisplayName", "DisplayVersion", "InstallDate" };
        private SqlBase sql = null;


        internal ListPrograms(string sqlType, params string[] filename) {
            if (sqlType == null) {
                Console.WriteLine("SQL Database not defined in app.conf, allowed types:");
                Console.WriteLine("MYSQL");
                Console.WriteLine("MARIADB");
                Console.WriteLine("MSSQL");
                Console.WriteLine("SQLITE");
                throw new ArgumentNullException();
            }

            string sqlname = "";
            if (sqlType.Equals("MYSQL", StringComparison.OrdinalIgnoreCase) || sqlType.Equals("MARIADB", StringComparison.OrdinalIgnoreCase)) {
                string server = PrmListConfigManager.GetSetting("server");
                string database = PrmListConfigManager.GetSetting("DB");
                string user = PrmListConfigManager.GetSetting("user");
                string pw = PrmListConfigManager.GetSetting("pw");
                sql = new Mysql(server, database, user, pw);
                sqlname = "MySQL/MariaDB";
            }
            else if (sqlType.Equals("MSSQL", StringComparison.OrdinalIgnoreCase)) {
                string server = PrmListConfigManager.GetSetting("server");
                string database = PrmListConfigManager.GetSetting("DB");
                string user = PrmListConfigManager.GetSetting("user");
                string pw = PrmListConfigManager.GetSetting("pw");
                sql = new Mssql(server, user, pw, database);
                sqlname = "MSSQL";
            }
            else if (sqlType.Equals("SQLITE", StringComparison.OrdinalIgnoreCase) && filename != null) {
                sql = new Sqlite(prgm_path, filename[0]);
                sqlname = "SQLITE";
            }
            else {
                // Default sqlite im gleichen ordner
                sql = new Sqlite(prgm_path, "sqllite.db");
                sqlname = "fallback default SQLITE";
            }

            Console.WriteLine();
        }

        internal void DeleteOldData() {
            sql.CheckTableExists();
            sql.DeleteOldData(Dns.GetHostName());
        }

        internal void createList(string hkey, string bit) {
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

                    result = String.Join("", value.ToArray());

                    if (result.EndsWith(",")) {
                        result = result.Remove(result.Length - 1);
                    }

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
