using Microsoft.EntityFrameworkCore;
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PolyclinicProjectKurs
{
    public partial class CancelAppointmentPage : UserControl
    {
        private User user = null;
        public CancelAppointmentPage()
        {
            InitializeComponent();
        }

        private void FindAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastNameTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string phone = PhoneTextBox.Text;
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            using (var context = new PolycCursContext())
            {
                // Очистка списка записей
                AppointmentsListBox.Items.Clear();

                // Поиск записей без авторизации
                var appointmentsWithoutAuth = context.DoctorAppointmentsWithoutAuthorization
                    .Include(a => a.Doctor)
                    .Where(a => a.LastName == lastName && a.FirstName == firstName && a.Patronymic == patronymic && a.Phone == phone && a.AppointmentDate >= currentDate)
                    .ToList();

                if (appointmentsWithoutAuth.Count > 0)
                {
                    foreach (var appointment in appointmentsWithoutAuth)
                    {
                        string appointmentInfo = $"Дата записи: {appointment.AppointmentDate}\n" +
                                                 $"Имя врача: {appointment.Doctor.Doctorname}\n" +
                                                 $"Время приема: {appointment.AppointmentTime}\n";
                        ListBoxItem item = new ListBoxItem
                        {
                            Content = appointmentInfo,
                            Tag = appointment
                        };
                        item.MouseDoubleClick += CancelAppointmentWithoutAuth;
                        AppointmentsListBox.Items.Add(item);
                    }
                }

                // Поиск пользователя в таблице Users
                user = context.Users
                    .FirstOrDefault(u => u.Usersurname == lastName &&
                                         u.Username == firstName &&
                                         u.Userpatronymicname == patronymic &&
                                         u.Userphone == phone);

                if (user != null)
                {
                    // Поиск всех записей в Appointments по UserId и указанной дате
                    var appointmentsWithAuth = context.Appointments
                        .Include(a => a.Doctor)  // Включаем связанного доктора
                        .Where(a => a.UserId == user.UserId && a.Appointmenttime >= currentDate)
                        .ToList();

                    foreach (var appointment in appointmentsWithAuth)
                    {
                        string appointmentInfo = $"Дата записи: {appointment.Appointmenttime}\n" +
                                                 $"Имя врача: {appointment.Doctor.Doctorname}\n" +
                                                 $"Время приема: {appointment.AppointmentTime1}\n";
                        ListBoxItem item = new ListBoxItem
                        {
                            Content = appointmentInfo,
                            Tag = appointment
                        };
                        item.MouseDoubleClick += CancelAppointmentWithAuth;
                        AppointmentsListBox.Items.Add(item);
                    }
                }

                if (AppointmentsListBox.Items.Count == 0)
                {
                    MessageBox.Show("Записи не найдены.");
                }
            }
        }

        private void CancelAppointmentWithoutAuth(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            var appointment = item.Tag as DoctorAppointmentWithoutAuthorization;

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите отменить запись на {appointment.AppointmentDate} к {appointment.Doctor.Doctorname}?", "Подтверждение отмены", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PolycCursContext())
                {
                    context.DoctorAppointmentsWithoutAuthorization.Remove(appointment);
                    context.SaveChanges();
                    AppointmentsListBox.Items.Remove(item);
                    MessageBox.Show("Запись успешно отменена.");

                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow.ContentControlPage.Content = new ServicesWithoutAuth();
                }
            }
        }

        private void CancelAppointmentWithAuth(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            var appointment = item.Tag as Appointment;

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите отменить запись на {appointment.Appointmenttime} к {appointment.Doctor.Doctorname}?", "Подтверждение отмены", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PolycCursContext())
                {
                    // Удаление связанных записей из Medicalrecords
                    var relatedMedicalRecords = context.Medicalrecords.Where(m => m.AppointmentId == appointment.Id).ToList();
                    context.Medicalrecords.RemoveRange(relatedMedicalRecords);

                    // Удаление записи из Appointments
                    context.Appointments.Remove(appointment);
                    context.SaveChanges();
                    AppointmentsListBox.Items.Remove(item);
                    MessageBox.Show("Запись успешно отменена.");

                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow.ContentControlPage.Content = new DoctorAppointment(user);
                }
            }
        }
    }
}
