using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HearthstoneCards
{
    public sealed class CheckedEventArgs
    {
        public bool IsChecked { get; private set; }
        
        public CheckedEventArgs(bool isChecked)
        {
            IsChecked = isChecked;
        }
    }

    public sealed class RangeBoxEventArgs
    {
        public int From { get; private set; }
        public int To { get; private set; }

        public RangeBoxEventArgs(int from, int to)
        {
            From = from;
            To = to;
        }
    }

    public sealed partial class RangeBox : UserControl
    {
        public delegate void IsCheckedChangedEventHandler(object sender, CheckedEventArgs args);
        public event IsCheckedChangedEventHandler IsCheckedChanged;

        public delegate void RangeValueChangedEventHandler(RangeBox sender, RangeBoxEventArgs args);
        public event RangeValueChangedEventHandler RangeValueChanged;

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

        private static readonly DependencyProperty IsRangeBoxEnabledProperty =
            DependencyProperty.Register("IsRangeBoxEnabled", typeof(bool), typeof(RangeBox), new PropertyMetadata(false));

        private static void RangeValue_Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            var from = (int)dObj.GetValue(FromProperty);
            var to = (int)dObj.GetValue(ToProperty);
            var min = (int)dObj.GetValue(MinProperty);
            var max = (int)dObj.GetValue(MaxProperty);

            // check range boundaries
            if (from < min) dObj.SetValue(FromProperty, min);
            if (from > max) dObj.SetValue(FromProperty, max);
            
            if (to < min) dObj.SetValue(ToProperty, min);
            if (to > max) dObj.SetValue(ToProperty, max);

            // apply validity check
            from = (int)dObj.GetValue(FromProperty);
            to = (int)dObj.GetValue(ToProperty);
            dObj.SetValue(IsValidProperty, from <= to);
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

        public bool IsRangeBoxEnabled
        {
            get { return (bool)GetValue(IsRangeBoxEnabledProperty); }
            set { SetValue(IsRangeBoxEnabledProperty, value); }
        }

        private void TextBox_OnLGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.Tag != null)
            {
                var isInvalid = string.IsNullOrEmpty(tb.Text) || string.IsNullOrWhiteSpace(tb.Text);
                var tmp = tb.Text;
                if (tb.Tag.Equals("from"))
                {
                    From = int.MinValue;
                    From = isInvalid ? Min : ExtractNumber(tmp);
                }
                else if (tb.Tag.Equals("to"))
                {
                    To = int.MaxValue;
                    To = isInvalid ? Max : ExtractNumber(tmp);
                }
                if (RangeValueChanged != null)
                {
                    RangeValueChanged(this, new RangeBoxEventArgs(From, To));
                }
            }
        }

        private static int ExtractNumber(string text)
        {
            // TODO fix comma handling
            return (int)Math.Round(double.Parse(text, System.Globalization.CultureInfo.InvariantCulture), 0);
        }

        private void Checkbox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (IsCheckedChanged != null)
            {
                IsCheckedChanged(this, new CheckedEventArgs(true));
            }
        }

        private void Checkbox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (IsCheckedChanged != null)
            {
                IsCheckedChanged(this, new CheckedEventArgs(false));
            }
        }
    }
}
