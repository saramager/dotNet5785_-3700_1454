﻿using BO;
using DO;
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
using System.Windows.Threading;
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
            InitializeComponent();
            this.Closed += Window_Closed;
            this.Loaded += Window_Loaded;
            Call = null;

            {

                Task.Run(() =>
            {
                var volunteer = s_bl.Volunteer.ReadVolunteer(id)!;

                Dispatcher.Invoke(() =>
                {
                    CurrentVolunteer = volunteer;
                    if (CurrentVolunteer.callProgress != null)
                    {

                        Call = s_bl.Call.ReadCall(CurrentVolunteer.callProgress.CallId);
                    }
                    s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
                    if (Call != null)
                        s_bl.Call.AddObserver(Call.ID, callObserver);
                });
            });
            }
        }



        private void queryVolunteer()
        {
            int id = CurrentVolunteer!.Id;
            Task.Run(() =>
            {
                var volunteer = s_bl.Volunteer.ReadVolunteer(id)!;

                Dispatcher.Invoke(() =>
                {
                    CurrentVolunteer = volunteer;
                    if (Call != null)
                    {
                        if (CurrentVolunteer.callProgress == null || CurrentVolunteer.callProgress.CallId != Call.ID)
                        {
                            s_bl.Call.RemoveObserver(Call.ID, callObserver);

                        }

                    }
                    if (CurrentVolunteer.callProgress != null && Call != null && CurrentVolunteer.callProgress.CallId != Call.ID)
                    {
                        s_bl.Call.AddObserver(CurrentVolunteer.callProgress.CallId, callObserver);
                    }
                    else



                        Call = null;
                    if (CurrentVolunteer.callProgress != null)
                    {

                        Call = s_bl.Call.ReadCall(CurrentVolunteer.callProgress.CallId);
                    }
               
                });
            });
        }

        /// <summary>
        /// Observer for volunteer changes
        /// </summary>
        private void volunteerObserver()
            => queryCall();

        /// <summary>
        /// Observer for Call changes
        /// </summary>
        private void callObserver()
            => queryCall();

        private void queryCall()
        {
            queryVolunteer();
        }
        /// <summary>
        /// Adds the observer when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
            {
            if (CurrentVolunteer != null)
            {
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
            }
            if (Call != null)
            s_bl.Call.AddObserver(Call.ID ,callObserver);}

        /// <summary>
        /// Removes the observer when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id,volunteerObserver);
            if (Call != null)
                s_bl.Call.RemoveObserver(Call.ID,callObserver);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.Id, CurrentVolunteer);
                MessageBox.Show("Successful update");
            }

            catch (BlDoesAlreadyExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (PaswordDoesNotstrongException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (PaswordDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (IdDoesNotVaildException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (PasswordIsNotCorrectException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (VolunteerCantUpadeOtherVolunteerException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (CantUpdatevolunteer ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EndCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.FinishTreat(CurrentVolunteer.Id, CurrentVolunteer.callProgress.ID);
                MessageBox.Show("finish Call ");

            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.VolunteerCantUpadeOtherVolunteerException ex)
            {
                MessageBox.Show(ex.Message, "Unauthorized Access", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.AssignmentAlreadyClosedException ex)
            {
                MessageBox.Show(ex.Message, "Assignment Closed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) // Catch all other exceptions
            {
                MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.cancelTreat(CurrentVolunteer.Id, CurrentVolunteer.callProgress.ID);
                MessageBox.Show("cancel Call ");
                
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Not Found Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.CantUpdatevolunteer ex)
            {
                MessageBox.Show(ex.Message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.VolunteerCantUpadeOtherVolunteerException ex)
            {
                MessageBox.Show(ex.Message, "Unauthorized Action", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex) // Catch all other exceptions
            {
                MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void NewCall_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer != null)
                
            {
          
                new NewCallWindow(CurrentVolunteer.Id).Show();
            }
            else
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void CallHistory_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer != null)   
                new VolunteerCallHistoryWindow(CurrentVolunteer.Id).Show();
            else
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
}
