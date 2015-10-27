using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using WPDevToolkit.Selection;

namespace HearthstoneCards
{
    public class MultiComboBox : ComboBox
    {
        // overwrite event
        public new event SelectionChangedEventHandler SelectionChanged;

        public MultiComboBox()
        {
            BindingOperations.SetBinding(this, PlaceholderTextProperty, new Binding { Source = this, Path = new PropertyPath("Status")});
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            ((Frame)Window.Current.Content).Navigate(typeof(MultiComboBoxPage), this);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(MultiComboBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(IList), typeof(MultiComboBox), new PropertyMetadata(new List<string>(), OnOptionsPropertyChanged));

        public new static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(MultiComboBox), new PropertyMetadata(default(DataTemplate)));

        public new static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(MultiComboBox), new PropertyMetadata(default(Style)));

        private static void OnOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DetermineStatus(d);
        }

        internal void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetermineStatus(this);

            // fire public control event
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        private static void DetermineStatus(DependencyObject d)
        {
            var options = d.GetValue(OptionsProperty) as IList;
            if (options == null)
            {
                return;
            }
            var selectedOptions = options.OfType<ISelectionItem>().Where(i => i.IsSelected).ToList();

            // change status
            var status = string.Empty;
            if (selectedOptions.Count == 0)
            {
                status = "None";
            }
            else if (selectedOptions.Count == options.Count)
            {
                status = "All";
            }
            else
            {
                if (selectedOptions.Count < 4)
                {
                    for (var i = 0; i < selectedOptions.Count - 1; i++)
                    {
                        status += selectedOptions[i] + ", ";
                    }
                    status += selectedOptions[selectedOptions.Count - 1];
                }
                else
                {
                    const int threshold = 3;
                    for (var i = 0; i < threshold - 1; i++)
                    {
                        status += selectedOptions[i] + ", ";
                    }
                    status += selectedOptions[threshold - 1];
                    status += string.Format(" and {0} more.", selectedOptions.Count - threshold);
                }
            }
            d.SetValue(StatusProperty, status);
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            private set { SetValue(StatusProperty, value); }
        }

        public IList Options
        {
            get { return (IList)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public new DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public new Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }
    }
}
