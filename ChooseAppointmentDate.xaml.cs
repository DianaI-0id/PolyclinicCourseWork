using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для ChooseAppointmentDate.xaml
    /// </summary>
    public partial class ChooseAppointmentDate : UserControl, INotifyPropertyChanged
    {
        private User _user;
        private string _choosenDoctorname;
        private Doctor _doctor;
        private ObservableCollection<DateOnly> _availableDates;

        public string ChoosenDoctorName
        {
            get { return _choosenDoctorname; }
            set
            {
                _choosenDoctorname = value;
                OnPropertyChanged(nameof(ChoosenDoctorName));
            }
        }

        public ObservableCollection<DateOnly> AvailableDates
        {
            get { return _availableDates; }
            set
            {
                _availableDates = value;
                OnPropertyChanged(nameof(AvailableDates));
            }
        }


        public ChooseAppointmentDate()
        {
            InitializeComponent();
        }

        public ChooseAppointmentDate(Doctor doctor, User user)
        {
            InitializeComponent();
            _doctor = doctor;
            ChoosenDoctorName = _doctor.Doctorname;
            LoadAvailableDates();
            DataContext = this;
            _user = user;
        }

        private void LoadAvailableDates()
        {
            using (var context = new PolycCursContext())
            {
                var dates = context.DoctorAppointmentDates
                    .Where(d => d.DoctorId == _doctor.DoctorId)
                    .Select(d => d.AvailableDate)
                    .ToList();

                AvailableDates = new ObservableCollection<DateOnly>(dates);
            }
        }


        private void ChooseDate_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button choosedate = (Button) sender;
            DateOnly date = (DateOnly)choosedate.DataContext;

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new ChooseHourAppointment(_doctor, date, _user);

            mainWindow.CloseSideMenu();

        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Back_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ContentControlPage.Content = new DoctorAppointment(_user);
        }
    }
}
