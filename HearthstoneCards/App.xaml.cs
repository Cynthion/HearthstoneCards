using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using HearthstoneCards.Common;
using HearthstoneCards.Helper;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace HearthstoneCards
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection _transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            // global exception handling
            // this.UnhandledException += App_UnhandledException;

            // initialize app
            Initializer.Initialize();
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            //Frame rootFrame = Window.Current.Content as Frame;
            RootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                RootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = RootFrame;
            }

            if (RootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (RootFrame.ContentTransitions != null)
                {
                    this._transitions = new TransitionCollection();
                    foreach (var c in RootFrame.ContentTransitions)
                    {
                        this._transitions.Add(c);
                    }
                }

                RootFrame.ContentTransitions = null;
                RootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!RootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this._transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity

            await SuspensionManager.SaveAsync();

            deferral.Complete();
        }

        #region OAuth2 Authentication

        // from http://msicc.net/?p=4054
        public static ContinuationManager ContinuationManager { get; private set; } // must be global
        public static Frame RootFrame { get; set; }

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            ContinuationManager = new ContinuationManager();

            CreateRootFrame();

            // Restore the saved session state only when appropriate
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }

            //Check if this is a continuation
            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                ContinuationManager.Continue(continuationEventArgs);
            }

            Window.Current.Activate();
        }

        private void CreateRootFrame()
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame != null)
                return;

            // Create a Frame to act as the navigation context and navigate to the first page
            RootFrame = new Frame();

            //Associate the frame with a SuspensionManager key                                
            SuspensionManager.RegisterFrame(RootFrame, "AppFrame");

            // Set the default language
            RootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

            RootFrame.NavigationFailed += OnNavigationFailed;

            // Place the frame in the current Window
            Window.Current.Content = RootFrame;
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // TODO change this code for your needs
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        #endregion
    }
}