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
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));
        public BO.FiledOfVolunteerInList filedToFilter { get; set; } = BO.FiledOfVolunteerInList.ID;
        public VolunteerListWindow()
        {
            InitializeComponent();
            queryVolunteerList();

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
        {
            filedToFilter = (BO.FiledOfVolunteerInList)(((ComboBox)sender).SelectedItem);

            VolunteerList = s_bl?.Volunteer.GetVolunteerInList(null, filedToFilter)!;

        }
        //{
        //?????
        // 8 ג 1
        //}

        private void queryVolunteerList()
    => VolunteerList = (filedToFilter == BO.FiledOfVolunteerInList.ID) ?
        s_bl?.Volunteer.GetVolunteerInList(null, null)! : s_bl?.Volunteer.GetVolunteerInList(null, filedToFilter)!;

        private void VolunteerListObserver()
            => queryVolunteerList();

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //=> s_bl.Volunteer.AddObserver(VolunteerListObserver);
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
            queryVolunteerList(); // טוען מחדש את הרשימה עם פתיחת החלון
        }


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
