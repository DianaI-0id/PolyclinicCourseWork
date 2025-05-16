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
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;

namespace PolyclinicProjectKurs
{
    public partial class Contacts : UserControl
    {
        public string Address { get; set; } = "192001, Россия, г.Санкт-Петербург, Воронежская улица 46-48";
        public string RegNumber { get; set; } = "+7 (812) 622-21-64";
        public string Type { get; set; } = "Частная клиника";
        public string EmailAddress { get; set; } = "ya.spb@newclinic.ru";
        public string Proezd { get; set; } = "Пешком 3 минуты от ст. м. \"Обводный канал\" ";

        private User _user;
        private Doctor _doctor;
        public Contacts(User user)
        {
            InitializeComponent();
            DataContext = this;
            _user = user;
        }

        public Contacts(Doctor doctor)
        {
            InitializeComponent();
            DataContext = this;
            _doctor = doctor;
        }

        public Contacts()
        {
            InitializeComponent();
            DataContext = this;
        }
    }

}
