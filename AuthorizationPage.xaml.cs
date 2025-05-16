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
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Window
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        //открываем окно авторизации
        private void OpenRegistrationPage_ButtonClick(object sender, RoutedEventArgs e)
        {
            RegistrationPage page = new RegistrationPage();
            page.Show();
            this.Close();
        }

        private void SignIn_ButtonClick(object sender, RoutedEventArgs e)
        {
            using (var db = new PolycCursContext())
            {
                // Проверка данных пользователя
                var user = db.Users
                    .FirstOrDefault(u => u.Useremail == emailtext.Text && u.Userpassword == passwordtext.Password);

                // Проверка данных аккаунта доктора
                var doctorAccount = db.DoctorAccounts
                    .FirstOrDefault(d => d.Useremail == emailtext.Text && d.Userpassword == passwordtext.Password);

                if (user != null)
                {
                    UserAuth.UserStatus = "patient";
                    UserAuth.UserAuthorized = true;
                    MainWindow w = new MainWindow(user, null);
                    w.Show();
                    this.Close();
                }
                else if (doctorAccount != null)
                {
                    UserAuth.UserStatus = "doctor";
                    UserAuth.UserAuthorized = true;
                    MainWindow w = new MainWindow(null, doctorAccount);
                    w.Show();  
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверные данные для авторизации!", "Успех", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordPage page = new ForgotPasswordPage();
            page.Show();
        }
    }
}
