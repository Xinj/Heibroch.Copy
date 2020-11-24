namespace Heibroch.Copy.Plugin
{
    public interface ILaunchPlugin
    {
        void OnProgramLoaded();

        void OnShortcutsLoaded();


        /// <summary>
        /// At the start of the description. If there's a match, it will create the shortcut via the given plugin
        /// </summary>
        string ShortcutFilter { get; }
    }
}
