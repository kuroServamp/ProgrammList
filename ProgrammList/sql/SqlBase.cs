using System.Collections.Generic;

namespace ProgrammList.sql {

    interface SqlBase {


        bool GetSingleLine(string pcid, string program, string version);

        void CheckTableExists();



        void InsertData(Dictionary<string, string> valuesqlCommand);


        void InsertOrUpdateData(Dictionary<string, string> value);

        void DeleteOldData(string hostname);
        void UpdateData(Dictionary<string, string> value);


    }

}
