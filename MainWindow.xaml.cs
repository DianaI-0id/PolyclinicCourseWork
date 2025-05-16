using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;

namespace PolyclinicProjectKurs
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private User _user;
        private DoctorAccount _account;
        private Doctor _doctor;
        private Visibility _infoVisibility = Visibility.Collapsed;
        private Visibility _patientVisibility = Visibility.Collapsed;
        private Visibility _doctorVisibility = Visibility.Collapsed;

        public Visibility patientVisibility
        {
            get { return _patientVisibility; }
            set
            {
                if (_patientVisibility != value)
                {
                    _patientVisibility = value;
                    OnPropertyChanged(nameof(patientVisibility));
                }
            }
        }

        public Visibility DoctorVisibility
        {
            get { return _doctorVisibility; }
            set
            {
                if (_doctorVisibility != value)
                {
                    _doctorVisibility = value;
                    OnPropertyChanged(nameof(DoctorVisibility));
                }
            }
        }

        public Visibility InfoVisibility
        {
            get { return _infoVisibility; }
            set
            {
                if (_infoVisibility != value)
                {
                    _infoVisibility = value;
                    OnPropertyChanged(nameof(InfoVisibility));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this; 
        }

        public MainWindow(User user, DoctorAccount account)
        {
            InitializeComponent();
            _user = user;
            _account = account;
            _infoVisibility = Visibility.Collapsed;

            if (UserAuth.UserAuthorized == false)
            {
                ContentControlPage.Content = new ServicesWithoutAuth();
            }
            else if (UserAuth.UserAuthorized == true && UserAuth.UserStatus == "doctor")
            {
                ContentControlPage.Content = new MyProfile(null, _account);
            }
            else if (UserAuth.UserAuthorized == true && UserAuth.UserStatus == "patient")
            {
                ContentControlPage.Content = new MainMenu(_user);
            }

            DataContext = this; 
        }

        // Возможность перемещать окно
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // Выход из всего приложения
        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        // Выходим из авторизованного аккаунта
        private void ExitUser_ButtonClick(object sender, RoutedEventArgs e)
        {
            UserAuth.UserAuthorized = false;

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new ServicesWithoutAuth(); 

            CloseSideMenu();
        }

        // Переходим в раздел Мой профиль
        private void MyProfile_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!UserAuth.UserAuthorized)
            {
                MessageBox.Show("Вы не авторизованы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (UserAuth.UserStatus == "doctor")
                {
                    ContentControlPage.Content = new MyProfile(null, _account); 
                }
                else if (UserAuth.UserStatus == "patient")
                {
                    ContentControlPage.Content = new MyProfile(_user, null); 
                }
                CloseSideMenu();

            }
        }

        // Переходим в главное меню
        private void GoToMainMenu(object sender, RoutedEventArgs e)
        {
            if (!UserAuth.UserAuthorized)
            {
                ContentControlPage.Content = new ServicesWithoutAuth(); 
            }
            else
            {
                ContentControlPage.Content = new MainMenu(_user); 
            }
            CloseSideMenu();
        }

        // Кнопка закрытия / открытия меню
        private void ToggleMenu(object sender, RoutedEventArgs e)
        {
            if (SideMenu.ActualWidth > 40)
                CloseSideMenu();
            else
                OpenSideMenu();
        }

        public void OpenSideMenu()
        {
            DoubleAnimation animation = new DoubleAnimation(200, TimeSpan.FromSeconds(0.2));
            SideMenu.BeginAnimation(Grid.WidthProperty, animation);
            InfoVisibility = Visibility.Visible;

            if (!UserAuth.UserAuthorized)
            {
                DoctorVisibility = Visibility.Collapsed;
                patientVisibility = Visibility.Visible;
            }
            else if (UserAuth.UserStatus == "patient")
            {
                DoctorVisibility = Visibility.Collapsed;
                patientVisibility = Visibility.Visible;
            }
            else if (UserAuth.UserStatus == "doctor")
            {
                DoctorVisibility = Visibility.Visible;
                patientVisibility = Visibility.Collapsed;
            }
        }

        public void CloseSideMenu()
        {
            DoubleAnimation animation = new DoubleAnimation(40, TimeSpan.FromSeconds(0.2));
            SideMenu.BeginAnimation(Grid.WidthProperty, animation);
            InfoVisibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenSideMenu();
        }

        private void MedicalRecords_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!UserAuth.UserAuthorized)
            {
                MessageBox.Show("Вы не авторизованы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ContentControlPage.Content = new MedicalRecords(_user); //заменить на Profile

                CloseSideMenu();
            }
        }

        private void DoctorAppointment(object sender, RoutedEventArgs e)
        {
            ContentControlPage.Content = new DoctorAppointment(_user); //заменить на Profile

            CloseSideMenu();
        }

        private void MyAppointments(object sender, RoutedEventArgs e)
        {
            if (!UserAuth.UserAuthorized)
            {
                MessageBox.Show("Вы не авторизованы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ContentControlPage.Content = new MyAppointmentsList(_user); //заменить на Appointments List

                CloseSideMenu();
            }
        }

        private void CallDoctorAtHome(object sender, RoutedEventArgs e)
        {
            if (!UserAuth.UserAuthorized)
            {
                MessageBox.Show("Вы не авторизованы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var calldoctor = new CallDoctorAtHome(_user);
                ContentControlPage.Content = calldoctor;

                CloseSideMenu();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShowContacts_ButtonClick(object sender, RoutedEventArgs e)
        {
            ContentControlPage.Content = new Contacts();
            CloseSideMenu();
        }

        private void MedEmployees_ButtonClick(object sender, RoutedEventArgs e)
        {
            ContentControlPage.Content = new DoctorsListInfo(_user);
            CloseSideMenu();
        }

        private void ShowAppointments_ButtonClick(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new PolycCursContext())
            {
                _doctor = dbContext.Doctors.FirstOrDefault(d => d.DoctorId == _account.DoctorId);
            }
            PatientsListForDoctor page = new PatientsListForDoctor(_doctor);
            ContentControlPage.Content = page;

            CloseSideMenu();
        }
    }
}
