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

namespace PolyclinicProjectKurs
{
    public partial class ServicesWithoutAuth : UserControl
    {
        public ServicesWithoutAuth()
        {
            InitializeComponent();
        }

        private void SignIn_ButtonClick(object sender, RoutedEventArgs e)
        {
            AuthorizationPage page = new AuthorizationPage();
            page.Show();

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.Close();
        }

        private void Registration_ButtonClick(object sender, RoutedEventArgs e)
        {
            RegistrationPage page = new RegistrationPage();
            page.Show();

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.Close();
        }

        private void DoctorAppointment_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new DoctorAppointment(null);
        }

        private void CancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new CancelAppointmentPage(); //добавить окно для поиска пользователя по его фио и номеру телефона
        }
    }
}
