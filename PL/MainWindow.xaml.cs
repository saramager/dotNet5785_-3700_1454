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
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }

        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

        private void SetRiskRangeButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(RiskRange);
        }

        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock(); 
        }
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskRange();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnWindowLoaded;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void clickAddOneMinute(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Minute);
        }
        private void clickAddOnehour(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Hour);
        }
        private void clickAddOneDay(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Day);
        }
        private void clickAddOneMonth(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Month);
        }
        private void clickAddOneYear(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddToClock(BO.TimeUnit.Year);
        }
    }
}