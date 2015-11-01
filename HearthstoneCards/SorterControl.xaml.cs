using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HearthstoneCards
{
    public sealed partial class SorterControl : UserControl
    {
        public SorterControl()
        {
            this.InitializeComponent();
        }

        public static DependencyProperty IsSortingControlVisibleProperty =
            DependencyProperty.Register("IsSortingControlVisible", typeof(bool), typeof(SorterControl), new PropertyMetadata(false));

        public static DependencyProperty ListProperty = 
            DependencyProperty.Register("List", typeof(IList), typeof(SorterControl), new PropertyMetadata(new List<string>(), OnListPropertyChanged));

        public static DependencyProperty OptionsProperty = 
            DependencyProperty.Register("Options", typeof(IList), typeof(SorterControl), new PropertyMetadata(new List<string>(), OnOptionsPropertyChanged));

        private static void OnOptionsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // register selection-changed event, which then triggers the new option to be applied
            throw new NotImplementedException();
        }

        private static void OnListPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // re-apply sort
            throw new NotImplementedException();
        }
    }
}
