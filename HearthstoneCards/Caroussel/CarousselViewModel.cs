using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace LightStoneWPApp
{
    public class CarousselViewModel
    {
        public ObservableCollection<Object> Datas
        {
            get; set; }

        public CarousselViewModel()
        {
            var datas = new ObservableCollection<Object>();
            datas.Add(new Data { BitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Test/testCard1.png", UriKind.Absolute)), Title = "01" });
            datas.Add(new Data { BitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Test/testCard2.png", UriKind.Absolute)), Title = "01" });
            datas.Add(new Data { BitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Test/testCard3.png", UriKind.Absolute)), Title = "01" });
            datas.Add(new Data { BitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Test/testCard1.png", UriKind.Absolute)), Title = "01" });
            datas.Add(new Data { BitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Test/testCard2.png", UriKind.Absolute)), Title = "01" });
            datas.Add(new Data { BitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Test/testCard3.png", UriKind.Absolute)), Title = "01" });
            this.Datas = datas;
        }
    }
    public class Data
    {
        public BitmapImage BitmapImage { get; set; }
        public String Title { get; set; }
    }
}
