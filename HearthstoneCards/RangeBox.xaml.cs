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
            DependencyProperty.Register("From", typeof(int), typeof(RangeBox), new PropertyMetadata(0, RangeValue_Changed));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(int), typeof(RangeBox), new PropertyMetadata(0, RangeValue_Changed));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(RangeBox), new PropertyMetadata(int.MinValue, RangeValue_Changed));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(RangeBox), new PropertyMetadata(int.MaxValue, RangeValue_Changed));

        private static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(RangeBox), new PropertyMetadata(false));

        private static void RangeValue_Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            var from = (int) dObj.GetValue(FromProperty);
            var to = (int) dObj.GetValue(ToProperty);
            var min = (int)dObj.GetValue(MinProperty);
            var max = (int)dObj.GetValue(MaxProperty);

            // check range boundaries
            if (from < min) dObj.SetValue(FromProperty, min);
            if (to > max) dObj.SetValue(ToProperty, max);

            // apply validity check
            from = (int)dObj.GetValue(FromProperty);
            to = (int)dObj.GetValue(ToProperty);
            dObj.SetValue(IsValidProperty, @from <= to);
        }

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

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
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
