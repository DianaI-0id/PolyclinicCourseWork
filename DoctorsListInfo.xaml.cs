using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для DoctorsListInfo.xaml
    /// </summary>
    public partial class DoctorsListInfo : UserControl
    {
        private User _user;
        public DoctorsListInfo(User user)
        {
            InitializeComponent();
            LoadDoctors();
            _user = user;
        }

        private void LoadDoctors()
        {
            using (var context = new PolycCursContext())
            {
                var doctors = context.Doctors
                    .Select(d => new Doctor
                    {
                        Doctorname = d.Doctorname,
                        Speciality = d.Speciality,
                        Availabletimebefore = d.Availabletimebefore,
                        Availabletimeafter = d.Availabletimeafter,
                        AppointmentPrice = d.AppointmentPrice                      
                    })
                    .ToList();

                DoctorsListBox.ItemsSource = doctors;
            }
        }
    }
}
