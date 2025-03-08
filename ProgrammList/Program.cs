using ProgrammList.ConfigManager;
using ProgrammList.ListPrograms;

class Program {
    public static void Main(string[] args) {

        string dbType = PrmListConfigManager.GetSetting("DB_Type");
        ListPrograms list = new ListPrograms(dbType);

        list.DeleteOldData();
        string keyname1 = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstal";
        list.createList(keyname1, "x86");
        string keyname2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        list.createList(keyname2, "x64");
    }


}
