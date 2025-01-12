using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        /// <summary>
        /// Static reference to the BL instance.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Selected volunteer from the list.
        /// </summary>
        public BO.VolunteerInList? SelectedVolunteer { get; set; }
        /// <summary>
        /// Handles double-clicking on a volunteer in the list.
        /// Opens a detailed volunteer window.
        /// </summary>
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
            if (SelectedVolunteer != null)
            {
                new VolunteerWindow(SelectedVolunteer.ID).ShowDialog();
            }
        }

        /// <summary>
        /// Handles the "Add" button click to open the add volunteer window.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var volunteerWindow = new VolunteerWindow();
            volunteerWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the "Delete" button click to delete a selected volunteer.
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //var volunteer = button?.DataContext as BO.VolunteerInList;

            if (SelectedVolunteer == null)
                return;

            var result = MessageBox.Show("Are you sure you want to delete this volunteer?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer.ID);
                    VolunteerList = s_bl.Volunteer.GetVolunteerInList(null, filedToSort);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.volunteerHandleCallException ex)
                {
                    MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// List of volunteers displayed in the UI.
        /// </summary>
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        /// <summary>
        /// Dependency property for VolunteerList.
        /// </summary>
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        /// <summary>
        /// Current filter field for volunteers.
        /// </summary>
        ///  צריך לבדוק אם לשנות את GetCallInList כי הוא כרגע לא מקבל ערך לסינון אלא רק למיון


        /// <summary>
        /// Current sort field for volunteers.
        /// </summary>
        public BO.FiledOfVolunteerInList filedToSort { get; set; } = BO.FiledOfVolunteerInList.ID;
        /// <summary>
        /// Initializes the VolunteerListWindow and loads the volunteer list.
        /// </summary>
        public VolunteerListWindow()
        {
            InitializeComponent();
            queryVolunteerList();
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }

        /// <summary>
        /// Handles selection changes in the data grid.
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Handles filtering volunteers based on the selected field.
        /// </summary>
        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
        {

            VolunteerList = s_bl?.Volunteer.GetVolunteerInList(null, filedToSort)!;
        }

        /// <summary>
        /// Queries the volunteer list based on the current filter.
        /// </summary>
        private void queryVolunteerList()
            => VolunteerList = (filedToSort == BO.FiledOfVolunteerInList.ID) ?
                s_bl?.Volunteer.GetVolunteerInList(null, null)! : s_bl?.Volunteer.GetVolunteerInList(null, filedToSort)!;

        /// <summary>
        /// Observer function to update the volunteer list.
        /// </summary>
        private void VolunteerListObserver()
            => queryVolunteerList();

        /// <summary>
        /// Handles the window loaded event to add the observer and load the list.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
            queryVolunteerList(); // טוען מחדש את הרשימה עם פתיחת החלון
        }

        /// <summary>
        /// Handles the window closed event to remove the observer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
    }
}





//using BlApi;
//using System;
//using System.Collections.Generic;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL.Volunteer
//{
//    public partial class VolunteerListWindow : Window
//    {
//        static readonly IBl s_bl = Factory.Get();

//        public IEnumerable<BO.VolunteerInList> VolunteerList
//        {
//            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
//            set { SetValue(VolunteerListProperty, value); }
//        }

//        public static readonly DependencyProperty VolunteerListProperty =
//            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

//        public BO.FiledOfVolunteerInList filedToFilter { get; set; } = BO.FiledOfVolunteerInList.ID;

//        public VolunteerListWindow()
//        {
//            InitializeComponent();
//            queryVolunteerList(); // טוען את הנתונים עם פתיחת החלון
//        }

//        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

//        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
//        {
//            filedToFilter = (BO.FiledOfVolunteerInList)(((ComboBox)sender).SelectedItem);
//            queryVolunteerList(); // מעדכן את הרשימה על פי הפילטר
//        }

//        private void queryVolunteerList()
//        {
//            VolunteerList = (filedToFilter == BO.FiledOfVolunteerInList.ID)
//                ? s_bl.Volunteer.GetVolunteerInList(null, null)
//                : s_bl.Volunteer.GetVolunteerInList(null, filedToFilter);
//        }

//        private void VolunteerListObserver() => queryVolunteerList();

//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            s_bl.Volunteer.AddObserver(VolunteerListObserver);
//            queryVolunteerList(); // טוען מחדש את הרשימה עם פתיחת החלון
//        }

//        private void Window_Closed(object sender, EventArgs e)
//        {
//            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
//        }
//    }
//}
