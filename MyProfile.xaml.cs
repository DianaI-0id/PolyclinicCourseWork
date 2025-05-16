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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using System.ComponentModel;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для MyProfile.xaml
    /// </summary>
    public partial class MyProfile : UserControl, INotifyPropertyChanged
    {
        private User _user;
        private DoctorAccount _account;

        public MyProfile()
        {
            InitializeComponent();
        }

        public MyProfile(User user, DoctorAccount account)
        {
            InitializeComponent();
            _user = user;
            _account = account;

            using (var db = new PolycCursContext())
            {
                if (_user != null)
                {
                    DataContext = _user;

                    doctorPanel.Visibility = Visibility.Collapsed;
                    statusPanel.Visibility = Visibility.Visible;
                }
                else if (_account != null)
                {
                    // Запрашиваем информацию о специальности врача
                    var doctor = db.Doctors.FirstOrDefault(d => d.DoctorId == _account.DoctorId);
                    if (doctor != null)
                    {
                        // Создаем объект, который будет использоваться для привязки данных
                        var doctorContext = new
                        {
                            DoctorName = doctor.Doctorname,
                            Useremail = _account.Useremail,
                            Phone = _account.Phone,
                            Speciality = doctor.Speciality,
                            AppointmPrice = doctor.AppointmentPrice + " pуб."
                        };

                        DataContext = doctorContext;

                        doctorPanel.Visibility = Visibility.Visible;
                        statusPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChangePassword_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Получаем введенные значения паролей
            string currentPassword = CurrentPassword.Password;
            string newPassword = NewPassword.Password;
            string repeatPassword = RepeatPassword.Password;

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(repeatPassword))
            {
                MessageBox.Show("Все поля пароля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newPassword != repeatPassword)
            {
                MessageBox.Show("Новый пароль и его повтор не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new PolycCursContext())
            {
                // Проверяем текущий пароль
                var user = db.Users.FirstOrDefault(u => u.UserId == _user.UserId);
                if (user == null || user.Userpassword != currentPassword)
                {
                    MessageBox.Show("Неверный текущий пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновляем пароль
                user.Userpassword = newPassword;
                db.SaveChanges();

                MessageBox.Show("Пароль успешно изменен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentPassword.Clear();
                NewPassword.Clear();
                RepeatPassword.Clear();
            }
        }

    }
}
