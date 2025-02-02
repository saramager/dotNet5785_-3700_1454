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

        /// <summary>
        /// the id of the volunteer
        /// </summary>
        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        /// <summary>
        /// Using DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(EnterToSystemWindow), new PropertyMetadata(null));
        /// <summary>
        /// the password of the volunteer
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// Using DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(EnterToSystemWindow), new PropertyMetadata(""));

        /// <summary>
        /// the constructor of the window
        /// </summary>
        public EnterToSystemWindow()
        {
            InitializeComponent();

        }
        /// <summary>
        /// the function that open the window of the volunteer or the manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        new VolunteerMainWindow(Id).Show();
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
        /// <summary>
        /// the function that open the window of the register
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    var request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }

                // בדיקה אם כל השדות מלאים לפני התחברות
                if (!string.IsNullOrWhiteSpace(Id.ToString()) && !string.IsNullOrWhiteSpace(Password))
                {
                    LoginButton_Click(sender, e);
                }
            }
        }
        /// <summary>
        /// the function that open the window of the register
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Role = s_bl.Volunteer.EnterToSystem(Id, Password);
                    if (Role == RoleType.Volunteer)

                    {
                        MessageBoxResult result = MessageBox.Show("Are you  want to open vlounteer window ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            new VolunteerMainWindow(Id).Show();
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
}
