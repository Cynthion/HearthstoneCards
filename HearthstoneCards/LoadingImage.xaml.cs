using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace HearthstoneCards
{
    public sealed partial class LoadingImage : UserControl
    {
        public LoadingImage()
        {
            this.InitializeComponent();

            // set part of this object as data context for the wrapped image
            BindingOperations.SetBinding(XImage, Image.SourceProperty, new Binding { Source = this, Path = new PropertyPath("Source2") });
            BindingOperations.SetBinding(XImage, Image.StretchProperty, new Binding { Source = this, Path = new PropertyPath("Stretch2") });
            BindingOperations.SetBinding(XImage, Image.StyleProperty, new Binding { Source = this, Path = new PropertyPath("Style2") });
            
            SetValue(IsLoadingProperty, true);
        }

        public static DependencyProperty Source2Property =
            DependencyProperty.Register("Source2", typeof(ImageSource), typeof(LoadingImage), new PropertyMetadata(Image.SourceProperty.GetMetadata(typeof(Image)).DefaultValue));

        public static DependencyProperty Stretch2Property =
            DependencyProperty.Register("Stretch2", typeof(Stretch), typeof(LoadingImage), new PropertyMetadata(Image.StretchProperty.GetMetadata(typeof(Image)).DefaultValue));

        public static DependencyProperty Style2Property =
            DependencyProperty.Register("Style2", typeof(Style), typeof(LoadingImage), new PropertyMetadata(Image.StyleProperty.GetMetadata(typeof(Image)).DefaultValue));

        public static DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingImage), new PropertyMetadata(false));

        private void Image_OnRendered(object sender, RoutedEventArgs e)
        {
            SetValue(IsLoadingProperty, false);
        }

        public ImageSource Source2
        {
            get { return (ImageSource)GetValue(Source2Property); }
            set { SetValue(Source2Property, value); }
        }

        public Stretch Stretch2
        {
            get { return (Stretch)GetValue(Stretch2Property); }
            set { SetValue(Stretch2Property, value); }
        }

        public Style Style2
        {
            get { return (Style)GetValue(Style2Property); }
            set { SetValue(Style2Property, value); }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
        }
    }
}
