using BO;
using DO;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        /// <summary>
        /// Static reference to the BL instance.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
       // static readonly DalApi.IDal s_dal = DalApi.Factory.Get;
       public int managerID { get; set; }
        /// <summary>
        /// Selected call from the list.
        /// </summary>
        public BO.CallInList? SelectedCall { get; set; }
        /// <summary>
        /// Handles double-clicking on a call in the list.
        /// Opens a detailed call window.
        /// </summary>
        private void lsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
            if (SelectedCall != null)
            {
                new CallWindow(SelectedCall.CallId).ShowDialog();
            }
        }

        /// <summary>
        /// Handles the "Add" button click to open the add call window.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var callWindow = new CallWindow();
            callWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the "Delete" button click to delete a selected call.
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var call = button?.DataContext as BO.CallInList;

            if (call == null)
                return;

            var result = MessageBox.Show("Are you sure you want to delete this volunteer?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.DeleteCall(call.CallId);
                    CallList = s_bl.Call.GetCallInList(filedToFilter,null , filedToSort);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show($"Failed to delete call: {ex.Message}", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.CantDeleteCallException ex)
                {
                    MessageBox.Show($"Failed to delete call: {ex.Message}", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var call = button?.DataContext as BO.CallInList;

            if (call == null)
                return;

            var result = MessageBox.Show("Are you sure you want to cancel this call?", "cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                   // int managerID = (int)(s_bl.Volunteer.findVolunteer(v => v.role == BO.role.Manager)?.Id ?? throw new InvalidOperationException("No manager found"));
                    int? callID = call.ID;
                    s_bl.Call.cancelTreat(managerID, callID);
                    CallList = s_bl.Call.GetCallInList(filedToFilter, null, filedToSort);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show($"Failed to cancel call: {ex.Message}", "Cancel failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.CantUpdatevolunteer ex)
                {
                    MessageBox.Show($"Failed to cancel call: {ex.Message}", "Cancel failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        /// <summary>
        /// List of calls displayed in the UI.
        /// </summary>
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        /// <summary>
        /// Dependency property for CallList.
        /// </summary>
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        /// <summary>
        /// Current filter field for Calls.
        /// </summary>
        public BO.FiledOfCallInList filedToFilter { get; set; } = BO.FiledOfCallInList.ID;
        public BO.FiledOfCallInList filedToSort{ get; set; } = BO.FiledOfCallInList.ID;

        public object? Filter;
        /// <summary>
        /// Initializes the VolunteerListWindow and loads the volunteer list.
        /// </summary>
        public CallListWindow(int managerID)
        {
            InitializeComponent();
            queryCallList();
            s_bl.Call.AddObserver(CallListObserver);
            this.managerID = managerID;
        }

        /// <summary>
        /// Handles selection changes in the data grid.
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Handles filtering calls based on the selected field.
        /// </summary>
        private void CallFilter(object sender, SelectionChangedEventArgs e)
        {
            filedToFilter = (BO.FiledOfCallInList)(((ComboBox)sender).SelectedItem);

            CallList = s_bl?.Call.GetCallInList(filedToFilter,null , null)!;
        }

        /// <summary>
        /// Handles sorting calls based on the selected field.
        /// </summary>
        private void CallSort(object sender, SelectionChangedEventArgs e)
        {
            filedToSort = (BO.FiledOfCallInList)(((ComboBox)sender).SelectedItem);

            CallList = s_bl?.Call.GetCallInList(null, null, filedToSort)!;
        }
        /// <summary>
        /// Queries the call list based on the current filter.
        /// </summary>
        private void queryCallList()
            => CallList = (filedToFilter == BO.FiledOfCallInList.ID) ?
                s_bl?.Call.GetCallInList(null, null, null)! : s_bl?.Call.GetCallInList(filedToFilter, null, filedToSort)!;

        /// <summary>
        /// Observer function to update the Call list.
        /// </summary>
        private void CallListObserver()
            => queryCallList();

        /// <summary>
        /// Handles the window loaded event to add the observer and load the list.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(CallListObserver);
            queryCallList(); // טוען מחדש את הרשימה עם פתיחת החלון
        }

        /// <summary>
        /// Handles the window closed event to remove the observer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(CallListObserver);
    }
}




