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
using System.Windows.Shapes;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для FillUserDataPage.xaml
    /// </summary>
    public partial class FillUserDataPage : Window
    {
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string Patronymic { get; private set; }
        public DateOnly BirthDate { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public FillUserDataPage()
        {
            InitializeComponent();
        }

        private void ApplyData_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Проверка, что все текстовые поля заполнены и дата выбрана
            if (string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                string.IsNullOrEmpty(PatronymicTextBox.Text) ||
                string.IsNullOrEmpty(EmailTextBox.Text) ||
                string.IsNullOrEmpty(PhoneTextBox.Text) ||
                BirthDatePicker.SelectedDate == null) // Проверка на дату
            {
                MessageBox.Show("Не все поля заполнены или дата не указана", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Присвоение значений полям
                LastName = LastNameTextBox.Text;
                FirstName = FirstNameTextBox.Text;
                Patronymic = PatronymicTextBox.Text;
                BirthDate = DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value);
                Email = EmailTextBox.Text;
                Phone = PhoneTextBox.Text;

                // Закрытие диалога с результатом
                DialogResult = true;
                this.Close();
            }
        }

    }
}
