using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace HearthstoneCards.Caroussel
{
    public class LightStone : Canvas
    {
        // Temp 
        private Double initialOffset;

        // Mirror
        private Rectangle rectangle = new Rectangle();

        // internalList keep reference on all UIElement
        private List<UIElement> internalList = new List<UIElement>();

        // ItemTemplate
        private DataTemplate itemTemplate;

        // prevent ArrangeOverride on items adding
        private bool isUpdatingPosition;

        // Storyboard on gesture
        private Storyboard storyboard = new Storyboard();

        // temp size
        private double desiredWidth;
        private double desiredHeight;

        // Gesture movement
        private bool isIncrementing;


        /// <summary>
        /// Duration of the easing function animation (ms)
        /// </summary>
        public int TransitionDuration
        {
            get { return (int)GetValue(TransitionDurationProperty); }
            set { SetValue(TransitionDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TransitionDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register("TransitionDuration", typeof(int), typeof(LightStone),
            new PropertyMetadata(500));


        /// <summary>
        /// Selected Index
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(LightStone), new PropertyMetadata(0, OnLightStonePropertyChanged));

        private static void OnLightStonePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            LightStone lightStone = (LightStone)d;
            lightStone.UpdatePosition();
        }

        /// <summary>
        /// Depth of non Selected Index Items
        /// </summary>
        public int Depth
        {
            get { return (int)GetValue(DepthProperty); }
            set { SetValue(DepthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(int), typeof(LightStone), new PropertyMetadata(0, OnLightStonePropertyChanged));


        /// <summary>
        /// Easing function to apply for each Transition
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(LightStone),
            new PropertyMetadata(new ExponentialEase { EasingMode = EasingMode.EaseOut }));



        public int TranslateX
        {
            get { return (int)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TranslateX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register("TranslateX", typeof(int), typeof(LightStone), new PropertyMetadata(0, OnLightStonePropertyChanged));



        /// <summary>
        /// Translation
        /// </summary>
        public int TranslateY
        {
            get { return (int)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Translation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register("TranslateY", typeof(int), typeof(LightStone), new PropertyMetadata(0, OnLightStonePropertyChanged));


        /// <summary>
        /// Rotation angle
        /// </summary>
        public Double Rotation
        {
            get { return (Double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(Double), typeof(LightStone), new PropertyMetadata(0, OnLightStonePropertyChanged));


        public int MaxVisibleItems
        {
            get { return (int)GetValue(MaxVisibleItemsProperty); }
            set { SetValue(MaxVisibleItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxViewableItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxVisibleItemsProperty =
            DependencyProperty.Register("MaxVisibleItems", typeof(int), typeof(LightStone), new PropertyMetadata(15, OnLightStonePropertyChanged));

        /// <summary>
        /// Items source : Better if ObservableCollection :)
        /// </summary>
        public IEnumerable<Object> ItemsSource
        {
            get { return (IEnumerable<Object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  
        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                        typeof(IEnumerable<Object>),
                        typeof(LightStone),
                        new PropertyMetadata(0, ItemsSourceChangedCallback));

        private static void ItemsSourceChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
                return;

            if (args.NewValue == args.OldValue)
                return;

            LightStone lightStone = dependencyObject as LightStone;

            if (lightStone == null)
                return;

            var obsList = args.NewValue as INotifyCollectionChanged;

            if (obsList != null)
            {
                obsList.CollectionChanged += (sender, eventArgs) =>
                {
                    switch (eventArgs.Action)
                    {
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var oldItem in eventArgs.OldItems)
                            {
                                for (int i = 0; i < lightStone.internalList.Count; i++)
                                {
                                    var fxElement = lightStone.internalList[i] as FrameworkElement;
                                    if (fxElement == null || fxElement.DataContext != oldItem) continue;
                                    lightStone.RemoveAt(i);
                                }
                            }

                            break;
                        case NotifyCollectionChangedAction.Add:
                            foreach (var newItem in eventArgs.NewItems)
                                lightStone.CreateItem(newItem, 0);
                            break;
                    }
                };
            }

            lightStone.Bind();
        }

        /// <summary>
        /// Item Template 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return itemTemplate;
            }
            set
            {
                itemTemplate = value;

            }
        }


        /// <summary>
        /// Bind all Items
        /// </summary>
        private void Bind()
        {
            if (ItemsSource == null)
                return;

            this.Children.Clear();
            this.internalList.Clear();

            foreach (object item in ItemsSource)
                this.CreateItem(item);

            this.Children.Add(rectangle);
        }

        /// <summary>
        /// Remove an item at position. Reaffect SelectedIndex
        /// </summary>
        public void RemoveAt(Int32 index)
        {
            var uiElement = this.internalList[index];

            //  this.isUpdatingPosition = true;
            // Remove from internal list
            this.internalList.RemoveAt(index);

            // Remove from visual tree
            this.Children.Remove(uiElement);

            if (this.SelectedIndex == index)
                this.SelectedIndex = this.SelectedIndex == 0 ? 0 : this.SelectedIndex - 1;
            else if (this.SelectedIndex > index)
                this.SelectedIndex--;

            this.UpdatePosition();
        }

        /// <summary>
        /// Create an item (Load data template and bind)
        /// </summary>
        private FrameworkElement CreateItem(object item, Double opacity = 1)
        {
            FrameworkElement element = ItemTemplate.LoadContent() as FrameworkElement;
            if (element == null)
                return null;

            element.DataContext = item;
            element.Opacity = opacity;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            PlaneProjection planeProjection = new PlaneProjection();
            planeProjection.CenterOfRotationX = 0.5;
            planeProjection.CenterOfRotationY = 0.5;
            element.Projection = planeProjection;

            this.internalList.Add(element);
            this.Children.Add(element);

            return element;
        }


        /// <summary>
        /// Arrange all items
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (isUpdatingPosition)
                return finalSize;

            Double centerLeft = 0;
            Double centerTop = 0;

            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

            // Set 
            Canvas.SetZIndex(rectangle, 1);

            // Storyboard for all Items to appear
            var localStoryboard = new Storyboard();

            for (int i = 0; i < this.internalList.Count; i++)
            {
                UIElement container = internalList[i];

                Size desiredSize = container.DesiredSize;
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height)) continue;

                // get the good center and top position
                if (centerLeft == 0 && centerTop == 0 && desiredSize.Width > 0 && desiredSize.Height > 0)
                {
                    desiredWidth = desiredSize.Width;
                    desiredHeight = desiredSize.Height;

                    centerLeft = (finalSize.Width / 2) - (desiredWidth / 2);
                    centerTop = (finalSize.Height - desiredHeight) / 2;
                }

                // Get position from SelectedIndex
                var deltaFromSelectedIndex = Math.Abs(this.SelectedIndex - i);

                // Get rect position
                var rect = new Rect(centerLeft, centerTop, desiredWidth, desiredHeight);

                container.Arrange(rect);
                Canvas.SetLeft(container, centerLeft);
                Canvas.SetTop(container, centerTop);

                // Apply Transform
                PlaneProjection planeProjection = container.Projection as PlaneProjection;

                if (planeProjection == null)
                    continue;

                // Get properies 
                var depth = (i == this.SelectedIndex) ? 0 : -(this.Depth);
                var rotation = (i == this.SelectedIndex) ? 0 : ((i < this.SelectedIndex) ? Rotation : -(Rotation));
                var translateX = (i == this.SelectedIndex) ? 0 : ((i < this.SelectedIndex) ? -TranslateX : TranslateX);
                var offsetX = (i == this.SelectedIndex) ? 0 : (i - this.SelectedIndex) * desiredWidth;
                var translateY = TranslateY;

                // CenterOfRotationX
                // to Get good center of rotation for SelectedIndex, must know the animation behavior
                int centerOfRotationSelectedIndex = isIncrementing ? 1 : 0;
                var centerOfRotationX = (i == this.SelectedIndex) ? centerOfRotationSelectedIndex : ((i > this.SelectedIndex) ? 1 : 0);

                // Apply on current item
                planeProjection.CenterOfRotationX = centerOfRotationX;
                planeProjection.GlobalOffsetY = translateY;
                planeProjection.GlobalOffsetZ = depth;
                planeProjection.GlobalOffsetX = translateX;
                planeProjection.RotationY = rotation;
                planeProjection.LocalOffsetX = offsetX;

                // calculate zindex and opacity
                int zindex = (this.internalList.Count * 100) - deltaFromSelectedIndex;
                double opacity = 1d - (Math.Abs((Double)(i - this.SelectedIndex) / (MaxVisibleItems + 1)));

                // Items appears
                if (container.Visibility == Visibility.Visible && container.Opacity == 0d)
                {
                    localStoryboard.AddAnimation(container, TransitionDuration, rotation, "(UIElement.Projection).(PlaneProjection.RotationY)", this.EasingFunction);
                    localStoryboard.AddAnimation(container, TransitionDuration, depth, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", this.EasingFunction);
                    localStoryboard.AddAnimation(container, TransitionDuration, translateX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)", this.EasingFunction);
                    localStoryboard.AddAnimation(container, TransitionDuration, offsetX, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", this.EasingFunction);
                    localStoryboard.AddAnimation(container, TransitionDuration, translateY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", this.EasingFunction);
                    localStoryboard.AddAnimation(container, TransitionDuration, 0, opacity, "Opacity", this.EasingFunction);
                }
                else
                {

                    container.Opacity = opacity;
                }

                Canvas.SetZIndex(container, zindex);

            }

            rectangle.Fill = new SolidColorBrush(Colors.Black);
            rectangle.Opacity = 0.9;
            Canvas.SetLeft(rectangle, 0);
            Canvas.SetTop(rectangle, (this.ActualHeight / 2));
            Canvas.SetZIndex(rectangle, 1);
            rectangle.Width = this.ActualWidth;
            rectangle.Height = this.ActualHeight;

            if (localStoryboard.Children.Count > 0)
                localStoryboard.Begin();

            return finalSize;
        }



        /// <summary>
        /// Update all positions. Launch every animations on all items with a unique StoryBoard
        /// </summary>
        private void UpdatePosition()
        {
            storyboard = new Storyboard();

            isUpdatingPosition = true;

            for (int i = 0; i < this.internalList.Count; i++)
            {
                var item = internalList[i];

                PlaneProjection planeProjection = item.Projection as PlaneProjection;

                if (planeProjection == null)
                    continue;

                // Get properties
                var depth = (i == this.SelectedIndex) ? 0 : -(this.Depth);
                var rotation = (i == this.SelectedIndex) ? 0 : ((i < this.SelectedIndex) ? Rotation : -Rotation);
                var offsetX = (i == this.SelectedIndex) ? 0 : (i - this.SelectedIndex) * desiredWidth;
                var translateY = TranslateY;
                var translateX = (i == this.SelectedIndex) ? 0 : ((i < this.SelectedIndex) ? -TranslateX : TranslateX);

                // CenterOfRotationX
                // to Get good center of rotation for SelectedIndex, must know the animation behavior
                int centerOfRotationSelectedIndex = isIncrementing ? 1 : 0;
                var centerOfRotationX = (i == this.SelectedIndex) ? centerOfRotationSelectedIndex : ((i > this.SelectedIndex) ? 1 : 0);
                planeProjection.CenterOfRotationX = centerOfRotationX;

                // Dont animate all items
                var inf = this.SelectedIndex - (MaxVisibleItems * 2);
                var sup = this.SelectedIndex + (MaxVisibleItems * 2);

                if (i < inf || i > sup)
                    continue;

                // Zindex and Opacity
                var deltaFromSelectedIndex = Math.Abs(this.SelectedIndex - i);
                int zindex = (this.internalList.Count * 100) - deltaFromSelectedIndex;
                Canvas.SetZIndex(item, zindex);
                Double opacity = 1d - (Math.Abs((Double)(i - this.SelectedIndex) / (MaxVisibleItems + 1)));

                var newVisibility = deltaFromSelectedIndex > MaxVisibleItems
                               ? Visibility.Collapsed
                               : Visibility.Visible;

                // Item already present
                if (item.Visibility == newVisibility)
                {
                    storyboard.AddAnimation(item, TransitionDuration, rotation, "(UIElement.Projection).(PlaneProjection.RotationY)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, depth, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, offsetX, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, opacity, "Opacity", this.EasingFunction);
                }
                else if (newVisibility == Visibility.Visible)
                {
                    // This animation will occur in the ArrangeOverride() method
                    item.Visibility = newVisibility;
                    item.Opacity = 0d;
                }
                else if (newVisibility == Visibility.Collapsed)
                {
                    storyboard.AddAnimation(item, TransitionDuration, rotation, "(UIElement.Projection).(PlaneProjection.RotationY)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, depth, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, offsetX, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", this.EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, 0d, "Opacity", this.EasingFunction);
                    storyboard.Completed += (sender, o) =>
                        item.Visibility = Visibility.Collapsed;
                }
            }

            // When storyboard completed, Invalidate
            storyboard.Completed += (sender, o) =>
            {
                this.isUpdatingPosition = false;
                this.InvalidateArrange();
            };

            storyboard.Begin();
        }



        /// <summary>
        /// Measure items 
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, availableSize.Width, availableSize.Height) };

            foreach (FrameworkElement container in this.Children)
            {
                container.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            }
            return (availableSize);
        }

        /// <summary>
        /// Need Events
        /// </summary>
        public LightStone()
        {
            this.PointerPressed += OnPointerPressed;
            this.PointerReleased += OnPointerReleased;
            this.Tapped += OnTapped;
        }

        /// <summary>
        /// Tap an element
        /// </summary>
        private void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            var positionX = args.GetPosition(this).X;
            for (int i = 0; i < this.internalList.Count; i++)
            {
                var child = internalList[i];
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                if (!(positionX >= rect.Left) || !(positionX <= (rect.Left + rect.Width))) continue;

                isIncrementing = (i > this.SelectedIndex);

                this.SelectedIndex = i;
                return;
            }
        }

        /// <summary>
        /// Initial pressed position
        /// </summary>
        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            initialOffset = args.GetCurrentPoint(this).Position.X;
        }

        /// <summary>
        /// Calculate Behavior
        /// </summary>
        private void OnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            // Minimum amount to declare as a manipulation
            const int moveThreshold = 40;

            // last position
            var clientX = pointerRoutedEventArgs.GetCurrentPoint(this).Position.X;

            // Here is a "Tap on Item"
            if (!(Math.Abs(clientX - initialOffset) > moveThreshold))
                return;

            isIncrementing = (clientX < initialOffset);

            // Here is a manipulation
            if (clientX < initialOffset)
            {
                this.SelectedIndex = (this.SelectedIndex < (this.internalList.Count - 1))
                                         ? this.SelectedIndex + 1
                                         : this.SelectedIndex;

            }
            else if (this.SelectedIndex > 0)
            {
                this.SelectedIndex--;
            }

            initialOffset = clientX;
        }





    }
}
