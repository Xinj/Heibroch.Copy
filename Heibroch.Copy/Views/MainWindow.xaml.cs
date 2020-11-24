using System;
using System.Diagnostics;
using System.Windows;
using Heibroch.Copy.ViewModels;

namespace Heibroch.Copy.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();                
                DataContext = new MainViewModel();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Heibroch.Copy", ex.StackTrace, EventLogEntryType.Error);
                throw ex;
            }            
        }
    }
}
