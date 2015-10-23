using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using HearthstoneCards.Helper;
using Newtonsoft.Json;
using WPDevToolkit;

namespace HearthstoneCards.Model
{
    [JsonConverter(typeof(CardConverter))]
    public class Card : BaseNotifyPropertyChanged
    {
        // from JSON
        public string CardId { get; set; }
        public string Name { get; set; }
        public string CardSet { get; set; }
        public string Type { get; set; }
        public string Faction { get; set; }
        public string Rarity { get; set; }
        public int Cost { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public string Text { get; set; }
        public string Flavor { get; set; }
        public string Artist { get; set; }
        public bool IsCollectible { get; set; }
        public bool IsElite { get; set; }
        public string Race { get; set; }
        public string ImgUrl { get; set; }
        public string ImgGoldUrl { get; set; }
        public string Locale { get; set; }
        public string Class { get; set; }
        public string HowToGet { get; set; }
        public string HowToGetGold { get; set; }
        // TODO add mechanics

        private ImageSource _image;
        private ImageSource _imageGold;

        public async Task LoadImageAsync()
        {
            if (ImgUrl != null && Image == null)
            {
                Image = await ImageLoader.LoadImageAsync(ImgUrl);
            }
        }

        public ImageSource Image
        {
            get { return _image; }
            private set
            {
                if (_image != value)
                {
                    _image = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ImageSource ImageGold
        {
            get { return _imageGold; }
            private set
            {
                if (_imageGold != value)
                {
                    _imageGold = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
