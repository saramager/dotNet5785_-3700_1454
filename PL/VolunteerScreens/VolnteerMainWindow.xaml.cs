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
/// לסדר עוד קצת להוסיף משקיפים לCALL ולעשות לחצנים לכלל דבר 
namespace PL.VolunteerScreens
{
    /// <summary>
    /// Interaction logic for VolnteerMainWindow.xaml
    /// </summary>
    public partial class VolnteerMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolnteerMainWindow), new PropertyMetadata(null));

        public BO.Call? Call
        {
            get { return (BO.Call?)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(VolnteerMainWindow), new PropertyMetadata(null));

        public VolnteerMainWindow(int id)
        {
            

            CurrentVolunteer = s_bl.Volunteer.ReadVolunteer(id);
            Call = null;
            if (CurrentVolunteer.callProgress != null)
            {

                Call = s_bl.Call.ReadCall(CurrentVolunteer.callProgress.CallId);
            }
            DataContext = this; // הגדרת DataContext
            InitializeComponent();

        }
        private void queryVolunteer()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = s_bl.Volunteer.ReadVolunteer(id) ;
        }

        /// <summary>
        /// Observer for volunteer changes
        /// </summary>
        private void volunteerObserver()
            => queryVolunteer();

        /// <summary>
        /// Adds the observer when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerObserver);

        /// <summary>
        /// Removes the observer when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerObserver);
    }
}
