﻿using BlApi;
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
using System.Windows.Threading;

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

        public static bool IsOpen { get; private set; } = false;
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
                new VolunteerWindow(SelectedVolunteer.ID, ManagerID).ShowDialog();
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
        /// Current sort field for volunteers.
        /// </summary>
        public BO.FiledOfVolunteerInList filedToSort { get; set; } = BO.FiledOfVolunteerInList.ID;
        public int ManagerID { get; set; }

        /// <summary>
        /// Dependency property for FilterActive.
        /// </summary>
        public static readonly DependencyProperty FilterActiveProperty =
            DependencyProperty.Register(
                "FilterActive",
                typeof(bool?),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null));

        /// <summary>
        /// Filter for active volunteers.
        /// </summary>
        public bool? FilterActive
        {
            get => (bool?)GetValue(FilterActiveProperty);
            set => SetValue(FilterActiveProperty, value);
        }
        /// <summary>
        /// Initializes the VolunteerListWindow and loads the volunteer list.
        /// </summary>
        public VolunteerListWindow(int managerID)
        {
            if (IsOpen)
            {
                throw new Exception("The Volunteer Management window is already open");
            }
            else
            {
                IsOpen = true;
          
            InitializeComponent();
            queryVolunteerList();
            this.Closed += Window_Closed;
            this.Loaded += Window_Loaded;
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
            this.ManagerID = managerID;
            }
        }

        /// <summary>
        /// Handles filtering volunteers based on the selected field.
        /// </summary>
        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
        {

            VolunteerList = s_bl?.Volunteer.GetVolunteerInList(FilterActive, filedToSort)!;
        }

        /// <summary>
        /// Queries the volunteer list based on the current filter.
        /// </summary>
        private void queryVolunteerList()
            => VolunteerList = (filedToSort == BO.FiledOfVolunteerInList.ID) ?
                s_bl?.Volunteer.GetVolunteerInList(FilterActive, null)! : s_bl?.Volunteer.GetVolunteerInList(FilterActive, filedToSort)!;

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        /// <summary>
        /// Observer function to update the volunteer list.
        /// </summary>
        private void VolunteerListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                });
        }
       

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
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
            IsOpen = false;
        }

    }
}





