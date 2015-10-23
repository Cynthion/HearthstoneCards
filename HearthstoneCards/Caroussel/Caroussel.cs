using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace HearthstoneCards.Caroussel
{
    public class Caroussel : Canvas
    {
        // gestures
        private double _initialX;
        private bool _isGesture;

        // mirror
        // private readonly Rectangle _rectangle = new Rectangle();

        // internalList keep reference on all UIElement
        private readonly List<UIElement> _internalList = new List<UIElement>();

        public DataTemplate ItemTemplate { get; set; }

        // prevent ArrangeOverride on items adding
        private bool _isUpdatingPosition;

        // storyboard on gesture
        // private Storyboard _storyboard = new Storyboard();

        // temp size
        private double _desiredWidth;
        private double _desiredHeight;

        public Caroussel()
        {
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            Tapped += OnTapped;
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(Caroussel), new PropertyMetadata(new List<string>(), ItemsSourceChangedCallback));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(Caroussel), new PropertyMetadata(new ExponentialEase { EasingMode = EasingMode.EaseOut }));

        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register("TransitionDuration", typeof(int), typeof(Caroussel), new PropertyMetadata(300));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnSelectedIndexChanged));

        public static readonly DependencyProperty SelectedItemProperty = // read-only
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Caroussel), new PropertyMetadata(null)); // TODO correct default value

        public static readonly DependencyProperty MaxVisibleItemsProperty =
            DependencyProperty.Register("MaxVisibleItems", typeof(int), typeof(Caroussel), new PropertyMetadata(10, OnLightStonePropertyChanged));

        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnLightStonePropertyChanged));

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register("TranslateX", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnLightStonePropertyChanged));

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register("TranslateY", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnLightStonePropertyChanged));

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(double), typeof(Caroussel), new PropertyMetadata(0.0, OnLightStonePropertyChanged));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var newIndex = (int) d.GetValue(SelectedIndexProperty);
            
            //// set SelectedItem
            //d.SetValue(SelectedItemProperty, ((IList)d.GetValue(ItemsSourceProperty))[newIndex]);

            //// load more items, if it's done incrementally
            //if (d.GetValue(ItemsSourceProperty) is ISupportIncrementalLoading)
            //{
            //    var items = (IList) d.GetValue(ItemsSourceProperty);
            //    var range = (int) d.GetValue(MaxVisibleItemsProperty);
            //    var loadMore = newIndex + (range/2.0) > items.Count || newIndex - (range/2.0) < 0;

            //    if (loadMore)
            //    {
            //        var list = (ISupportIncrementalLoading) d.GetValue(ItemsSourceProperty);
            //        list.LoadMoreItemsAsync(0);
            //    }
            //}

            OnLightStonePropertyChanged(d, e);
        }

        private static void OnLightStonePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var lightStone = (Caroussel)d;
            lightStone.UpdatePosition();
        }

        private static void ItemsSourceChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null || args.NewValue == args.OldValue)
                return;

            var caroussel = dependencyObject as Caroussel;
            if (caroussel == null)
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
                                for (var i = 0; i < caroussel._internalList.Count; i++)
                                {
                                    var fxElement = caroussel._internalList[i] as FrameworkElement;
                                    if (fxElement == null || fxElement.DataContext != oldItem) continue;
                                    caroussel.RemoveAt(i);
                                }
                            }
                            break;
                        case NotifyCollectionChangedAction.Add:
                            foreach (var newItem in eventArgs.NewItems)
                            {
                                caroussel.CreateItem(newItem, 0);
                            }
                            break;
                    }
                };
            }
            caroussel.Bind();
        }

        private void Bind()
        {
            if (ItemsSource != null)
            {
                Children.Clear();
                _internalList.Clear();

                foreach (var item in ItemsSource)
                {
                    CreateItem(item);
                }
                //Children.Add(_rectangle);
            }
        }

        /// <summary>
        /// Create an item (load data template and bind).
        /// </summary>
        private void CreateItem(object item, double opacity = 1)
        {
            // distinguish between object being a single item or a list of items
            var itemList = item as IEnumerable;
            if (itemList != null)
            {
                foreach (var item2 in itemList)
                {
                    CreateItem2(item2, opacity);
                }
            }
            else
            {
                CreateItem2(item, opacity);
            }
        }

        private void CreateItem2(object item, double opacity)
        {
            var element = ItemTemplate.LoadContent() as FrameworkElement;
            if (element != null)
            {
                element.DataContext = item;
                element.Opacity = opacity;
                element.RenderTransformOrigin = new Point(0.5, 0.5);

                var planeProjection = new PlaneProjection
                {
                    CenterOfRotationX = 0.5,
                    CenterOfRotationY = 0.5
                };
                element.Projection = planeProjection;

                _internalList.Add(element);
                Children.Add(element);
            }
        }

        /// <summary>
        /// Remove an item at position. Reaffect SelectedIndex.
        /// </summary>
        public void RemoveAt(int index)
        {
            var uiElement = _internalList[index];

            // remove from internal list
            _internalList.RemoveAt(index);

            // remove from visual tree
            Children.Remove(uiElement);

            if (SelectedIndex == index)
                SelectedIndex = SelectedIndex == 0 ? 0 : SelectedIndex - 1;
            else if (SelectedIndex > index)
                SelectedIndex--;

            UpdatePosition();
        }

        /// <summary>
        /// Update all positions. Launch every animation on all items with a unique StoryBoard.
        /// </summary>
        private void UpdatePosition()
        {
            var storyboard = new Storyboard();

            _isUpdatingPosition = true;

            for (var i = 0; i < _internalList.Count; i++)
            {
                var item = _internalList[i];

                var planeProjection = item.Projection as PlaneProjection;
                if (planeProjection == null)
                    continue;

                // get properties
                var depth = (i == SelectedIndex) ? 0 : -(Depth);
                var rotation = (i == SelectedIndex) ? 0 : ((i < SelectedIndex) ? Rotation : -Rotation);
                var offsetX = (i == SelectedIndex) ? 0 : (i - SelectedIndex) * _desiredWidth;
                var translateY = TranslateY;
                var translateX = (i == SelectedIndex) ? 0 : ((i < SelectedIndex) ? -TranslateX : TranslateX);

                // CenterOfRotationX
                // to get good center of rotation for SelectedIndex, must know the animation behavior
                var centerOfRotationSelectedIndex = _isGesture ? 1 : 0;
                var centerOfRotationX = (i == SelectedIndex) ? centerOfRotationSelectedIndex : ((i > SelectedIndex) ? 1 : 0);
                planeProjection.CenterOfRotationX = centerOfRotationX;

                // don't animate all items
                var inf = SelectedIndex - (MaxVisibleItems * 2);
                var sup = SelectedIndex + (MaxVisibleItems * 2);

                if (i < inf || i > sup)
                    continue;

                // Z-Index and Opacity
                var deltaFromSelectedIndex = Math.Abs(SelectedIndex - i);
                var zindex = (_internalList.Count * 100) - deltaFromSelectedIndex;
                SetZIndex(item, zindex);
                var opacity = 1d - (Math.Abs((double)(i - SelectedIndex) / (MaxVisibleItems + 1)));

                var newVisibility = deltaFromSelectedIndex > MaxVisibleItems ? Visibility.Collapsed : Visibility.Visible;

                // item already present
                if (item.Visibility == newVisibility)
                {
                    storyboard.AddAnimation(item, TransitionDuration, rotation, "(UIElement.Projection).(PlaneProjection.RotationY)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, depth, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, offsetX, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, opacity, "Opacity", EasingFunction);
                }
                else if (newVisibility == Visibility.Visible)
                {
                    // this animation will occur in the ArrangeOverride() method
                    item.Visibility = newVisibility;
                    item.Opacity = 0d;
                }
                else if (newVisibility == Visibility.Collapsed)
                {
                    storyboard.AddAnimation(item, TransitionDuration, rotation, "(UIElement.Projection).(PlaneProjection.RotationY)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, depth, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, offsetX, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, translateY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, 0d, "Opacity", EasingFunction);
                    storyboard.Completed += (sender, o) =>
                    {
                        item.Visibility = Visibility.Collapsed;
                    };
                }
            }

            // start storyboard
            // when storyboard completed, InvalidateArrange()
            storyboard.Completed += (sender, o) =>
            {
                _isUpdatingPosition = false;
                InvalidateArrange();
            };
            storyboard.Begin();
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public int TransitionDuration
        {
            get { return (int)GetValue(TransitionDurationProperty); }
            set { SetValue(TransitionDurationProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public int SelectedItem
        {
            get { return (int)GetValue(SelectedItemProperty); }
        }

        public int MaxVisibleItems
        {
            get { return (int)GetValue(MaxVisibleItemsProperty); }
            set { SetValue(MaxVisibleItemsProperty, value); }
        }

        public int Depth
        {
            get { return (int)GetValue(DepthProperty); }
            set { SetValue(DepthProperty, value); }
        }

        public int TranslateX
        {
            get { return (int)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }

        public int TranslateY
        {
            get { return (int)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }

        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isUpdatingPosition)
                return finalSize;

            double centerLeft = 0;
            double centerTop = 0;

            Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

            // storyboard for all items to appear
            var localStoryboard = new Storyboard();

            for (var i = 0; i < _internalList.Count; i++)
            {
                var container = _internalList[i];

                var desiredSize = container.DesiredSize;
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height))
                {
                    continue;
                }

                // get the good center and top position
                if (centerLeft == 0 && centerTop == 0 && desiredSize.Width > 0 && desiredSize.Height > 0)
                {
                    _desiredWidth = desiredSize.Width;
                    _desiredHeight = desiredSize.Height;

                    centerLeft = (finalSize.Width / 2) - (_desiredWidth / 2);
                    centerTop = (finalSize.Height - _desiredHeight) / 2;
                }

                // get position from SelectedIndex
                var deltaFromSelectedIndex = Math.Abs(SelectedIndex - i);

                // get rect position
                var rect = new Rect(centerLeft, centerTop, _desiredWidth, _desiredHeight);

                container.Arrange(rect);
                SetLeft(container, centerLeft);
                SetTop(container, centerTop);

                // apply transform
                var planeProjection = container.Projection as PlaneProjection;
                if (planeProjection == null)
                {
                    continue;
                }

                // get properies 
                var depth = (i == SelectedIndex) ? 0 : -(Depth);
                var rotation = (i == SelectedIndex) ? 0 : ((i < SelectedIndex) ? Rotation : -(Rotation));
                var translateX = (i == SelectedIndex) ? 0 : ((i < SelectedIndex) ? -TranslateX : TranslateX);
                var offsetX = (i == SelectedIndex) ? 0 : (i - SelectedIndex) * _desiredWidth;
                var translateY = TranslateY;

                // CenterOfRotationX
                // to get good center of rotation for SelectedIndex, must know the animation behavior
                var centerOfRotationSelectedIndex = _isGesture ? 1 : 0;
                var centerOfRotationX = (i == SelectedIndex) ? centerOfRotationSelectedIndex : ((i > SelectedIndex) ? 1 : 0);

                // apply on current item
                planeProjection.CenterOfRotationX = centerOfRotationX;
                planeProjection.GlobalOffsetY = translateY;
                planeProjection.GlobalOffsetZ = depth;
                planeProjection.GlobalOffsetX = translateX;
                planeProjection.RotationY = rotation;
                planeProjection.LocalOffsetX = offsetX;

                // calculate zindex and opacity
                var zindex = (_internalList.Count * 100) - deltaFromSelectedIndex;
                SetZIndex(container, zindex);
                // var opacity = 1d - (Math.Abs((double)(i - SelectedIndex) / (MaxVisibleItems + 1)));

                // items appears
                //if (container.Visibility == Visibility.Visible && container.Opacity == 0d)
                //{
                //    //localStoryboard.AddAnimation(container, TransitionDuration, rotation, "(UIElement.Projection).(PlaneProjection.RotationY)", EasingFunction);
                //    //localStoryboard.AddAnimation(container, TransitionDuration, depth, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", EasingFunction);
                //    //localStoryboard.AddAnimation(container, TransitionDuration, translateX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)", EasingFunction);
                //    //localStoryboard.AddAnimation(container, TransitionDuration, offsetX, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", EasingFunction);
                //    //localStoryboard.AddAnimation(container, TransitionDuration, translateY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", EasingFunction);
                //    //localStoryboard.AddAnimation(container, TransitionDuration, 0, opacity, "Opacity", EasingFunction);
                //}
                //else
                //{
                //    container.Opacity = opacity;
                //}
            }

            //_rectangle.Fill = new SolidColorBrush(Colors.Gray);
            //_rectangle.Opacity = 0.9;
            //SetLeft(_rectangle, 0);
            //SetTop(_rectangle, (ActualHeight / 2));
            //SetZIndex(_rectangle, 1);
            //_rectangle.Width = ActualWidth;
            //_rectangle.Height = ActualHeight;

            if (localStoryboard.Children.Count > 0)
                localStoryboard.Begin();

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, availableSize.Width, availableSize.Height) };

            foreach (var uiElement in Children)
            {
                var container = (FrameworkElement) uiElement;
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return (availableSize);
        }

        private void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            var positionX = args.GetPosition(this).X;
            for (var i = 0; i < _internalList.Count; i++)
            {
                var child = _internalList[i];
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                if (!(positionX >= rect.Left) || !(positionX <= (rect.Left + rect.Width)))
                {
                    continue;
                }

                _isGesture = (i > SelectedIndex);

                SelectedIndex = i;
                return;
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            _initialX = args.GetCurrentPoint(this).Position.X;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            // minimum amount to declare as a manipulation
            const int threshold = 40;

            // last position
            var currentX = pointerRoutedEventArgs.GetCurrentPoint(this).Position.X;

            if (!(Math.Abs(currentX - _initialX) > threshold))
            {
                // here is a "Tap on Item"
                return;
            }

            _isGesture = (currentX < _initialX);

            // here is a manipulation
            if (_isGesture)
            {
                SelectedIndex = SelectedIndex < (_internalList.Count - 1) ? SelectedIndex + 1 : SelectedIndex;
            }
            else if (SelectedIndex > 0)
            {
                SelectedIndex--;
            }
            _initialX = currentX;
        }
    }
}