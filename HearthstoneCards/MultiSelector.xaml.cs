using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using WPDevToolkit.Selection;

namespace HearthstoneCards
{
    // TODO make generic, use interfaces

    public sealed partial class MultiSelector : UserControl
    {
        public event SelectionChangedEventHandler SelectionChanged;

        public MultiSelector()
        {
            InitializeComponent();
            XLayoutRoot.DataContext = this;

            SelectionOptions_OnChanged(this);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MultiSelector), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(MultiSelector), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(List<SelectionItem<string>>), typeof(MultiSelector), new PropertyMetadata(new List<SelectionItem<string>>()));

        public static readonly DependencyProperty SelectedOptionsProperty =
            DependencyProperty.Register("SelectedOptions", typeof(List<SelectionItem<string>>), typeof(MultiSelector), new PropertyMetadata(new List<SelectionItem<string>>(), SelectedOptions_PropertyChangedCallback));

        // If list content changes, this needs to be called manually. (DependencyProperty value does not change.)
        private static void SelectedOptions_PropertyChangedCallback(DependencyObject dobj, DependencyPropertyChangedEventArgs eventArgs)
        {
            SelectionOptions_OnChanged(dobj);
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(MultiSelector), new PropertyMetadata(default(DataTemplate)));

        private static void SelectionOptions_OnChanged(DependencyObject dobj)
        {
            
            // 1. sort
            var selectedOptions = ((List<SelectionItem<string>>)dobj.GetValue(SelectedOptionsProperty)).OrderBy(i => i.Key).ToList();

            // 2. change selection status
            var status = string.Empty;
            if (selectedOptions.Count == 0)
            {
                status = "None";
            }
            else if (selectedOptions.Count == ((IList)dobj.GetValue(OptionsProperty)).Count)
            {
                status = "All";
            }
            else
            {
                if (selectedOptions.Count < 4)
                {
                    for (var i = 0; i < selectedOptions.Count - 1; i++)
                    {
                        status += selectedOptions[i].Key + ", ";
                    }
                    status += selectedOptions[selectedOptions.Count - 1].Key;
                }
                else
                {
                    const int threshold = 3;
                    for (var i = 0; i < threshold - 1; i++)
                    {
                        status += selectedOptions[i].Key + ", ";
                    }
                    status += selectedOptions[threshold - 1].Key;
                    status += string.Format(" and {0} more.", selectedOptions.Count - threshold);
                }
            }
            dobj.SetValue(StatusProperty, status);
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            private set { SetValue(StatusProperty, value); }
        }

        public List<SelectionItem<string>> Options
        {
            get { return (List<SelectionItem<string>>)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public List<SelectionItem<string>> SelectedOptions
        {
            get { return (List<SelectionItem<string>>)GetValue(SelectedOptionsProperty); }
            set { SetValue(SelectedOptionsProperty, value);}
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private void XOptionList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }

            foreach (SelectionItem<string> addedItem in e.AddedItems)
            {
                if (!SelectedOptions.Contains(addedItem))
                {
                    SelectedOptions.Add(addedItem);

                }
            }
            foreach (SelectionItem<string> removedItem in e.RemovedItems)
            {
                if (SelectedOptions.Contains(removedItem))
                {
                    SelectedOptions.Remove(removedItem);
                }
            }
            SelectionOptions_OnChanged(this);
        }

        private void Grid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as Grid);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            XSelectionFlyout.Hide();
        }
    }
}
