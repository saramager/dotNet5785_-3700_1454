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
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private string ButtonText { get; set; }
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }


        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));


        public VolunteerWindow(int id = 0)
        {
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.ReadVolunteer(id)! : new BO.Volunteer() { Id = 0 };
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try { if (ButtonText == "Add")
                {
                    s_bl.Volunteer.CreateVolunteer(CurrentVolunteer);
                    MessageBox.Show("secssful add");
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                    MessageBox.Show("secssful update ");

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void queryVolunteer(){
            int id = CurrentVolunteer?.Id ?? 0;

            CurrentVolunteer =id != 0? s_bl.Volunteer.ReadVolunteer(id): null;



        }
        private void volunteerObserver()
            => queryVolunteer();
 
private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Volunteer.AddObserver(volunteerObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerObserver);

    }
}

       
    
