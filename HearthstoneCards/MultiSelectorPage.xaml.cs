using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace HearthstoneCards
{
    public sealed partial class MultiSelectorPage : BasePage
    {
        private MultiSelector _msControl;

        public MultiSelectorPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // provide the control itself as datacontext
            _msControl = e.Parameter as MultiSelector;
            if (_msControl != null)
            {
                this.DataContext = _msControl;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // forward to control
            _msControl.OnSelectionChanged(sender, e);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null);
            Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
        }
    }
}
