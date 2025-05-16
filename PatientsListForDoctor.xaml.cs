using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using Microsoft.EntityFrameworkCore;

namespace PolyclinicProjectKurs
{
    public partial class PatientsListForDoctor : UserControl
    {
        private Doctor _doctor;

        public PatientsListForDoctor(Doctor doctor)
        {
            InitializeComponent();
            _doctor = doctor;
            LoadPatients();
        }

        private void LoadPatients()
        {
            using (var context = new PolycCursContext())
            {
                // Получаем всех уникальных пользователей, у которых есть записи у данного врача
                var patients = context.Appointments
                                      .Where(a => a.DoctorId == _doctor.DoctorId)
                                      .Select(a => a.User)
                                      .Distinct()
                                      .ToList();

                // Создаем список данных для отображения
                var patientData = patients.Select(p => new
                {
                    FullName = $"{p.Usersurname} {p.Username} {p.Userpatronymicname}",
                    FullAddress = $"{p.Userstreet}, дом {p.Userhome}, кв.{p.Userflat}",
                    p.Birthdate,
                    p.Snils,
                    UserId = p.UserId // Добавляем UserId для использования в кнопке
                }).ToList();

                // Устанавливаем источник данных для ListBox
                PatientsListBox.ItemsSource = patientData;
            }
        }


        private void ViewAppointmentsSelectedPatient_ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // Получаем текущий объект пациента из DataContext кнопки
            var selectedPatient = button?.DataContext as dynamic;

            if (selectedPatient != null)
            {
                int userId = selectedPatient.UserId;

                using (var context = new PolycCursContext())
                {
                    // Находим пациента в базе данных по UserId
                    var patient = context.Users
                        .Include(u => u.Appointments)
                        .FirstOrDefault(u => u.UserId == userId);

                    if (patient != null)
                    {
                        // Создаем страницу для отображения посещений и передаем в нее пациента и врача
                        var appointmentsPage = new DoctorAppointmentsPage(patient, _doctor);

                        // Переходим на страницу с посещениями
                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow.ContentControlPage.Content = appointmentsPage;
                    }
                    else
                    {
                        MessageBox.Show("Пациент не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

    }
}
