using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ProgrammList.sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace ProgrammList.ListPrograms {
    internal class ListPrograms {

        string prgm_path = Directory.GetCurrentDirectory() + "\\";
        string[] keyvaluenames = { "DisplayName", "DisplayVersion", "InstallDate"};
        Mysql sql;

        internal ListPrograms() { 
             sql = new Mysql();
        }

        internal void DeleteOldData() {
            sql.checkTableExists();
            sql.deleteOldData(Dns.GetHostName());
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
