using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using WPDevToolkit;
using WPDevToolkit.Interfaces;
using WPDevToolkit.Selection;

namespace HearthstoneCards.ViewModel
{
    public class MainViewModel : AsyncLoader, ILocatable, IIncrementalSource<Card>, IViewInfoProvider
    {
        public ChangeViewInfoCommand ChangeViewInfoCommand { get; private set; }
        private ItemsControlViewInfo _itemsControlViewInfo;

        public string NameFilter { get; set; }
        public bool _isAttackFilterEnabled;
        public IList<ImageSelectionItem<string>> ClassOptions { get; private set; }
        public IList<ImageSelectionItem<Set>> SetOptions { get; private set; }
        public IList<ImageSelectionItem<Rarity>> RarityOptions { get; private set; }
        public IList<int> AttackOptions { get; private set; }
        public IList<ISelectionItem<Func<Card, object>>> SortOptions { get; private set; }
        public IncrementalObservableCollection<MainViewModel, Card> PresentedCards { get { return _presentedCards; } }

        private ISelectionItem _selectedSortOption;
        public int SelectedAttackFromOption { get; set; }
        public int SelectedAttackToOption { get; set; }

        private readonly List<Card> _allCards;                                                  // whole DB
        private readonly List<Card> _filteredCards;                                             // filtered DB
        private readonly IncrementalObservableCollection<MainViewModel, Card> _presentedCards;  // presented filtered -> sorted, incrementally loaded
        private int _resultsCount;

        private bool _isSortedAscending;
        private bool _isSortingControlVisible;
        private bool _isSortConfigurationChanged;
        private bool _isIncrementalLoading;
        private bool _isResultEmpty = true;

        private bool _isNameFilterEnabled;

        public MainViewModel()
        {
            ChangeViewInfoCommand = new ChangeViewInfoCommand(this);

            // TODO get options from DB
            ClassOptions = new List<ImageSelectionItem<string>>
            {
                new ImageSelectionItem<string>("Neutral"),
                new ImageSelectionItem<string>("Druid") { ImagePath = "../Assets/Icons/Classes/druid.png"},
                new ImageSelectionItem<string>("Hunter") { ImagePath = "../Assets/Icons/Classes/hunter.png"},
                new ImageSelectionItem<string>("Mage") { ImagePath = "../Assets/Icons/Classes/mage.png"},
                new ImageSelectionItem<string>("Paladin") { ImagePath = "../Assets/Icons/Classes/paladin.png"},
                new ImageSelectionItem<string>("Priest") { ImagePath = "../Assets/Icons/Classes/priest.png"},
                new ImageSelectionItem<string>("Rogue") { ImagePath = "../Assets/Icons/Classes/rogue.png"},
                new ImageSelectionItem<string>("Shaman") { ImagePath = "../Assets/Icons/Classes/shaman.png"},
                new ImageSelectionItem<string>("Warlock") { ImagePath = "../Assets/Icons/Classes/warlock.png"},
                new ImageSelectionItem<string>("Warrior") { ImagePath = "../Assets/Icons/Classes/warrior.png"},
            };
            SetOptions = new List<ImageSelectionItem<Set>>
            {
                new ImageSelectionItem<Set>("Basic", Set.Basic) { ImagePath = "../Assets/Icons/Sets/basic-60.png"},
                new ImageSelectionItem<Set>("Classic", Set.Classic) { ImagePath = "../Assets/Icons/Sets/classic-60.png"},
                new ImageSelectionItem<Set>("Curse of Naxxramas", Set.Naxxramas) { ImagePath = "../Assets/Icons/Sets/naxx-60.png"},
                new ImageSelectionItem<Set>("Goblins vs Gnomes", Set.GoblinVsGnomes) { ImagePath = "../Assets/Icons/Sets/gvg-60.png"},
                new ImageSelectionItem<Set>("Blackrock Mountain", Set.BlackrockMountain) { ImagePath = "../Assets/Icons/Sets/brm-60.png"},
                new ImageSelectionItem<Set>("The Grand Tournament", Set.TheGrandTournament) { ImagePath = "../Assets/Icons/Sets/tgt-60.png"},
                new ImageSelectionItem<Set>("League of Explorers", Set.LeagueOfExplorers) { ImagePath = "../Assets/Icons/Sets/loe-60.png"},
            };
            RarityOptions = new List<ImageSelectionItem<Rarity>>
            {
                new ImageSelectionItem<Rarity>("Free", Rarity.Free),
                new ImageSelectionItem<Rarity>("Common", Rarity.Common) { ImagePath = "../Assets/Icons/Rarity/common.png"},
                new ImageSelectionItem<Rarity>("Rare", Rarity.Rare) { ImagePath = "../Assets/Icons/Rarity/rare.png"},
                new ImageSelectionItem<Rarity>("Epic", Rarity.Epic) { ImagePath = "../Assets/Icons/Rarity/epic.png"},
                new ImageSelectionItem<Rarity>("Legendary", Rarity.Legendary) { ImagePath = "../Assets/Icons/Rarity/legendary.png"},
            };
            SortOptions = new List<ISelectionItem<Func<Card, object>>>
            {
                new SelectionItem<Func<Card, object>>("Attack", c => c.Attack),
                new SelectionItem<Func<Card, object>>("Class", c => c.Class),
                new SelectionItem<Func<Card, object>>("Cost", c => c.Cost),
                new SelectionItem<Func<Card, object>>("Faction", c => c.Faction),
                new SelectionItem<Func<Card, object>>("Health", c => c.Health),
                new SelectionItem<Func<Card, object>>("Name", c => c.Name),
                new SelectionItem<Func<Card, object>>("Race", c => c.Race),
                new SelectionItem<Func<Card, object>>("Rarity", c => c.Rarity),
                new SelectionItem<Func<Card, object>>("Set", c => c.Set),
                new SelectionItem<Func<Card, object>>("Text", c => c.Text),
                new SelectionItem<Func<Card, object>>("Type", c => c.Type)
            };
            AttackOptions = new List<int>();
            for (var i = 1; i <= 10; i++)
            {
                AttackOptions.Add(i);   
            }

            _allCards = new List<Card>();
            _filteredCards = new List<Card>();
            _presentedCards = new IncrementalObservableCollection<MainViewModel, Card>(this, 5);
            _presentedCards.CollectionChanged += PresentedResultsOnCollectionChanged;
            
            var settings = new AppSettings();
            IsSortedAscending = settings.IsSortedAscending;
            IsAttackFilterEnabled = settings.IsAttackFilterEnabled;
            SelectedAttackFromOption = settings.AttackFromSelection;
            SelectedAttackToOption = settings.AttackToSelection;
            LoadSelection(ClassOptions, AppSettings.ClassSelectionKey);
            LoadSelection(SetOptions, AppSettings.SetSelectionKey);
            LoadSelection(RarityOptions, AppSettings.RaritySelectionKey);
            LoadSelection(SortOptions, AppSettings.SortOptionsSelectionKey);
            SelectedSortOption = SortOptions.First(o => o.IsSelected);
        }

        public async void PresentedResultsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            // TODO correct for move/replace (use provided indices)
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // ObservableRangeCollection -> adds multiple items
                    await LoadCardImages(args.NewItems.Cast<Card[]>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    //await LoadCardImages((Card[]) args.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    //await LoadCardImages((Card[]) args.NewItems);
                    break;
            }
        }

        private static async Task LoadCardImages(IEnumerable<Card[]> cardArrays)
        {
            var tasks = new List<Task>();
            foreach (var array in cardArrays)
            {
                foreach (var card in array)
                {
                    tasks.Add(card.LoadImageAsync());
                }
            }
            await Task.WhenAll(tasks);
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
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AllSets.enUS.json"));
                    using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
                    {
                        fileContent = await reader.ReadToEndAsync();
                    }
                    var json = await new StringReader(fileContent).ReadToEndAsync();
                    if (json != null)
                    {
                        var globalCollection = JsonConvert.DeserializeObject<GlobalCollection>(json);
                        foreach (var set in globalCollection.CardSets)
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
            // TODO make parallel
            // TODO add option for IsCollectible
            // TODO optimize by reducing entries as fast as possible
            // filter by currently selected filter options
            var filtered =
                from card in _allCards
                where card.IsCollectible
                where !IsNameFilterEnabled || card.Name.Contains(NameFilter)
                where ClassOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Class))
                where SetOptions.Where(o => o.IsSelected).Any(o => o.Value.Equals(card.Set))
                where RarityOptions.Where(o => o.IsSelected).Any(o => o.Value.Equals(card.Rarity))
                //where card.Attack >= SelectedAttackFromOption && card.Attack <= SelectedAttackToOption
                select card;

            await SortAndPresentAsync(filtered.ToList());
            
            // store filter settings
            SaveSettings();
        }

        private async Task SortAndPresentAsync(IEnumerable<Card> filtered)
        {
            // sort by currently selected sort options
            var sortOption = SortOptions.First(o => o.IsSelected);
            var sorted = _isSortedAscending ? filtered.OrderBy(sortOption.Value).ToList() : filtered.OrderByDescending(sortOption.Value).ToList();
            SelectedSortOption = sortOption;

            // update filtered DB
            _filteredCards.Clear();
            _filteredCards.AddRange(sorted);
            ResultsCount = _filteredCards.Count;
            IsResultEmpty = _filteredCards.Count == 0;

            // present results, start incremental loading
            _presentedCards.Clear();
            await _presentedCards.LoadMoreItemsAsync();
        }

        public void ToggleSorterControlVisibility()
        {
            IsSortingControlVisible = !IsSortingControlVisible;
        }

        public async Task ApplySortAsync()
        {
            // only sort if necessary
            if (IsSortConfigurationChanged)
            {
                IsSortConfigurationChanged = false;
                IsSortingControlVisible = false;

                // apply sort on existing filtered DB
                await SortAndPresentAsync(_filteredCards);
                // TODO "jump to current item"
            }
        }

        public async Task<IEnumerable<Card>> GetPagedItems(int pageIndex, int pageSize)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsIncrementalLoading = true);
            try
            {
                var index = pageIndex * pageSize;
                var pagedItems = new List<Card>(pageSize);
                for (var i = index; i < index + pageSize && i < _filteredCards.Count; i++)
                {
                    pagedItems.Add(_filteredCards[i]);
                }
                return pagedItems;
            }
            finally
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsIncrementalLoading = false);
            }
        }

        private void SaveSettings()
        {
            var settings = new AppSettings();
            StoreSelection(ClassOptions, AppSettings.ClassSelectionKey);
            StoreSelection(SetOptions, AppSettings.SetSelectionKey);
            StoreSelection(RarityOptions, AppSettings.RaritySelectionKey);
            settings.ItemsControlViewInfoIndex = ItemsControlViewInfo.Id;
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

        public ItemsControlViewInfo ItemsControlViewInfo
        {
            get { return _itemsControlViewInfo; }
            set
            {
                if (_itemsControlViewInfo != value)
                {
                    _itemsControlViewInfo = value;
                    SaveSettings();
                    NotifyPropertyChanged();
                }
            }
        }

        public int ResultsCount
        {
            get { return _resultsCount; }
            private set
            {
                if (_resultsCount != value)
                {
                    _resultsCount = value;
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

        public ISelectionItem SelectedSortOption
        {
            get { return _selectedSortOption; }
            private set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsNameFilterEnabled
        {
            get { return _isNameFilterEnabled; }
            set
            {
                if (_isNameFilterEnabled != value)
                {
                    _isNameFilterEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsAttackFilterEnabled
        {
            get { return _isAttackFilterEnabled; }
            set
            {
                if (_isAttackFilterEnabled != value)
                {
                    _isAttackFilterEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
