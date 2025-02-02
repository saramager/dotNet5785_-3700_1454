﻿using BlApi;
using Newtonsoft.Json.Linq;
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
using System.Windows.Threading;

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
        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }


        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public bool  IsSimulatorWork
        {
            get { return (bool)GetValue(IsSimulatorWorkProperty); }
            set { SetValue(IsSimulatorWorkProperty, value); }
        }


        public static readonly DependencyProperty IsSimulatorWorkProperty =
            DependencyProperty.Register("IsSimulatorWork", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
       
        public int managerID { get; set; } 
        public static  bool IsOpen { get; private set; } = false;

        /// <summary>
        /// Constructor for the MainWindow
        /// </summary>
        public MainWindow(int MId)
        {
            
            if (IsOpen)
            {
                throw new Exception("Manager window is already open");

            }
            else
            {
                IsOpen = true;  
            }
                InitializeComponent();

            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(callsListObserver);

            this.Closed += OnWindowClosed;
            this.Loaded += OnWindowLoaded;
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskRange();
            CallSums = s_bl.Call.SumOfCalls();
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
        public int[] CallSums 
        {
            get { return (int[])GetValue(CallSumsProperty); }
            set { SetValue(CallSumsProperty, value); }
        }

        /// <summary>
        /// Dependency property for RiskRange
        /// </summary>
        public static readonly DependencyProperty CallSumsProperty =
            DependencyProperty.Register("CallSums", typeof(int[]), typeof(MainWindow));
        /// <summary>
        /// Updates the risk range value
        /// </summary>
        private void SetRiskRangeButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(RiskRange);
        }

        private volatile DispatcherOperation? _observerOperation1 = null; //stage 7

        /// <summary>
        /// Updates CurrentTime by observing clock changes
        /// </summary>
        private void clockObserver()
        {
            if (_observerOperation1 is null || _observerOperation1.Status == DispatcherOperationStatus.Completed)
                _observerOperation1 = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                    CallSums = s_bl.Call.SumOfCalls();
                });
        }

        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7
        /// <summary>
        /// Updates CallSums by observing call changes
        /// </summary>
        private void callsListObserver()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
                _observerOperation2 = Dispatcher.BeginInvoke(() =>
                {
                    CallSums = s_bl.Call.SumOfCalls();
                });
        }
        private volatile DispatcherOperation? _observerOperation3 = null; //stage 7
        /// <summary>
        /// Updates RiskRange by observing configuration changes
        /// </summary>
        private void configObserver()
        {
            if (_observerOperation3 is null || _observerOperation3.Status == DispatcherOperationStatus.Completed)
                _observerOperation3 = Dispatcher.BeginInvoke(() =>
                {
                    RiskRange = s_bl.Admin.GetRiskRange();
                    CallSums = s_bl.Call.SumOfCalls();
                });

        }

        /// <summary>
        /// Cleans up observers when the window is closed
        /// </summary>
        private void OnWindowClosed(object? sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            IsOpen = false;
            if (IsSimulatorWork)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorWork = false;

            }
        }

        /// <summary>
        /// Initializes observers and properties when the window is loaded
        /// </summary>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskRange();
            CallSums = s_bl.Call.SumOfCalls();

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
            try
            {
                new VolunteerListWindow(managerID).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /// <summary>
        /// Opens the Call List window
        /// </summary>
        private void ButtonCallList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new Call.CallListWindow(managerID).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
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
                s_bl.Admin.InitializeDB();
                managerID= s_bl.Volunteer.ManagerID();   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ButtonSimulator_Click(object sender, RoutedEventArgs e)
        {
            if(IsSimulatorWork)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorWork = false;
                
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorWork=true;
            }
       

        }
    }
}
