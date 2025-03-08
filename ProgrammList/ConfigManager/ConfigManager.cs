using System.Configuration;

namespace ProgrammList.ConfigManager {
    public static class PrmListConfigManager {

        public static string GetSetting(string key) {
            try {
                if (!File.Exists(Directory.GetCurrentDirectory() + "\\app.conf")) {
                    return "";
                }
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = Directory.GetCurrentDirectory() + "\\app.conf";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                return config.AppSettings.Settings[key].Value;
            }
            catch (Exception e) {
                System.Environment.Exit(13);
            }
            return "13";
        }

        public static void SetSetting(string key, string value) {
            try {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = Directory.GetCurrentDirectory() + "\\app.conf";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                config.AppSettings.Settings["key"].Value = value;
            }
            catch (Exception e) {
                // Exception
            }
        }
    }
}