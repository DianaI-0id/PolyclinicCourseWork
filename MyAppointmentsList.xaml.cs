using PolyclinicProjectKurs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
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
using Org.BouncyCastle.Asn1.X509;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для MyAppointmentsList.xaml
    /// </summary>
    public partial class MyAppointmentsList : UserControl
    {
        public MyAppointmentsList()
        {
            InitializeComponent();
        }

        public ObservableCollection<Appointment> AllAppointmentsCollection { get; set; }
        public ObservableCollection<Appointment> UserAppointmentsCollection { get; set; }
        private User _user;

        public MyAppointmentsList(User user)
        {
            InitializeComponent();
            _user = user;

            //UpdateAppointmentsAndLoyaltyPoints(_user);

            using (var context = new PolycCursContext())
            {
                var appointmentList = context.Appointments
                    .Include(m => m.Doctor)
                    .Where(m => m.UserId == _user.UserId)
                    .ToList();

                AllAppointmentsCollection = new ObservableCollection<Appointment>(appointmentList);
                UserAppointmentsCollection = new ObservableCollection<Appointment>(appointmentList);
            }

            DataContext = this; // Установка контекста данных на текущий UserControl
        }

        private void AllAppointments_ButtonClick(object sender, RoutedEventArgs e)
        {
            UserAppointmentsCollection.Clear();

            foreach (var appointment in AllAppointmentsCollection)
            {
                UserAppointmentsCollection.Add(appointment);
            }
        }

        private void UpcomingAppointments_ButtonClick(object sender, RoutedEventArgs e)
        {
            var filteredAppointments = AllAppointmentsCollection.Where(a => a.AppointmentStatus == "Предстоящие").ToList();
            UserAppointmentsCollection.Clear();

            foreach (var appointment in filteredAppointments)
            {
                UserAppointmentsCollection.Add(appointment);
            }
        }

        private void CompletedAppointments_ButtonClick(object sender, RoutedEventArgs e)
        {
            var filteredAppointments = AllAppointmentsCollection.Where(a => a.AppointmentStatus == "Завершенные").ToList();
            UserAppointmentsCollection.Clear();

            foreach (var appointment in filteredAppointments)
            {
                UserAppointmentsCollection.Add(appointment);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //отмена записи
        private void CancelAppointment_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент из коллекции UserAppointmentsCollection
            Appointment selectedAppointment = (sender as Button).DataContext as Appointment;

            // Удаляем выбранный элемент из базы данных
            MessageBoxResult result = MessageBox.Show("Вы уверены? Отменить это дейстие будет нельзя", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Удаляем выбранный элемент из базы данных
                using (var context = new PolycCursContext())
                {
                    context.Appointments.Remove(selectedAppointment);
                    context.SaveChanges();
                }

                // Удаляем выбранный элемент из коллекции AllAppointmentsCollection и UserAppointmentsCollection
                AllAppointmentsCollection.Remove(selectedAppointment);
                UserAppointmentsCollection.Remove(selectedAppointment);
            }
        }

        public void UpdateAppointmentsAndLoyaltyPoints(User user)
        {
            using (var context = new PolycCursContext())
            {
                var appointmentList = context.Appointments
                    .Include(m => m.Doctor)
                    .Where(m => m.UserId == user.UserId)
                    .ToList();

                var loyaltyCardData = context.LoyaltyCards
                    .FirstOrDefault(lc => lc.UserId == user.UserId);

                foreach (var appointment in appointmentList)
                {
                    if (DateOnly.FromDateTime(DateTime.Now) > appointment.Appointmenttime && TimeOnly.FromDateTime(DateTime.Now) >= appointment.AppointmentTime1)
                    {
                        // Обновление статуса на "Завершенные"
                        appointment.AppointmentStatus = "Завершенные";

                        bool recordExists = context.Medicalrecords
                               .Any(mr => mr.UserId == appointment.UserId &&
                               mr.DoctorId == appointment.DoctorId &&
                               mr.AppointmentId == appointment.Id);

                        if (!recordExists)
                        {
                            var medicalRecord = new Medicalrecord
                            {
                                UserId = appointment.UserId,
                                DoctorId = appointment.DoctorId,
                                AppointmentId = appointment.Id,
                                Diagnosis = "Не указан"
                            };

                            context.Medicalrecords.Add(medicalRecord);
                        }

                        // Начисление баллов лояльности
                        loyaltyCardData.Points++;
                    }
                }

                // Сохранение изменений в базе данных
                context.SaveChanges();
            }
        }

    }
}

namespace PolyclinicProjectKurs
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == "Предстоящие")
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
