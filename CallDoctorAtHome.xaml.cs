using System;
using System.Collections.Generic;
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
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using Microsoft.EntityFrameworkCore;

namespace PolyclinicProjectKurs
{
    public partial class CallDoctorAtHome : UserControl, INotifyPropertyChanged
    {
        private DateOnly selecteddate;
        private User _user;

        public CallDoctorAtHome()
        {
            InitializeComponent();
        }

        public CallDoctorAtHome(User user)
        {
            InitializeComponent();
            _user = user;
            DataContext = _user;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private void OstavitZayavku_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(surnametext.Text) && !string.IsNullOrEmpty(nametext.Text) && !string.IsNullOrEmpty(patronymictext.Text)  &&
                !string.IsNullOrEmpty(emailtext.Text) && !string.IsNullOrEmpty(phonetext.Text) && (MaleRadioButton.IsChecked == true || FemaleRadioButton.IsChecked == true) &&
                !string.IsNullOrEmpty(snilstext.Text) && !string.IsNullOrEmpty(citytext.Text) && !string.IsNullOrEmpty(streettext.Text) && 
                !string.IsNullOrEmpty(housenumbertext.Text) && !string.IsNullOrEmpty(flatnumbertext.Text) && !string.IsNullOrEmpty(callreasontext.Text) &&
                appointmdate.SelectedDate!= null)
            {
                selecteddate = DateOnly.FromDateTime(appointmdate.SelectedDate.Value);
                if (selecteddate < DateOnly.FromDateTime(DateTime.Now))
                {
                    MessageBox.Show("Введите корректную дату");
                    appointmdate.SelectedDate = null;
                }
                else
                {
                    using (var dbContext = new PolycCursContext())
                    {
                        var user = dbContext.Users.FirstOrDefault(u => u.Useremail == emailtext.Text);

                        Homedoctorcall newCall = new Homedoctorcall
                        {
                            UserId = user.UserId,
                            LivingPlace = citytext.Text,
                            Street = streettext.Text,
                            Housenumber = int.Parse(housenumbertext.Text),
                            Flatnumber = int.Parse(flatnumbertext.Text),
                            Korpus = korpustext.Text,
                            Appointmentdate = selecteddate,
                            Callreason = callreasontext.Text
                        };

                        dbContext.Homedoctorcalls.Add(newCall);
                        dbContext.SaveChanges(); // Сохранение изменений в базе данных

                        MessageBox.Show("Заявка на вызов врача на дом успешно оставлена!");

                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow.ContentControlPage.Content = new MainMenu(_user);

                        mainWindow.CloseSideMenu();
                    }
                }
            }

            else
            {
                // Какое-то из полей пустое
                MessageBox.Show("Не все поля заполнены");
            }
        }
    }
}
