using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using HearthstoneCards.CollectionViewEx;
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
        private readonly List<Card> _allCards;                                  // whole DB
        private readonly List<Card> _filteredCards;                             // filtered DB
        private readonly IncrementalObservableCollection<MainViewModel, Card> _presentedCards;
        //private readonly CollectionViewEx.CollectionViewEx _presentedCards;     // presented cards -> sorted, incrementally loaded
        //public ICollectionView PresentedCards
        //{
        //    get { return _presentedCards; }
        //}

        //private readonly List<Card> _filteredResults;
        //public IncrementalObservableCollection<MainViewModel, Card> PresentedResults { get; private set; }

        public ObservableCollection<ImageSelectionItem<string>> ClassOptions { get; private set; }
        public ObservableCollection<ImageSelectionItem<string>> SetOptions { get; private set; }
        public ObservableCollection<ImageSelectionItem<string>> RarityOptions { get; private set; }

        public IList<ISelectionItem> SortOptions { get; private set; }

        private bool _isIncrementalLoading;
        private bool _isResultEmpty = true;
        private bool _isSortingControlVisible;
        private bool _isSortedAscending;
        private bool _isSortConfigurationChanged;

        public MainViewModel()
        {
            // TODO get options from DB
            ClassOptions = new ObservableCollection<ImageSelectionItem<string>>(new List<ImageSelectionItem<string>>
            {
                new ImageSelectionItem<string>("Druid") { ImagePath = "../Assets/Icons/Classes/druid.png"},
                new ImageSelectionItem<string>("Hunter") { ImagePath = "../Assets/Icons/Classes/hunter.png"},
                new ImageSelectionItem<string>("Mage") { ImagePath = "../Assets/Icons/Classes/mage.png"},
                new ImageSelectionItem<string>("Paladin") { ImagePath = "../Assets/Icons/Classes/paladin.png"},
                new ImageSelectionItem<string>("Priest") { ImagePath = "../Assets/Icons/Classes/priest.png"},
                new ImageSelectionItem<string>("Rogue") { ImagePath = "../Assets/Icons/Classes/rogue.png"},
                new ImageSelectionItem<string>("Shaman") { ImagePath = "../Assets/Icons/Classes/shaman.png"},
                new ImageSelectionItem<string>("Warlock") { ImagePath = "../Assets/Icons/Classes/warlock.png"},
                new ImageSelectionItem<string>("Warrior") { ImagePath = "../Assets/Icons/Classes/warrior.png"},
            });
            SetOptions = new ObservableCollection<ImageSelectionItem<string>>(new List<ImageSelectionItem<string>>
            {
                new ImageSelectionItem<string>("Basic"),
                new ImageSelectionItem<string>("Classic") { ImagePath = "../Assets/Icons/Sets/classic.png"},
                new ImageSelectionItem<string>("Naxxramas") { ImagePath = "../Assets/Icons/Sets/naxx.png"},
                new ImageSelectionItem<string>("Goblins vs Gnomes") { ImagePath = "../Assets/Icons/Sets/gvg.png"},
                new ImageSelectionItem<string>("Blackrock Mountain") { ImagePath = "../Assets/Icons/Sets/blackrock.png"},
                new ImageSelectionItem<string>("The Grand Tournament") { ImagePath = "../Assets/Icons/Sets/tgt.png"},
            });
            RarityOptions = new ObservableCollection<ImageSelectionItem<string>>(new List<ImageSelectionItem<string>>
            {
                new ImageSelectionItem<string>("Free"),
                new ImageSelectionItem<string>("Common") { ImagePath = "../Assets/Icons/Rarity/common.png"},
                new ImageSelectionItem<string>("Rare") { ImagePath = "../Assets/Icons/Rarity/rare.png"},
                new ImageSelectionItem<string>("Epic") { ImagePath = "../Assets/Icons/Rarity/epic.png"},
                new ImageSelectionItem<string>("Legendary") { ImagePath = "../Assets/Icons/Rarity/legendary.png"},
            });
            SortOptions = new List<ISelectionItem>
            {
                new SelectionItem<string>("Cost", "Cost"),
                new SelectionItem<string>("Attack", "Attack"),
                new SelectionItem<string>("Health", "Health")
            };

            _allCards = new List<Card>();
            _filteredCards = new List<Card>();
            _presentedCards = new IncrementalObservableCollection<MainViewModel, Card>(this, 5);
            _presentedCards.CollectionChanged += PresentedResultsOnCollectionChanged;
            
            IsSortedAscending = BaseSettings.Load<bool>(AppSettings.IsSortedAscendingKey);
            LoadSelection(SortOptions, AppSettings.SortOptionsSelectionKey);
            LoadSelection(ClassOptions, AppSettings.ClassSelectionKey);
            LoadSelection(SetOptions, AppSettings.SetSelectionKey);
            LoadSelection(RarityOptions, AppSettings.RaritySelectionKey);
        }

        private static void LoadSelection<T>(IList<T> options, string settingKey) where T : ISelectionItem
        {
            var selections = BaseSettings.Load<bool[]>(settingKey);
            for (var i = 0; i < options.Count && i < selections.Length; i++)
            {
                options[i].IsSelected = selections[i];
            }
        }

        private static void StoreSelection<T>(IList<T> options, string settingKey) where T : ISelectionItem
        {
            var selections = new bool[options.Count];
            for (var i = 0; i < options.Count; i++)
            {
                selections[i] = options[i].IsSelected;
            }
            BaseSettings.Store(settingKey, selections);
        }

        public async void PresentedResultsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                // load card images
                var tasks = new List<Task>(args.NewItems.Count);
                foreach (Card[] cards in args.NewItems)
                {
                    foreach (var card in cards)
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
                // load from local storage
                try
                {
                    // TODO load from storage, not from file
                    string fileContent;
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/AllSets.json"));
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

        // TODO use as CollectionViewEx filter-predicate
        public async Task OnQueryChangedAsync()
        {
            // update presented results from filtered collection view
            PresentedCards.Clear();

            // TODO make parallel
            // TODO add option for IsCollectible
            var result =
                from card in _allCards
                where card.IsCollectible
                where ClassOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Class))
                where SetOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Set))
                where RarityOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Rarity))
                orderby card.Cost ascending
                select card;

            PresentedResults.Clear();

            //_filteredResults.AddRange(result);
            //FilterResultCount = _filteredResults.Count;
            //IsResultEmpty = FilterResultCount == 0;

            //// present results
            //PresentedResults.LoadMoreItemsAsync();

            //// store filter settings
            //StoreSelection(ClassOptions, AppSettings.ClassSelectionKey);
            //StoreSelection(SetOptions, AppSettings.SetSelectionKey);
            //StoreSelection(RarityOptions, AppSettings.RaritySelectionKey);
        }

        public void ToggleSorterControlVisibility()
        {
            IsSortingControlVisible = !IsSortingControlVisible;
        }

        public void ApplySort()
        {
            // only sort if necessary
            if (IsSortConfigurationChanged)
            {
                IsSortConfigurationChanged = false;
                IsSortingControlVisible = false;

                var sortOption = SortOptions.First(o => o.IsSelected);
                var sortDescription = new SortDescription(sortOption.Key, IsSortedAscending ? SortDirection.Ascending : SortDirection.Descending);
            
                var sortCommand = new SortCommand(_allCards);
                sortCommand.Execute(sortDescription);
            }
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

        public bool IsSortingControlVisible
        {
            get { return _isSortingControlVisible; }
            private set
            {
                if (_isSortingControlVisible != value)
                {
                    _isSortingControlVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSortedAscending
        {
            get { return _isSortedAscending; }
            // mode: two-way
            set
            {
                if (_isSortedAscending != value)
                {
                    _isSortedAscending = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSortConfigurationChanged
        {
            get { return _isSortConfigurationChanged; }
            set
            {
                if (_isSortConfigurationChanged != value)
                {
                    _isSortConfigurationChanged = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsResultEmpty
        {
            get { return _isResultEmpty; }
            private set
            {
                if (_isResultEmpty != value)
                {
                    _isResultEmpty = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
