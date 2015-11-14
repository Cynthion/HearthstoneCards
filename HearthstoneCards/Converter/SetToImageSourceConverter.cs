using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using HearthstoneCards.Model;
using WPDevToolkit;

namespace HearthstoneCards.Converter
{
    public class SetToImageSourceConverter : IValueConverter
    {
        private readonly ImageSource _basicImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/basic-60.png");
        private readonly ImageSource _classicImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/classic-60.png");
        private readonly ImageSource _naxxImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/naxx-60.png");
        private readonly ImageSource _gvgImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/gvg-60.png");
        private readonly ImageSource _brmImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/brm-60.png");
        private readonly ImageSource _tgtImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/tgt-60.png");
        private readonly ImageSource _loeImageSource = ImageLoader.LoadFromAssets("/Assets/Icons/Sets/loe-60.png");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Set)
            {
                switch ((Set)value)
                {
                    case Set.Basic:
                        return _basicImageSource;
                    case Set.Classic:
                        return _classicImageSource;
                    case Set.Naxxramas:
                        return _naxxImageSource;
                    case Set.GoblinVsGnomes:
                        return _gvgImageSource;
                    case Set.BlackrockMountain:
                        return _brmImageSource;
                    case Set.TheGrandTournament:
                        return _tgtImageSource;
                    case Set.LeagueOfExplorers:
                        return _loeImageSource;
                    default:
                        return _basicImageSource;
                }
            }
            return _basicImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
