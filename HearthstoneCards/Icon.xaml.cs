using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HearthstoneCards
{
    public sealed partial class Icon : UserControl
    {
        public Icon()
        {
            this.InitializeComponent();
        }

        public static DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(ControlTemplate), typeof(Icon), new PropertyMetadata(null));

        public static DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(Icon), new PropertyMetadata(null));

        public ControlTemplate IconTemplate
        {
            get { return (ControlTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}
