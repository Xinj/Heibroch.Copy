using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Heibroch.Common;
using Heibroch.Common.Wpf;
using Heibroch.Copy.Plugin;

namespace Heibroch.Copy.ViewModels
{
    public class ShortcutViewModel : ViewModelBase
    {
        private readonly ICopyCollection copyCollection;
        private readonly IPluginLoader pluginLoader;
        private readonly IEventBus eventBus;
        private readonly SelectionCycler selectionCycler;
        private string selectedItem;

        public ShortcutViewModel(ICopyCollection copyCollection, 
                                 IPluginLoader pluginLoader,
                                 IEventBus eventBus)
        {
            this.copyCollection = copyCollection;
            this.pluginLoader = pluginLoader;
            this.eventBus = eventBus;
            selectionCycler = new SelectionCycler();

            Initialize();
        }

        private void Initialize() => pluginLoader.Plugins.ForEach(x => x.OnShortcutsLoaded());

        public List<string> DisplayedQueryResults => new List<string>(selectionCycler.SubSelect(QueryResults));

        public List<string> QueryResults => copyCollection.QueryResults;

        public string SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }

        public void Reset()
        {
            selectionCycler.Reset();
            SelectedItem = DisplayedQueryResults.FirstOrDefault();
        }

        public void IncrementSelection(int increment)
        {
            if (DisplayedQueryResults.Count <= 0) return;

            //Move/cycle visibility/shortcuts
            selectionCycler.Increment(increment, copyCollection.QueryResults.Count);

            if (string.IsNullOrWhiteSpace(SelectedItem))
            {
                SelectedItem = DisplayedQueryResults.First();
                return;
            }

            if (!DisplayedQueryResults.Contains(SelectedItem) && DisplayedQueryResults.Any())
            {
                SelectedItem = DisplayedQueryResults.First();
                return;
            }

            if (DisplayedQueryResults.Count == 1)
            {
                SelectedItem = DisplayedQueryResults.First();
                return;
            }

            var index = DisplayedQueryResults.IndexOf(SelectedItem) + increment;
            if (index >= DisplayedQueryResults.Count || index < 0) return;

            RaisePropertyChanged(nameof(DisplayedQueryResults));

            SelectedItem = DisplayedQueryResults.ElementAt(selectionCycler.CycleIndex);

            Debug.WriteLine($"SelectedItem now {SelectedItem} \r\n" +
                            $"StartIndex {selectionCycler.StartIndex}\r\n" +
                            $"StopIndex {selectionCycler.StopIndex}\r\n" +
                            $"TotalIndex {selectionCycler.CurrentIndex}\r\n" +
                            $"CycleIndex {selectionCycler.CycleIndex}\r\n" +
                            $"Delta {selectionCycler.Delta}\r\n" +
                            $"CollectionCount {copyCollection.QueryResults.Count}" +
                            $"-----------------------------------------------");
        }

        public Visibility QueryResultsVisibility => QueryResults.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility WaterMarkVisibility => Visibility.Hidden;
    }
}
