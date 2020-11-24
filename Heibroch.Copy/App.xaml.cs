using Heibroch.Common;
using Heibroch.Copy.Events;
using Heibroch.Copy.Plugin;
using Heibroch.WindowsInteractivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Heibroch.Copy
{
    public partial class App : Application
    {
        EventBus eventBus;

        public App()
        {
            //Fixes an issue with current directory being system32 for the plugin loader and not the application path as desired
            EventLog.WriteEntry("Heibroch.Copy - Initializing", "Setting base directory...", EventLogEntryType.Information);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            EventLog.WriteEntry("Heibroch.Copy - Initializing", "Initializing eventbus...", EventLogEntryType.Information);
            eventBus = new EventBus();
            Container.Current.Register<IEventBus>(eventBus);

            EventLog.WriteEntry("Heibroch.Copy - Initializing", "Initializing plugin loader...", EventLogEntryType.Information);
            var pluginLoader = new PluginLoader();
            Container.Current.Register<IPluginLoader>(pluginLoader);

            EventLog.WriteEntry("Heibroch.Copy - Initializing", "Initializing shortcut collection...", EventLogEntryType.Information);
            var copyCollection = new CopyCollection();
            Container.Current.Register<ICopyCollection>(copyCollection);
            
            EventLog.WriteEntry("Heibroch.Copy - Initializing", "Initializing setting collection...", EventLogEntryType.Information);
            var settingCollection = new SettingCollection();
            Container.Current.Register<ISettingCollection>(settingCollection);

            EventLog.WriteEntry("Heibroch.Copy - Initializing", "Initializing plugins...", EventLogEntryType.Information);
            pluginLoader.Load();

            foreach (var plugin in pluginLoader.Plugins)
            {
                try
                {
                    plugin.OnProgramLoaded();
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show($"{exception.ToString()}\r\n{exception.StackTrace}");
                }
            }

            KeyboardInterop.KeyPressed += KeyboardInterop_KeyPressed;
            ClipboardInterop.ClipboardUpdate += ClipboardInterop_ClipboardUpdate;
        }

        private void KeyboardInterop_KeyPressed(object sender, KeyPressed e) => eventBus.Publish(new GlobalKeyPressed(e));

        private void ClipboardInterop_ClipboardUpdate(object sender, ClipboardUpdated e)
        {
            if (Clipboard.ContainsText())
            {
                eventBus.Publish(new GlobalClipboardTextChanged() { Text = TryGetText() });
            }
        }

        void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var comException = e.Exception as System.Runtime.InteropServices.COMException;
            if (comException != null && comException.ErrorCode == -2147221040)
                e.Handled = true;
        }

        private string TryGetText()
        {
            var attempts = 3;
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    return (string)Clipboard.GetDataObject().GetData(typeof(string));
                }
                catch
                {
                    if (i + 1 >= attempts)
                        MessageBox.Show("An error ocurred. Try again");

                    Thread.Sleep(5);
                    continue;
                }
            }

            return string.Empty;
        }
    }
}