using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HearthstoneCards.ViewModel;
using WPDevToolkit;

namespace HearthstoneCards
{
    public sealed partial class CollectionPage : BasePage
    {
        public CollectionPage()
        {
            this.InitializeComponent();

            // TODO change
            DataContext = SingletonLocator.Get<MainViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int)
            {
                XLightStoneElement.SelectedIndex = (int)e.Parameter;
            }

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.Landscape;
            base.OnNavigatedTo(e);
        }
    }
}
