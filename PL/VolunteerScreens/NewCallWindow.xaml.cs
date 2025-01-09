using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.VolunteerScreens
{
    /// <summary>
    /// Interaction logic for NewCallWindow.xaml
    /// </summary>
    public partial class NewCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.OpenCallInList? SelectedCall { get; set; } = null;
        public BO.CallType? filterOpenCalls { get; set; } = null;
        public BO.FiledOfOpenCallInList? sortOpenCalls { get; set; } = null;
        public int Id { get; set; }
        public IEnumerable<BO.OpenCallInList> OpenCallsList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallsListProperty); }
            set { SetValue(OpenCallsListProperty, value); }
        }

        /// <summary>
        /// Dependency property for VolunteerList.
        /// </summary>
        public static readonly DependencyProperty OpenCallsListProperty =
            DependencyProperty.Register("OpenCallsList", typeof(IEnumerable<BO.OpenCallInList>), typeof(NewCallWindow), new PropertyMetadata(null));
       
        public NewCallWindow(int id)
        {
            Id = id;
            queryOpenCallsList();
            s_bl.Call.AddObserver(CallsListObserver);
            DataContext = this;
            InitializeComponent();
        }
        private void queryOpenCallsList()
    => OpenCallsList = s_bl.Call.ReadOpenCallsVolunteer(Id, filterOpenCalls, sortOpenCalls);
        private void CallsListObserver()
           => queryOpenCallsList();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallsListObserver);
            queryOpenCallsList(); // טוען מחדש את הרשימה עם פתיחת החלון
        }

        /// <summary>
        /// Handles the window closed event to remove the observer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallsListObserver);
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryOpenCallsList();
        }
        private void addCall_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(SelectedCall.verbalDescription, "Do you want to Take this Call", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.ChooseCallTreat(Id, SelectedCall.ID);
                    MessageBox.Show("new call add");
                    this.Close();


                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "ERROR ", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}
