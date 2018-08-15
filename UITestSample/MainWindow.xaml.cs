namespace UITestSample
{
    using System.Windows;
    using System.Windows.Media;
    using System.Threading;
    using System.Threading.Tasks;
    using Tests;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
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

        private async void RunUITests(object sender, RoutedEventArgs e)
        {
            AutoResetEvent eventControl = null;
            bool testSucceeded = false;
            await Task.Run(() =>
            {
                eventControl = new AutoResetEvent(false);

                // Ejecuta la prueba.
                LoginUITest test = new LoginUITest(eventControl);
                if (!test.Login())
                {
                    testSucceeded = false;
                }
                else
                {
                    // Espera hasta que el login se haya completado, o el tiempo de espera límite es alcanzado.
                    testSucceeded = eventControl.WaitOne(LOGIN_TIMEOUT);
                    Thread.Sleep(5000);
                }
            });

            if (testSucceeded)
            {
                LoginCheck.Visibility = Visibility.Visible;
            }
            else
            {
                LoginFail.Visibility = Visibility.Visible;
            }

            LoginLabel.Foreground = new SolidColorBrush(Colors.Black);
        }

        private async Task Sleep(int time)
        {
            await Task.Run(() => Thread.Sleep(time));
        }
    }
}