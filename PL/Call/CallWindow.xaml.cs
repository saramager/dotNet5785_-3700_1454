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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {

        /// <summary>
        /// Instance of the BL layer
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Text for the add/update button
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        /// Current Call being managed
        /// </summary>
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentCall
        /// </summary>
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        /// <summary>
        /// Constructor for the CallWindow
        /// </summary>
        /// <param name="id">ID of the Call (0 for new Call)</param>
        public CallWindow(int id = 0)
        {
            CurrentCall = (id != 0) ? s_bl.Call.ReadCall(id)! : new BO.Call() { ID = 0 };
            this.Closed += Window_Closed;
            this.Loaded += Window_Loaded;
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Add/Update button click
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall != null)
                {
                    if (ButtonText == "Add")
                    {
                        s_bl.Call.CreateCall(CurrentCall);
                        MessageBox.Show("Successful add");
                    }
                    else
                    {
                        s_bl.Call.UpdateCall(CurrentCall);
                        MessageBox.Show("Successful update");
                    }
                    this.Close();
                }
            }
            catch (BlDoesAlreadyExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Queries the current Call details
        /// </summary>
        private void queryCall()
        {
            int id = CurrentCall?.ID ?? 0;
            CurrentCall = id != 0 ? s_bl.Call.ReadCall(id) : null;
        }

        /// <summary>
        /// Observer for Call changes
        /// </summary>
        private void callObserver()
            => queryCall();

        /// <summary>
        /// Adds the observer when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callObserver);

        /// <summary>
        /// Removes the observer when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(callObserver);
    }
}
