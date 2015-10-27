using System;
using System.Collections;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HearthstoneCards
{
    public sealed partial class SorterControl : UserControl
    {
        public SorterControl()
        {
            this.InitializeComponent();
        }

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
