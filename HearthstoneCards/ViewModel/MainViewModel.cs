using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using WPDevToolkit;
using WPDevToolkit.Interfaces;

namespace HearthstoneCards.ViewModel
{
    public class MainViewModel : AsyncLoader, ILocatable
    {
        protected override async Task<LoadResult> DoLoadAsync()
        {
            var api = SingletonLocator.Get<ApiCaller>();

            //var json = await api.GetAllCardsAsync();
            string fileContent = String.Empty;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/TestDb.txt"));
            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                fileContent = await sRead.ReadToEndAsync();
            }
            var json = await new StringReader(fileContent).ReadToEndAsync();
            if (json != null)
            {
                var globalCollection = JsonConvert.DeserializeObject<GlobalCollection>(json);
            }

            // TODO fix
            return LoadResult.Success;
        }
    }
}
