using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace HearthstoneCards.Caroussel
{
    public class Caroussel : Canvas
    {
        // gestures
        private double _initialOffset;

        // internalList keep reference on all UIElement
        private readonly List<UIElement> _internalList = new List<UIElement>();

        public DataTemplate ItemTemplate { get; set; }

        // prevent ArrangeOverride on items adding
        private bool _isUpdatingPosition;

        // temp size
        private double _desiredWidth;
        private double _desiredHeight;

        // Gesture movement
        private bool _isIncrementing;

        public Caroussel()
        {
            PointerPressed += OnPointerPressed;
            ManipulationMode = ManipulationModes.TranslateX;
            ManipulationCompleted += OnManipulationCompleted;
            ManipulationDelta += OnManipulationDelta;
            Tapped += OnTapped;
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(Caroussel), new PropertyMetadata(0, OnItemsSourceChanged));

        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register("TransitionDuration", typeof(int), typeof(Caroussel), new PropertyMetadata(300));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(Caroussel), new PropertyMetadata(new ExponentialEase { EasingMode = EasingMode.EaseOut }));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnSelectedIndexPropertyChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Caroussel), new PropertyMetadata(null));

        public static readonly DependencyProperty MaxVisibleItemsProperty =
            DependencyProperty.Register("MaxVisibleItems", typeof(int), typeof(Caroussel), new PropertyMetadata(5, OnLightStonePropertyChanged));

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(double), typeof(Caroussel), new PropertyMetadata(0.0, OnLightStonePropertyChanged));

        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(double), typeof(Caroussel), new PropertyMetadata(0.0, OnLightStonePropertyChanged));

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register("TranslateX", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnLightStonePropertyChanged));

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register("TranslateY", typeof(int), typeof(Caroussel), new PropertyMetadata(0, OnLightStonePropertyChanged));

        private static void OnItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
                return;

            if (args.NewValue == args.OldValue)
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
                                for (int i = 0; i < caroussel._internalList.Count; i++)
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

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // set selected item
            d.SetValue(SelectedItemProperty, ((IList)d.GetValue(ItemsSourceProperty))[(int)e.NewValue]);

            // load more incremental items, if necessary
            var itemSource = (d.GetValue(ItemsSourceProperty) as ISupportIncrementalLoading);
            if (itemSource != null)
            {
                var newIndex = (int)e.NewValue;
                var maxVisible = (int)d.GetValue(MaxVisibleItemsProperty);
                var loadMore = (newIndex + maxVisible) >= ((IList)itemSource).Count;
                if (loadMore)
                {
                    itemSource.LoadMoreItemsAsync(0);
                }
            }

            // forward
            OnLightStonePropertyChanged(d, e);
        }

        private static void OnLightStonePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var caroussel = (Caroussel)d;
            caroussel.UpdatePosition();
        }

        public void RemoveAt(int index)
        {
            var uiElement = _internalList[index];

            //  this.isUpdatingPosition = true;
            // Remove from internal list
            _internalList.RemoveAt(index);

            // Remove from visual tree
            Children.Remove(uiElement);

            if (SelectedIndex == index)
            {
                SelectedIndex = SelectedIndex == 0 ? 0 : SelectedIndex - 1;
            }
            else if (SelectedIndex > index)
            {
                SelectedIndex--;
            }
            UpdatePosition();
        }

        private void Bind()
        {
            if (ItemsSource == null)
                return;

            Children.Clear();
            _internalList.Clear();

            foreach (var item in ItemsSource)
            {
                CreateItem(item);
            }
        }

        /// <summary>
        /// Create an item (load data template and bind).
        /// </summary>
        private void CreateItem(object item, double opacity = 1)
        {
            // distinguish between single item and list of items
            if (item is IEnumerable)
            {
                foreach (var item2 in (IEnumerable)item)
                {
                    CreateItem2(item2);
                }
            }
            else
            {
                CreateItem2(item, opacity);
            }
        }

        private void CreateItem2(object item, double opacity = 1)
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
        /// Update all positions. Launch every animations on all items with a unique StoryBoard.
        /// </summary>
        private void UpdatePosition()
        {
            var storyboard = new Storyboard();

            _isUpdatingPosition = true;

            for (var i = 0; i < _internalList.Count; i++)
            {
                // TODO optimize (x2 ?)
                // don't animate all items
                var inf = SelectedIndex - (MaxVisibleItems * 2);
                var sup = SelectedIndex + (MaxVisibleItems * 2);

                if (i < inf || i > sup)
                {
                    continue;
                }

                var item = _internalList[i];

                var planeProjection = item.Projection as PlaneProjection;
                if (planeProjection == null)
                {
                    continue;
                }

                // get target projection
                var props = GetProjection(i, 0d);

                // Zindex and Opacity
                var deltaFromSelectedIndex = Math.Abs(SelectedIndex - i);
                var zindex = (_internalList.Count * 100) - deltaFromSelectedIndex;
                SetZIndex(item, zindex);

                var opacity = 1d - (Math.Abs((double)(i - SelectedIndex) / (MaxVisibleItems + 1)));
                var newVisibility = deltaFromSelectedIndex > MaxVisibleItems ? Visibility.Collapsed : Visibility.Visible;

                // item already present
                if (item.Visibility == newVisibility && item.Visibility == Visibility.Visible)
                {
                    storyboard.AddAnimation(item, TransitionDuration, props.Item1, "(UIElement.Projection).(PlaneProjection.LocalOffsetX)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, props.Item2, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, props.Item3, "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, props.Item4, "(UIElement.Projection).(PlaneProjection.RotationY)", EasingFunction);
                    storyboard.AddAnimation(item, TransitionDuration, opacity, "Opacity", EasingFunction);
                }
                else switch (newVisibility)
                {
                    case Visibility.Visible:
                        // this animation will occur in the ArrangeOverride() method
                        item.Visibility = newVisibility;
                        item.Opacity = 0d;
                        break;
                    case Visibility.Collapsed:
                        storyboard.AddAnimation(item, TransitionDuration, 0d, "Opacity", EasingFunction);
                        storyboard.Completed += (sender, o) =>
                        {
                            item.Visibility = Visibility.Collapsed;
                        };
                        break;
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

        private Tuple<double, double, double, double> GetProjection(int i, double deltaX)
        {
            var isLeftToRight = deltaX > 0;
            var isRightToLeft = !isLeftToRight;

            var newDepth = -Depth;

            var initialRotation = (i == SelectedIndex) ? 0d : ((i < SelectedIndex) ? Rotation : -Rotation);
            double newRotation;

            var offsetX = (i == SelectedIndex) ? 0 : (i - SelectedIndex) * _desiredWidth;
            var translateX = (i == SelectedIndex) ? 0 : ((i < SelectedIndex) ? -TranslateX : TranslateX);
            var initialOffsetX = offsetX + translateX;
            double newOffsetX;

            var translateY = TranslateY;

            if (i == SelectedIndex)
            {
                // rotation is from -Rotation to Rotation
                // We get the proportional of deltaX by desiredWidth
                newRotation = initialRotation - Rotation * deltaX / _desiredWidth;

                // the offset max is the Sum(TranslateX + desiredWidth)
                // We get the proportional too
                newOffsetX = deltaX * (TranslateX + _desiredWidth) / _desiredWidth;
            }
            else if ((i == SelectedIndex - 1 && isLeftToRight) || (i == SelectedIndex + 1 && isRightToLeft))
            {
                // only the first item on the left or right is moving on x, z, and d

                // We get the rotation (proportional from delta to desiredwidth, always)
                // by far the initial position is Rotation, so we made a subsraction
                newRotation = initialRotation - Rotation * deltaX / _desiredWidth;

                // the Translation is decreasing to 0
                newOffsetX = initialOffsetX - initialOffsetX * Math.Abs(deltaX) / _desiredWidth;
            }
            else
            {
                // other items just moved on X
                newOffsetX = initialOffsetX + deltaX;
                newRotation = initialRotation;
            }

            // provide valid values to the animation
            if (double.IsNaN(newOffsetX))
            {
                newOffsetX = 0d;
            }
            if (double.IsNaN(newDepth))
            {
                newDepth = 0d;
            }
            if (double.IsNaN(newRotation))
            {
                newRotation = 0d;
            }
            return new Tuple<double, double, double, double>(newOffsetX, translateY, newDepth, newRotation);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, availableSize.Width, availableSize.Height) };

            foreach (var container in Children)
            {
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return (availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isUpdatingPosition)
            {
                return finalSize;
            }

            var centerLeft = 0.0;
            var centerTop = 0.0;

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

                // get delta position from SelectedIndex
                var deltaFromSelectedIndex = Math.Abs(SelectedIndex - i);

                // get rect position
                var rect = new Rect(centerLeft, centerTop, _desiredWidth, _desiredHeight);
                container.Arrange(rect);
                SetLeft(container, centerLeft);
                SetTop(container, centerTop);

                // get an initial projection (without move)
                // apply transformation
                var planeProjection = container.Projection as PlaneProjection;
                if (planeProjection == null)
                    continue;

                var props = GetProjection(i, 0d);
                planeProjection.LocalOffsetX = props.Item1;
                planeProjection.GlobalOffsetY = props.Item2;
                planeProjection.GlobalOffsetZ = props.Item3;
                planeProjection.RotationY = props.Item4;

                // calculate zindex and opacity
                var zindex = (_internalList.Count * 100) - deltaFromSelectedIndex;
                var opacity = 1d - (Math.Abs((double)(i - SelectedIndex) / (MaxVisibleItems + 1)));
                SetZIndex(container, zindex);
                container.Opacity = opacity;

                // item appearance
                if (container.Visibility == Visibility.Visible && container.Opacity == 0.0)
                {
                    localStoryboard.AddAnimation(container, TransitionDuration, 0, opacity, "Opacity", EasingFunction);
                }
                else
                {
                    container.Opacity = opacity;
                }
            }

            //rectangle.Fill = new SolidColorBrush(Colors.Black);
            //rectangle.Opacity = 0.9;
            //Canvas.SetLeft(rectangle, 0);
            //Canvas.SetTop(rectangle, (this.ActualHeight / 2));
            //Canvas.SetZIndex(rectangle, 1);
            //rectangle.Width = this.ActualWidth;
            //rectangle.Height = this.ActualHeight;

            // start storyboard
            if (localStoryboard.Children.Count > 0)
            {
                localStoryboard.Begin();
            }
            return finalSize;
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

                _isIncrementing = (i > SelectedIndex);

                SelectedIndex = i;
                return;
            }
        }

        public void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            _initialOffset = args.GetCurrentPoint(this).Position.X;
        }

        public void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            var deltaX = e.Cumulative.Translation.X;

            if (deltaX > _desiredWidth)
            {
                deltaX = _desiredWidth;
            }

            if (deltaX < -_desiredWidth)
            {
                deltaX = -_desiredWidth;
            }

            // TODO merge logic with the one from UpdatePositions()
            // don't animate all items
            var inf = SelectedIndex - (MaxVisibleItems * 2);
            var sup = SelectedIndex + (MaxVisibleItems * 2);

            for (var i = 0; i < _internalList.Count; i++)
            {
                if (i < inf || i > sup)
                {
                    continue;
                }

                var item = _internalList[i];

                var planeProjection = item.Projection as PlaneProjection;
                if (planeProjection == null)
                {
                    continue;
                }

                // get the new projection for the current item
                var props = GetProjection(i, deltaX);
                planeProjection.LocalOffsetX = props.Item1;
                planeProjection.GlobalOffsetY = props.Item2;
                planeProjection.GlobalOffsetZ = props.Item3;
                planeProjection.RotationY = props.Item4;

                // Zindex and Opacity

                // minimum amount of movement to declare as a manipulation
                var moveThreshold = (int)_desiredWidth / 4;

                // last position
                var clientX = e.Position.X;

                var zindexItemIndex = deltaX > 0 ? SelectedIndex - 1 : SelectedIndex + 1;
                var deltaFromSelectedIndex = Math.Abs(zindexItemIndex - i);

                // if we are under minimum amount, no Zindex applied
                if (Math.Abs(clientX - _initialOffset) > moveThreshold)
                {
                    var zindex = (_internalList.Count * 100) - deltaFromSelectedIndex;
                    SetZIndex(item, zindex);
                }
            }
        }

        public void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // minimum amount of movement to declare as a manipulation
            var moveThreshold = (int)_desiredWidth / 4;

            // last position
            var clientX = e.Position.X;

            // if not enough movement, go in last placement
            if (!(Math.Abs(clientX - _initialOffset) > moveThreshold))
            {
                // rec-enter to SelectedIndex;
                UpdatePosition();
                return;
            }

            _isIncrementing = (clientX < _initialOffset);

            // here is a manipulation
            if (_isIncrementing)
            {
                // We are trying to move the last item to the left.
                // Just raised an UpdatePosition to reposition it on the center of the screen.
                if (SelectedIndex == (_internalList.Count - 1))
                {
                    UpdatePosition();
                }
                else
                {
                    SelectedIndex = (SelectedIndex < (_internalList.Count - 1)) ? SelectedIndex + 1 : SelectedIndex;
                }
            }
            else if (SelectedIndex > 0)
            {
                SelectedIndex--;
            }
            else
            {
                // We are trying to move the first item to the right.
                // Just raised an UpdatePosition to reposition it on the center of the screen.
                if (SelectedIndex == 0)
                {
                    UpdatePosition();
                }
                else
                {
                    SelectedIndex = 0;
                }
            }
            _initialOffset = clientX;
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

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            //set { SetValue(SelectedItemProperty, value); }
        }

        public int MaxVisibleItems
        {
            get { return (int)GetValue(MaxVisibleItemsProperty); }
            set { SetValue(MaxVisibleItemsProperty, value); }
        }

        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        public double Depth
        {
            get { return (double)GetValue(DepthProperty); }
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
    }
}
