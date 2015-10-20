using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

namespace HearthstoneCards
{
    public sealed partial class MultiSelector : UserControl
    {
        public MultiSelector()
        {
            InitializeComponent();
            XLayoutRoot.DataContext = this;
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MultiSelector), new PropertyMetadata(string.Empty));

        // TODO string -> generic T
        public static readonly DependencyProperty OptionsProperty = 
            DependencyProperty.Register("Options", typeof(IList), typeof(MultiSelector), new PropertyMetadata(null));

        // TODO string -> generic T
        public static readonly DependencyProperty SelectedOptionsProperty =
            DependencyProperty.Register("SelectedOptions", typeof(ObservableCollection<string>), typeof(MultiSelector), new PropertyMetadata(new ObservableCollection<string>()));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public IList Options
        {
            get { return (IList)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public ObservableCollection<string> SelectedOptions
        {
            get { return (ObservableCollection<string>)GetValue(SelectedOptionsProperty); }
            set { SetValue(SelectedOptionsProperty, value);}
        }

        private void XOptionList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (string addedItem in e.AddedItems)
            {
                if (!SelectedOptions.Contains(addedItem))
                {
                    SelectedOptions.Add(addedItem);
                }
            }
            foreach (string removedItem in e.RemovedItems)
            {
                if (SelectedOptions.Contains(removedItem))
                {
                    SelectedOptions.Remove(removedItem);
                }
            }
            // TODO sort remaining entries
        }
    }
}
