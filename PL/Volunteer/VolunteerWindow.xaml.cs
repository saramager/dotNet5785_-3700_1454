using BO;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        /// <summary>
        /// Instance of the BL layer
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Text for the add/update button
        /// </summary>
        public string ButtonText { get; set; }= "";

        /// <summary>
        /// Current volunteer being managed
        /// </summary>
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        /// <summary>
        /// Constructor for the VolunteerWindow
        /// </summary>
        /// <param name="id">ID of the volunteer (0 for new volunteer)</param>
        public VolunteerWindow(int id = 0)
        {
            this.Closed += Window_Closed;
            this.Loaded += Window_Loaded;
            ButtonText = id == 0 ? "Add" : "Update";
            Task.Run(() =>
            {
                var volunteer = (id != 0) ? s_bl.Volunteer.ReadVolunteer(id)! : new BO.Volunteer() { Id = 0 };
                Dispatcher.Invoke(() =>
                {
                    CurrentVolunteer = volunteer;
                });
            });
            InitializeComponent();

        }

        /// <summary>
        /// Handles the Add/Update button click
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer != null)
                {
                    if (ButtonText == "Add")
                    {
                        s_bl.Volunteer.CreateVolunteer(CurrentVolunteer);
                        MessageBox.Show("Successful add");
                    }
                    else
                    {
                        s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                        MessageBox.Show("Successful update");
                    }
                    this.Close();
                }
            }
            catch (BlDoesAlreadyExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Queries the current volunteer details
        /// </summary>
        private void queryVolunteer()
        {
            int id = CurrentVolunteer?.Id ?? 0;
            if (id != 0)
            {
                try
                {
                    Task.Run(() =>
                    {
                        var volunteer = s_bl.Volunteer.ReadVolunteer(id)!;

                        Dispatcher.Invoke(() =>
                        {
                            CurrentVolunteer = volunteer;
                        });
                    });
                }
                catch (BlDoesNotExistException ex) { MessageBox.Show(ex.Message); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else
                CurrentVolunteer = null;
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



