using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using WPDevToolkit;
using WPDevToolkit.Interfaces;
using WPDevToolkit.Selection;

namespace HearthstoneCards.ViewModel
{
    public class MainViewModel : AsyncLoader, ILocatable
    {
        private readonly List<Card> _allCards; 

        public IList<SelectionItem<string>> ClassOptions { get; private set; }

        public ObservableRangeCollection<Card> FilterResults { get; private set; }
        private int _filterResultCount;

        public MainViewModel()
        {
            ClassOptions = new List<SelectionItem<string>>(new List<SelectionItem<string>>
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
            FilterResults = new ObservableRangeCollection<Card>();
            _allCards = new List<Card>();
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
                    OnQueryChanged();
                    return LoadResult.Success;
                }
                catch (Exception e)
                {
                    return new LoadResult(e.Message);
                }
            }
            return LoadResult.Success;
        }

        public void OnQueryChanged()
        {
            // TODO query
            // TODO remove fake query
            var result = 
                from card in _allCards
                where ClassOptions.Where(o => o.IsSelected).Any(o => o.Key.Equals(card.Class))
                orderby card.Cost ascending
                select card;

            FilterResults.Clear();
            FilterResults.AddRange(result);
            FilterResultCount = FilterResults.Count;
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
    }
}
