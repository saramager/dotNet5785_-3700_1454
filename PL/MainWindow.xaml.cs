using BlApi;
using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instance of the BL layer
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int managerID { get; set; }  
        /// <summary>
        /// Constructor for the MainWindow
        /// </summary>
        public MainWindow(int MId)
        {
            InitializeComponent();

            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);

            this.Closed += OnWindowClosed;
            this.Loaded += OnWindowLoaded;
            managerID = MId;
        }

        /// <summary>
        /// Current time from the system clock
        /// </summary>
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentTime
        /// </summary>
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        /// <summary>
        /// Risk range configuration
        /// </summary>
        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }

        /// <summary>
        /// Dependency property for RiskRange
        /// </summary>
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

        /// <summary>
        /// Updates the risk range value
        /// </summary>
        private void SetRiskRangeButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(RiskRange);
        }

        /// <summary>
        /// Updates CurrentTime by observing clock changes
        /// </summary>
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        /// <summary>
        /// Updates RiskRange by observing configuration changes
        /// </summary>
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskRange();
        }

        /// <summary>
        /// Cleans up observers when the window is closed
        /// </summary>
        private void OnWindowClosed(object? sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        /// <summary>
        /// Initializes observers and properties when the window is loaded
        /// </summary>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        /// <summary>
        /// Adds one minute to the clock
        /// </summary>
        private void clickAddOneMinute(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Minute);
        }

        /// <summary>
        /// Adds one hour to the clock
        /// </summary>
        private void clickAddOnehour(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Hour);
        }

        /// <summary>
        /// Adds one day to the clock
        /// </summary>
        private void clickAddOneDay(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Day);
        }

        /// <summary>
        /// Adds one month to the clock
        /// </summary>
        private void clickAddOneMonth(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Month);
        }

        /// <summary>
        /// Adds one year to the clock
        /// </summary>
        private void clickAddOneYear(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Year);
        }

        /// <summary>
        /// Opens the Volunteer List window
        /// </summary>
        private void ButtonVolunteerList_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        /// <summary>
        /// Opens the Call List window
        /// </summary>
        private void ButtonCallList_Click(object sender, RoutedEventArgs e)
        {
            new Call.CallListWindow(managerID).Show();
        }

        /// <summary>
        /// Initializes the database with confirmation
        /// </summary>
        private void Initialize_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                InitializeDatabase();
            }
        }

        /// <summary>
        /// Performs the database initialization
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                CloseAllWindowsExceptMain();
                s_bl.Admin.UpdateDB();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Resets the database with confirmation
        /// </summary>
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the database?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ResetDatabase();
            }
        }

        /// <summary>
        /// Performs the database reset
        /// </summary>
        private void ResetDatabase()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                CloseAllWindowsExceptMain();
                s_bl.Admin.RestartDB();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Closes all open windows except the main window
        /// </summary>
        private void CloseAllWindowsExceptMain()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }
        }
    }
}
