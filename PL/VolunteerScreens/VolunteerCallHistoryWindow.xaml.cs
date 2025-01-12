using BO;
using PL.Volunteer;
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
    /// Interaction logic for VolunteerCallHistoryWindow.xaml
    /// </summary>
    public partial class VolunteerCallHistoryWindow : Window
    {
        public BO.CallType? filterCloseCalls { get; set; } = null;
        public BO.FiledOfClosedCallInList? sortCloseCalls { get; set; } = null;
        public int Id { get; set; }

        /// <summary>
        /// Static reference to the BL instance.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public IEnumerable<BO.ClosedCallInList> CloseCallsList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CloseCallsListProperty); }
            set { SetValue(CloseCallsListProperty, value); }
        }
       
        /// <summary>
        /// Dependency property for VolunteerList.
        /// </summary>
        public static readonly DependencyProperty CloseCallsListProperty =
            DependencyProperty.Register("CloseCallsList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(VolunteerCallHistoryWindow), new PropertyMetadata(null));


        public VolunteerCallHistoryWindow(int id)
        {
            Id = id;
            queryCloseCallsList();
            s_bl.Volunteer.AddObserver(Id,CallsListObserver);
            InitializeComponent();
        }
        private void queryCloseCallsList()
           => CloseCallsList = s_bl.Call.ReadCloseCallsVolunteer(Id, filterCloseCalls, sortCloseCalls);
        /// <summary>
        /// Observer function to update the volunteer list.
        /// </summary>
        private void CallsListObserver()
            => queryCloseCallsList();

        /// <summary>
        /// Handles the window loaded event to add the observer and load the list.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(Id,CallsListObserver);
            queryCloseCallsList(); // טוען מחדש את הרשימה עם פתיחת החלון
        }

        /// <summary>
        /// Handles the window closed event to remove the observer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(Id,CallsListObserver);

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCloseCallsList();
        }
    }
}
