namespace UITestSample
{
    using System.Windows;
    using System.Windows.Media;
    using System.Threading;
    using System.Threading.Tasks;
    using Tests;


    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int LOGIN_TIMEOUT = 30000;
        private const int LAUNCHDEMO_TIMEOUT = 10000;
        private const int OPENDATOS_TIMEOUT = 10000;



        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Runs all user interface tests.
        /// </summary>
        private async void RunUITests(object sender, RoutedEventArgs e)
        {
            AutoResetEvent eventControl = null;
            bool testSucceeded = false;
            await Task.Run(() =>
            {
                eventControl = new AutoResetEvent(false);

                // Runs the login test.
                LoginUITest test = new LoginUITest(eventControl);
                if (!test.Login())
                {
                    testSucceeded = false;
                }
                else
                {
                    // Waits for the login to be completed, or the timeout is reached.
                    testSucceeded = eventControl.WaitOne(LOGIN_TIMEOUT);
                }
            });

            // Visual feedback.
            LoginCheck.Visibility = testSucceeded ? Visibility.Visible : Visibility.Collapsed;
            LoginFail.Visibility = testSucceeded ? Visibility.Collapsed : Visibility.Visible;
            LoginLabel.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}