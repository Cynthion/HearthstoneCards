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
        private GlobalCollection _globalCollection;

        public IList<SelectionItem<string>> Classes { get; private set; }

        public ObservableRangeCollection<Card> FilterResults { get; private set; }

        public MainViewModel()
        {
            Classes = new List<SelectionItem<string>>(new List<SelectionItem<string>>
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
        }

        protected override async Task<LoadResult> DoLoadAsync()
        {
            if (_globalCollection == null)
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
                        _globalCollection = JsonConvert.DeserializeObject<GlobalCollection>(json);
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
            FilterResults.AddRange(_globalCollection.Sets[0].Cards.Take(20));
        }
    }
}
