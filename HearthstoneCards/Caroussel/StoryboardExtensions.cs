using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace HearthstoneCards.Caroussel
{
    public static class StoryboardExtensions
    {
        public static void AddAnimation(this Storyboard storyboard, DependencyObject element, Timeline timeline, string propertyPath)
        {
            storyboard.Children.Add(timeline);
            Storyboard.SetTarget(timeline, element);
            Storyboard.SetTargetProperty(timeline, propertyPath);
        }

        public static void AddAnimation(this Storyboard storyboard, DependencyObject element, int duration, double toValue, string propertyPath, EasingFunctionBase easingFunction = null)
        {
            var animation = new DoubleAnimation
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            if (easingFunction != null)
            {
                animation.EasingFunction = easingFunction;
            }

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, propertyPath);
        }

        public static void AddAnimation(this Storyboard storyboard, DependencyObject element, int duration, double fromValue, double toValue, string propertyPath, EasingFunctionBase easingFunction = null)
        {
            var animation = new DoubleAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            if (easingFunction != null)
            {
                animation.EasingFunction = easingFunction;
            }

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, propertyPath);
        }

        public static void Animate(this DependencyObject element, string propertyPath, int duration, double toValue, EasingFunctionBase easingFunction = null, EventHandler<object> completed = null)
        {
            var timeline = new DoubleAnimation
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            if (easingFunction != null)
            {
                timeline.EasingFunction = easingFunction;
            }

            var storyboard = new Storyboard();
            if (completed != null)
            {
                storyboard.Completed += completed;
            }

            storyboard.Children.Add(timeline);
            Storyboard.SetTarget(storyboard, element);
            Storyboard.SetTargetProperty(storyboard, propertyPath);
            storyboard.Begin();
        }

        public static void Animate(this DependencyObject element, string propertyPath, int duration, int startingDuration, double toValue, EasingFunctionBase easingFunction = null, EventHandler<object> completed = null)
        {
            var timeline = new DoubleAnimation
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            if (easingFunction != null)
            {
                timeline.EasingFunction = easingFunction;
            }

            var storyboard = new Storyboard();
            if (completed != null)
            {
                storyboard.Completed += completed;
            }

            storyboard.Children.Add(timeline);
            Storyboard.SetTarget(storyboard, element);
            Storyboard.SetTargetProperty(storyboard, propertyPath);
            storyboard.BeginTime = TimeSpan.FromMilliseconds(startingDuration);
            storyboard.Begin();
        }
    }
}
