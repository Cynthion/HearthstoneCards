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
using HearthstoneCards.Model;
using Newtonsoft.Json;
using WPDevToolkit;
using WPDevToolkit.Interfaces;

namespace HearthstoneCards.ViewModel
{
    public class MainViewModel : AsyncLoader, ILocatable, IIncrementalSource<Card>
    {
        private readonly List<Card> _allCards; 
        private readonly List<Card> _filteredResults;
        public IncrementalObservableCollection<MainViewModel, Card> PresentedResults { get; private set; }

        public ObservableCollection<ImageSelectionItem<string>> ClassOptions { get; private set; }
        public ObservableCollection<ImageSelectionItem<string>> SetOptions { get; private set; }
        public ObservableCollection<ImageSelectionItem<string>> RarityOptions { get; private set; }

        private int _filterResultCount;
        private bool _isIncrementalLoading;

        public MainViewModel()
        {
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
            // TODO get sets from DB
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
            // TODO make parallel
            // TODO add option for collectible
            var result =
                from card in _allCards
                where card.IsCollectible
                where ClassOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Class))
                where SetOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.CardSet))
                where RarityOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Rarity))
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
