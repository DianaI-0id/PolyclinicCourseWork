using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;

namespace PolyclinicProjectKurs
{
    public partial class MedicalRecords : UserControl
    {
        public ObservableCollection<Medicalrecord> MedicalRecordsCollection { get; set; }
        private User _user;

        public MedicalRecords()
        {
            InitializeComponent();
        }

        public MedicalRecords(User user)
        {
            InitializeComponent();
            _user = user;

            using (var context = new PolycCursContext())
            {
                var medicalRecordsList = context.Medicalrecords
                    .Include(m => m.Appointment)
                    .Include(m => m.Doctor)
                    .Where(m => m.UserId == _user.UserId) // Фильтр по UserId текущего пользователя
                    .ToList();

                MedicalRecordsCollection = new ObservableCollection<Medicalrecord>(medicalRecordsList);
                //выводим из базы данных записи текущего пользователя
            }

            DataContext = this;
        }
    }
}
