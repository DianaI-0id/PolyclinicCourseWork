using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using System.Windows;

namespace PolyclinicProjectKurs
{
    public partial class ChangeDiagnosisPage : Window
    {
        private readonly Medicalrecord _medicalRecord;
        public Medicalrecord UpdatedMedicalRecord { get; private set; }

        public ChangeDiagnosisPage()
        {
            InitializeComponent();
        }

        public ChangeDiagnosisPage(Medicalrecord medicalRecord)
        {
            InitializeComponent();
            _medicalRecord = medicalRecord;
            DiagnosisTextBox.Text = _medicalRecord.Diagnosis;
            ComplaintsTextBox.Text = _medicalRecord.Complaints;
            RegimenTextBox.Text = _medicalRecord.TreatmentRegimen;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Получить новый диагноз и жалобы из TextBox
            string newDiagnosis = DiagnosisTextBox.Text;
            string newComplaints = ComplaintsTextBox.Text;
            string newRegimen = RegimenTextBox.Text;

            if (!string.IsNullOrWhiteSpace(newDiagnosis) && !string.IsNullOrWhiteSpace(newComplaints) && !string.IsNullOrWhiteSpace(newRegimen))
            {
                // Обновить диагноз и жалобы в базе данных
                using (var context = new PolycCursContext())
                {
                    var record = context.Medicalrecords.FirstOrDefault(m => m.Id == _medicalRecord.Id);
                    if (record != null)
                    {
                        record.Diagnosis = newDiagnosis;
                        record.Complaints = newComplaints;
                        record.TreatmentRegimen = newRegimen;
                        context.SaveChanges();
                        MessageBox.Show("Данные успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        UpdatedMedicalRecord = record; // Обновление свойства
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти запись в базе данных.", "Успех", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Закрыть окно с результатом
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите диагноз и жалобы перед подтверждением.");
            }
        }
    }
}