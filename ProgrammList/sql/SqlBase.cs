namespace ProgrammList.sql {

    interface SqlBase {


        internal bool GetSingleLine(string pcid, string program, string version);

        internal void CheckTableExists();

        internal void InsertData(Dictionary<string, string> valuesqlCommand);

        internal void InsertOrUpdateData(Dictionary<string, string> value);

        internal void DeleteOldData(string hostname);

        internal void UpdateData(Dictionary<string, string> value);


    }

}
