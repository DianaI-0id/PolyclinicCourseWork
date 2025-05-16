using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PolyclinicProjectKurs
{
    public partial class DoctorAppointmentsPage : UserControl
    {
        public ObservableCollection<Appointment> DoctorAppointmentsCollection { get; set; }
        public User User { get; set; } // Объявляем свойство User
        private Doctor _doctor;

        public DoctorAppointmentsPage(User patient, Doctor doctor)
        {
            InitializeComponent();
            User = patient; // Инициализация свойства User
            _doctor = doctor;
            LoadAppointments();
            DataContext = this;
        }

        private void LoadAppointments()
        {
            using (var context = new PolycCursContext())
            {
                // Загружаем все завершенные записи для указанного врача
                var appointmentList = context.Appointments
                    .Include(a => a.User)
                    .Where(a => a.DoctorId == _doctor.DoctorId && a.AppointmentStatus == "Завершенные" &&
                        a.UserId == User.UserId)
                    .ToList();

                if (appointmentList.Count == 0)
                {
                    MessageBox.Show("Нет доступных посещений для отображения", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    // Для каждой записи загружаем соответствующую медицинскую запись
                    foreach (var appointment in appointmentList)
                    {
                        var medicalRecord = context.Medicalrecords
                            .FirstOrDefault(m => m.AppointmentId == appointment.Id && m.DoctorId == _doctor.DoctorId);

                        if (medicalRecord != null)
                        {
                            appointment.Medicalrecords = new List<Medicalrecord> { medicalRecord };
                        }
                        else
                        {
                            appointment.Medicalrecords = new List<Medicalrecord>();
                        }
                    }

                    // Создаем коллекцию для отображения
                    DoctorAppointmentsCollection = new ObservableCollection<Appointment>(appointmentList);
                }
            }
        }


        private void ChangeDiagnosisButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент из коллекции
            var button = sender as Button;
            var selectedAppointment = button?.DataContext as Appointment;

            // Если у выбранного приема есть медицинские записи, открываем окно для изменения диагноза
            if (selectedAppointment != null && selectedAppointment.Medicalrecords.Any())
            {
                var medicalRecord = selectedAppointment.Medicalrecords.First();
                ChangeDiagnosisPage changeDiagnosisPage = new ChangeDiagnosisPage(medicalRecord);
                if (changeDiagnosisPage.ShowDialog() == true)
                {
                    // Обновить диагноз в базе данных и коллекции после закрытия окна
                    using (var context = new PolycCursContext())
                    {
                        var record = context.Medicalrecords.FirstOrDefault(m => m.Id == medicalRecord.Id);
                        if (record != null)
                        {
                            record.Diagnosis = changeDiagnosisPage.UpdatedMedicalRecord.Diagnosis;
                            record.Complaints = changeDiagnosisPage.UpdatedMedicalRecord.Complaints;
                            record.TreatmentRegimen = changeDiagnosisPage.UpdatedMedicalRecord.TreatmentRegimen;
                            context.SaveChanges();

                            // Обновить коллекцию и UI
                            var updatedRecord = record;
                            UpdateMedicalRecordInCollection(selectedAppointment, updatedRecord);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти запись в базе данных.");
                        }
                    }
                }
            }
        }

        private void UpdateMedicalRecordInCollection(Appointment appointment, Medicalrecord updatedRecord)
        {
            // Поиск медицинской записи в коллекции
            var record = appointment.Medicalrecords.FirstOrDefault(m => m.Id == updatedRecord.Id);
            if (record != null)
            {
                record.Diagnosis = updatedRecord.Diagnosis;
                record.Complaints = updatedRecord.Complaints;
                record.TreatmentRegimen = updatedRecord.TreatmentRegimen;

                // Обновление привязки данных
                var index = DoctorAppointmentsCollection.IndexOf(appointment);
                DoctorAppointmentsCollection[index] = new Appointment
                {
                    // Заполните свойства новой Appointment, чтобы отобразить обновления
                    Id = appointment.Id,
                    Appointmenttime = appointment.Appointmenttime,
                    Medicalrecords = new List<Medicalrecord> { updatedRecord }
                };
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            // Создаем новый экземпляр PatientsListForDoctor с тем же врачом
            PatientsListForDoctor patientsListPage = new PatientsListForDoctor(_doctor);
            mainWindow.ContentControlPage.Content = patientsListPage;
        }
    }
}
