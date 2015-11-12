using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HearthstoneCards
{
    public sealed partial class RangeBox : UserControl
    {
        public RangeBox()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RangeBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(int), typeof(RangeBox), new PropertyMetadata(0, FromToValue_ChangedCallback));

        private static void FromToValue_ChangedCallback(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            var from = (int) dObj.GetValue(FromProperty);
            var to = (int) dObj.GetValue(ToProperty);
            dObj.SetValue(IsValidProperty, @from <= to);
        }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(int), typeof(RangeBox), new PropertyMetadata(0));

        private static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(RangeBox), new PropertyMetadata(false));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public int From
        {
            get { return (int)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public int To
        {
            get { return (int)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            private set { SetValue(IsValidProperty, value); }
        }

        private void TextBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            // TODO handle format errors
            var tb = sender as TextBox;
            if (tb != null)
            {
                if (tb.Tag.Equals("from"))
                {
                    From = Convert.ToInt32(tb.Text);
                }
                else if (tb.Tag.Equals("to"))
                {
                    To = Convert.ToInt32(tb.Text);
                }
            }
        }
    }
}
