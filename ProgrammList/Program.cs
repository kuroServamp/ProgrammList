using Microsoft.Extensions.Logging;
using ProgrammList.ListPrograms;

class Program {
    static void Main(string[] args) {

        ListPrograms list = new ListPrograms();

            Console.WriteLine("Deleting old data");
        list.DeleteOldData();
            string keyname1 = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstal";
            Console.WriteLine("Searching for 32 bit");
            list.createList(keyname1, "x86");
            string keyname2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            Console.WriteLine("Searching for 64 bit");
            list.createList(keyname2, "x64");
        }
    }
