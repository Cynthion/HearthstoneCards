using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public IList<SelectionItem<string>> Classes { get; private set; }

        public MainViewModel()
        {
            //Classes = new ObservableCollection<string>(new List<string>{"Shaman", "Priest", "Druid", "Paladin", "Rogue", "Hunter", "Warrior", "Warlock", "Mage"});
            Classes = new List<SelectionItem<string>>(new List<SelectionItem<string>>
            {
                new SelectionItem<string>("Priest", "../Assets/Icons/Classes/priest.png")
            });
        }

        protected override async Task<LoadResult> DoLoadAsync()
        {
            var api = SingletonLocator.Get<ApiCaller>();

            try
            {
                //var json = await api.GetAllCardsAsync();
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
                }
                return LoadResult.Success;
            }
            catch (Exception e)
            {
                return new LoadResult(e.Message);
            }
        }
    }
}
