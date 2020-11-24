using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Heibroch.Common;
using Heibroch.Common.Wpf;
using Heibroch.Copy.Events;
using Heibroch.Copy.Plugin;
using Heibroch.Copy.Views;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace Heibroch.Copy.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventBus eventBus;
        private readonly ISettingCollection settingCollection;
        private readonly ICopyCollection copyCollection;
        private readonly IPluginLoader pluginLoader;
        private static ShortcutWindow currentShortcutWindow = null;
        private static ShortcutViewModel shortcutViewModel;
        private static SettingsWindow currentSettingsWindow = null;
        private static SettingsViewModel settingsViewModel;

        private bool isPasting = false;

        private DispatcherTimer dispatcherTimer;
        private TrayIcon trayIcon;

        public MainViewModel() : this(Container.Current.Resolve<IEventBus>(),
                                      Container.Current.Resolve<ICopyCollection>(),
                                      Container.Current.Resolve<ISettingCollection>(),
                                      Container.Current.Resolve<IPluginLoader>())
        {

        }

        public MainViewModel(IEventBus eventBus,
                             ICopyCollection copyCollection,
                             ISettingCollection settingCollection,
                             IPluginLoader pluginLoader)
        {
            this.eventBus = eventBus;
            this.copyCollection = copyCollection;
            this.settingCollection = settingCollection;
            this.pluginLoader = pluginLoader;
            Initialize();
        }

        private void Initialize()
        {
            shortcutViewModel = new ShortcutViewModel(copyCollection, pluginLoader, eventBus);
            settingsViewModel = new SettingsViewModel(settingCollection);

            eventBus.Subscribe<GlobalKeyPressed>(OnKeyPressed);
            eventBus.Subscribe<GlobalClipboardTextChanged>(OnClipboardTextChanged);

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += OnDispatcherTimerTick;
            dispatcherTimer.Start();

            trayIcon = new TrayIcon(OnTrayIconContextMenuItemClicked, new List<string> { "Settings", "Exit" });
        }

        private void OnTrayIconContextMenuItemClicked(string obj)
        {
            if (obj == Constants.ContextMenu.Exit)
            {
                trayIcon.Dispose();
                System.Windows.Application.Current.Shutdown();
            }

            if (obj == Constants.ContextMenu.Settings)
            {
                currentSettingsWindow = new SettingsWindow();
                currentSettingsWindow.DataContext = settingsViewModel;
                currentSettingsWindow.Show();
                currentSettingsWindow.Activate();
            }
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            if (currentShortcutWindow == null) 
                return;

            if (!currentShortcutWindow.IsActive)
                currentShortcutWindow.Activate();
        }

        private void OnClipboardTextChanged(GlobalClipboardTextChanged obj)
        {
            Console.WriteLine("Clipboard text changed!");
            copyCollection.Add(obj.Text);
            //Clipboard.Clear();
        }

        private void OnKeyPressed(GlobalKeyPressed obj)
        {
            if (currentShortcutWindow != null)
            {
                switch (obj.Key)
                {
                    case 0x0000001B: //Escape
                        obj.ProcessKey = false;
                        CloseShortcutWindow();
                        break;
                    case 0x00000028: //Down
                        shortcutViewModel?.IncrementSelection(1);
                        break;
                    case 0x00000026: //Up
                        shortcutViewModel?.IncrementSelection(-1);
                        break;
                    case 0x0000000D: //Enter
                        obj.ProcessKey = false;

                        //If the current shortcut window is open and doesn't have args, then execute it
                        if (currentShortcutWindow != null)
                        {
                            isPasting = true;

                            var selectedItem = shortcutViewModel.SelectedItem;
                            CloseShortcutWindow();
                            Clipboard.SetDataObject(selectedItem);

                            Thread.Sleep(5);
                            System.Windows.Forms.SendKeys.SendWait("^{v}");
                            Console.WriteLine("PASTED FROM SHORTCUT WINDOW!!!!");

                            isPasting = false;
                        }
                        break;
                }
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control) && obj.Key == (int)System.Windows.Forms.Keys.V)
            {
                currentShortcutWindow = new ShortcutWindow();
                currentShortcutWindow.DataContext = shortcutViewModel;
                currentShortcutWindow.Show();
                currentShortcutWindow.Activate();
                obj.ProcessKey = false; //Don't process key in order to avoid pasting
            }

            //if (Keyboard.Modifiers == (ModifierKeys.Control) && obj.Key == (int)System.Windows.Forms.Keys.V && isPasting)
            //{
            //    //currentShortcutWindow = new ShortcutWindow();
            //    //currentShortcutWindow.DataContext = shortcutViewModel;
            //    //currentShortcutWindow.Show();
            //    //currentShortcutWindow.Activate();
            //    Console.WriteLine("PASTED FROM KEYBOARD!!");
            //    obj.ProcessKey = false; //Don't process key in order to avoid pasting
            //}

        }

        private void CloseShortcutWindow()
        {
            shortcutViewModel.Reset();
            currentShortcutWindow?.Close();
            currentShortcutWindow = null;
        }
    }
}