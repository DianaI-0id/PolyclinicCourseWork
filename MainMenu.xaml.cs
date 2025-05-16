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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private User _user;
        public MainMenu()
        {
            InitializeComponent();
        }
        public MainMenu(User user)
        {
            InitializeComponent();
            _user = user;
            DataContext = _user;
        }
        private void CallDoctor_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new CallDoctorAtHome(_user);

            mainWindow.CloseSideMenu();
        }

        private void DoctorAppointment_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new DoctorAppointment(_user);

            mainWindow.CloseSideMenu();
        }

        private void ContactsInfo_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new Contacts(_user);

            mainWindow.CloseSideMenu();
        }

        private void DoctorsList_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new DoctorsListInfo(_user);

            mainWindow.CloseSideMenu();
        }

        //карта лояльности пользователя
        private void LoyaltyCard_ButtonClick(object sender, RoutedEventArgs e)
        {
            //тут будет открываться карта лояльности пользователя
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new MyLoyaltyCard(_user);

            mainWindow.CloseSideMenu();
        }
    }
}
