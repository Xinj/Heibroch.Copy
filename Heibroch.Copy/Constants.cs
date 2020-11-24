using System;

namespace Heibroch.Copy
{
    public class Constants
    {
        public static string RootPath = Environment.GetEnvironmentVariable("APPDATA"/*"LocalAppData"*/) + "\\Heibroch\\";

        public static string ShortcutFileName = "Shortcuts";

        public static string ShortcutFileExtension = ".hscut";

        public static string SettingFileName = "CopySettings";

        public static string SettingFileExtension = ".hcset";

        public const string CommandLineCommand = "[CMD]";

        public const string RemoteCommand = "[Remote]";

        public const string ApplicationName = "Heibroch.Copy";

        public const string SearchLocation = "[SearchPath]";

        public const string ReloadCommand = "Reload";

        public const int MaxResultCount = 10;

        public class ContextMenu
        {
            public const string Exit = "Exit";

            public const string Settings = "Settings";
        }
    }
}
