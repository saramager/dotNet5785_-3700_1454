using BO;
using Newtonsoft.Json.Linq;
using PL.VolunteerScreens;
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

namespace PL
{
    /// <summary>
    /// Interaction logic EnterToSystemWindow.xaml
    /// </summary>
    public partial class EnterToSystemWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public RoleType Role { get; set; }
        public int  Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
       

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(EnterToSystemWindow), new PropertyMetadata(null));
        public string Password 
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(EnterToSystemWindow), new PropertyMetadata(""));

        public EnterToSystemWindow()
        {
            InitializeComponent();
        
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Role = s_bl.Volunteer.EnterToSystem(Id, Password);
                if (Role == RoleType.Volunteer)

                {
                    MessageBoxResult result = MessageBox.Show("Are you  want to open vlounteer window ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        new VolnteerMainWindow(Id).Show();
                    }

                }

                else
                {
                    new ChoseForManagerWindow(Id).Show();
                }
            }
            catch (BO.BlDoesNotExistException blEx)
            {
                MessageBox.Show("there is n volunteer with such Id");
            }
            catch (PaswordDoesNotExistException PasEx)
            {
                MessageBox.Show("there is no password entered");
            }
            catch (PasswordIsNotCorrectException PasEx)
            {
                MessageBox.Show(PasEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
    }
}
