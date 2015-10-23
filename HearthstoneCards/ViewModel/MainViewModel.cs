using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using WPDevToolkit;
using WPDevToolkit.Interfaces;
using WPDevToolkit.Selection;

namespace HearthstoneCards.ViewModel
{
    public class MainViewModel : AsyncLoader, ILocatable, IIncrementalSource<Card>
    {
        private readonly List<Card> _allCards; 
        private readonly List<Card> _filteredResults;
        public IncrementalObservableCollection<MainViewModel, Card> PresentedResults { get; private set; }

        public ObservableCollection<SelectionItem<string>> ClassOptions { get; private set; }

        private int _filterResultCount;
        private bool _isIncrementalLoading;

        public MainViewModel()
        {
            ClassOptions = new ObservableCollection<SelectionItem<string>>(new List<SelectionItem<string>>
            {
                new SelectionItem<string>("Druid", "../Assets/Icons/Classes/druid.png"),
                new SelectionItem<string>("Hunter", "../Assets/Icons/Classes/hunter.png"),
                new SelectionItem<string>("Mage", "../Assets/Icons/Classes/mage.png"),
                new SelectionItem<string>("Paladin", "../Assets/Icons/Classes/paladin.png"),
                new SelectionItem<string>("Priest", "../Assets/Icons/Classes/priest.png"),
                new SelectionItem<string>("Rogue", "../Assets/Icons/Classes/rogue.png"),
                new SelectionItem<string>("Shaman", "../Assets/Icons/Classes/shaman.png"),
                new SelectionItem<string>("Warlock", "../Assets/Icons/Classes/warlock.png"),
                new SelectionItem<string>("Warrior", "../Assets/Icons/Classes/warrior.png")
            });
            _allCards = new List<Card>();
            _filteredResults = new List<Card>();
            PresentedResults = new IncrementalObservableCollection<MainViewModel, Card>(this, 5);
            PresentedResults.CollectionChanged += PresentedResultsOnCollectionChanged;
        }

        public async void PresentedResultsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                // load card images
                var tasks = new List<Task>(args.NewItems.Count);
                foreach (Card[] newItems in args.NewItems)
                {
                    foreach (var card in newItems)
                    {
                        tasks.Add(card.LoadImageAsync());
                    }
                }
                await Task.WhenAll(tasks);
            }
        }

        protected override async Task<LoadResult> DoLoadAsync()
        {
            if (_allCards.Count == 0)
            {
                // TODO check if needs to be re-newed (serialized date)
                // var api = SingletonLocator.Get<ApiCaller>();

                // load from local storage
                try
                {
                    // TODO load from storage, not from file
                    string fileContent;
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/TestDb.txt"));
                    using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
                    {
                        fileContent = await reader.ReadToEndAsync();
                    }
                    var json = await new StringReader(fileContent).ReadToEndAsync();
                    if (json != null)
                    {
                        var globalCollection = JsonConvert.DeserializeObject<GlobalCollection>(json);
                        foreach (var set in globalCollection.Sets)
                        {
                            _allCards.AddRange(set.Cards);
                        }
                    }

                    // TODO remove
                    await OnQueryChangedAsync();
                    return LoadResult.Success;
                }
                catch (Exception e)
                {
                    return new LoadResult(e.Message);
                }
            }
            return LoadResult.Success;
        }

        public async Task OnQueryChangedAsync()
        {
            // TODO query
            // TODO remove fake query
            var result = 
                from card in _allCards
                where ClassOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Class))
                orderby card.Cost ascending
                select card;

            _filteredResults.Clear();
            PresentedResults.Clear();

            _filteredResults.AddRange(result);
            FilterResultCount = _filteredResults.Count;

            // present results
            PresentedResults.LoadMoreItemsAsync();
        }

        public async Task<IEnumerable<Card>> GetPagedItems(int pageIndex, int pageSize)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsIncrementalLoading = true);
            try
            {
                var index = pageIndex * pageSize;
                var pagedItems = new List<Card>(pageSize);
                for (var i = index; i < index + pageSize && i < _filteredResults.Count; i++)
                {
                    pagedItems.Add(_filteredResults[i]);
                }
                return pagedItems;
            }
            finally
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsIncrementalLoading = false);
            }
        }

        public int FilterResultCount
        {
            get { return _filterResultCount; }
            private set
            {
                if (_filterResultCount != value)
                {
                    _filterResultCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsIncrementalLoading
        {
            get { return _isIncrementalLoading; }
            private set
            {
                if (_isIncrementalLoading != value)
                {
                    _isIncrementalLoading = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
