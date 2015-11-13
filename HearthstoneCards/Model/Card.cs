using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using HearthstoneCards.Helper;
using Newtonsoft.Json;
using WPDevToolkit;

namespace HearthstoneCards.Model
{
    public enum Rarity
    {
        Free = 0,
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    [JsonConverter(typeof(CardConverter))]
    public class Card : BaseNotifyPropertyChanged, IEquatable<Card>
    {
        // from JSON (http://hearthstonejson.com/)
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Type { get; set; }
        public Rarity Rarity { get; set; }
        public string Faction { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string Text { get; set; }
        public string InPlayText { get; set; }
        public List<string> Mechanics { get; set; }
        public string Flavor { get; set; }
        public string Artist { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public int Durability { get; set; }
        public string Id { get; set; }
        public bool IsCollectible { get; set; }
        public bool IsElite { get; set; }
        public string HowToGet { get; set; }
        public string HowToGetGold { get; set; }
        public string Set { get; set; }
        
        private ImageSource _image;
        //private ImageSource _imageGold;
        
        private bool _isImageLoading;

        public async Task LoadImageAsync()
        {
            if (Image == null)
            {
                try
                {
                    IsImageLoading = true;
                    var imgUrl = string.Format("http://wow.zamimg.com/images/hearthstone/cards/enus/original/{0}.png", Id);
                    Image = await ImageLoader.LoadImageAsync(imgUrl);
                }
                finally
                {
                    IsImageLoading = false;
                }
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

        //public ImageSource ImageGold
        //{
        //    get { return _imageGold; }
        //    private set
        //    {
        //        if (_imageGold != value)
        //        {
        //            _imageGold = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        public bool IsImageLoading
        {
            get { return _isImageLoading; }
            set
            {
                if (_isImageLoading != value)
                {
                    _isImageLoading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Equals(Card other)
        {
            return Id.Equals(other.Id);
        }
    }
}
