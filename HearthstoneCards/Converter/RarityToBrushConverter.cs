using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using HearthstoneCards.Model;

namespace HearthstoneCards.Converter
{
    public class RarityToBrushConverter : IValueConverter
    {
        private readonly Brush _freeBrush = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
        private readonly Brush _commonBrush = new SolidColorBrush(Color.FromArgb(255, 15, 175, 3));
        private readonly Brush _rareBrush = new SolidColorBrush(Color.FromArgb(255, 25, 142, 255));
        private readonly Brush _epicBrush = new SolidColorBrush(Color.FromArgb(255, 171, 72, 238));
        private readonly Brush _legendaryBrush = new SolidColorBrush(Color.FromArgb(255, 240, 112, 0));
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Rarity)
            {
                switch ((Rarity) value)
                {
                    case Rarity.Common:
                        return _commonBrush;
                    case Rarity.Rare:
                        return _rareBrush;
                    case Rarity.Epic:
                        return _epicBrush;
                    case Rarity.Legendary:
                        return _legendaryBrush;
                    case Rarity.Free:
                        return _freeBrush;
                    default:
                        return _freeBrush;
                }
            }
            return _freeBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
