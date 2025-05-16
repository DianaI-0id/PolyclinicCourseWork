using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using System.Windows.Controls.Primitives;

namespace PolyclinicProjectKurs
{
    public partial class ChooseHourAppointment : UserControl, INotifyPropertyChanged
    {
        private User _user;
        private Doctor _doctor;
        private DateOnly _selecteddate;
        private string doctorname;
        private decimal doctorprice;
        private List<DateTime> _availableHours;

        public List<DateTime> AvailableHours
        {
            get { return _availableHours; }
            set
            {
                _availableHours = value;
                OnPropertyChanged(nameof(AvailableHours));
            }
        }

        public string ChoosenDoctorName
        {
            get { return doctorname; }
            set
            {
                doctorname = value;
                OnPropertyChanged(nameof(ChoosenDoctorName));
            }
        }

        public decimal Doctorprice
        {
            get { return doctorprice; }
            set
            {
                doctorprice = value;
                OnPropertyChanged(nameof(Doctorprice));
            }
        }

        public DateOnly ChoosenDate
        {
            get { return _selecteddate; }
            set
            {
                _selecteddate = value;
                OnPropertyChanged(nameof(ChoosenDate));
            }
        }

        public ChooseHourAppointment()
        {
            InitializeComponent();
        }

        public ChooseHourAppointment(Doctor doctor, DateOnly selectedDate, User user)
        {
            InitializeComponent();
            _doctor = doctor;
            _selecteddate = selectedDate;
            doctorname = _doctor.Doctorname;
            doctorprice = _doctor.AppointmentPrice; // Устанавливаем стоимость приема
            _user = user;

            if (_user == null)
            {
                ButtonSale.Visibility = Visibility.Collapsed;
                TextSale.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonSale.Visibility = Visibility.Visible;
                TextSale.Visibility = Visibility.Visible;
            }

            // Преобразование строк в DateTime
            TimeOnly startTimeOnly = TimeOnly.Parse(_doctor.Availabletimebefore);
            TimeOnly endTimeOnly = TimeOnly.Parse(_doctor.Availabletimeafter);
            int intervalMinutes = _doctor.Appointmentinterval ?? 30;

            DateTime startTime = _selecteddate.ToDateTime(startTimeOnly);
            DateTime endTime = _selecteddate.ToDateTime(endTimeOnly);

            // Генерация доступного времени
            var allAvailableHours = GenerateAvailableHours(startTime, endTime, intervalMinutes);

            // Получение занятых слотов
            var occupiedSlots = GetOccupiedSlots(_selecteddate);

            // Отфильтровать занятые слоты
            AvailableHours = allAvailableHours.Except(occupiedSlots).ToList();

            UpdatePriceSale();
            DataContext = this;
        }

        // Генерация временных слотов для записи
        private List<DateTime> GenerateAvailableHours(DateTime startTime, DateTime endTime, int intervalMinutes)
        {
            var availableHours = new List<DateTime>();

            for (var time = startTime; time <= endTime; time = time.AddMinutes(intervalMinutes))
            {
                availableHours.Add(time);
            }

            return availableHours;
        }

        private List<DateTime> GetOccupiedSlots(DateOnly selectedDate)
        {
            using (var context = new PolycCursContext())
            {
                // Преобразуем дату в DateTime, устанавливая время на начало дня
                var date = selectedDate.ToDateTime(TimeOnly.MinValue);

                // Получаем все записи на выбранную дату из таблицы Appointments
                var appointments = context.Appointments
                    .Where(a => a.DoctorId == _doctor.DoctorId && a.Appointmenttime == selectedDate)
                    .Select(a => a.AppointmentTime1)
                    .ToList();

                // Получаем все записи на выбранную дату из таблицы DoctorAppointmentsWithoutAuthorization
                var appointmentsWithoutAuth = context.DoctorAppointmentsWithoutAuthorization
                    .Where(a => a.DoctorId == _doctor.DoctorId && a.AppointmentDate == selectedDate)
                    .Select(a => a.AppointmentTime)
                    .ToList();

                // Преобразуем TimeOnly в DateTime и объединяем списки занятых слотов
                var occupiedSlots = appointments
                    .Where(timeOnly => timeOnly.HasValue)
                    .Select(timeOnly => date.Date.Add(new TimeSpan(timeOnly.Value.Hour, timeOnly.Value.Minute, 0)))
                    .ToList();

                occupiedSlots.AddRange(appointmentsWithoutAuth
                    .Select(timeOnly => date.Date.Add(new TimeSpan(timeOnly.Hour, timeOnly.Minute, 0)))
                    .ToList());

                return occupiedSlots;
            }
        }

        private void blockThisUserControl()
        {
            this.IsEnabled = false;
            OverlayGrid.Visibility = Visibility.Visible; // Показываем перекрывающий слой
        }

        private async void ChooseHour_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button selectedButton = (Button)sender;
            DateTime selectedTime = (DateTime)selectedButton.DataContext;

            // Формируем текст сообщения
            string message = $"Вы собираетесь записаться к врачу {_doctor.Doctorname} {_selecteddate:dd MMMM yyyy} на {selectedTime:HH:mm}. Подтвердить запись?";

            // Показываем окно подтверждения
            MessageBoxResult result = MessageBox.Show(message, "Подтверждение записи", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Обработка карты лояльности
                await UseLoyaltyCard();

                // Запись в базу данных
                using (var context = new PolycCursContext())
                {
                    var appointment = new Appointment
                    {
                        DoctorId = _doctor.DoctorId,
                        UserId = _user?.UserId,
                        Appointmenttime = _selecteddate,
                        AppointmentTime1 = TimeOnly.FromDateTime(selectedTime),
                        AppointmentStatus = "Предстоящие",
                        AppointmentPrice = doctorprice // Установка стоимости
                    };

                    if (_user == null)
                    {
                        FillUserDataPage fillUserDataPage = new FillUserDataPage();
                        if (fillUserDataPage.ShowDialog() == true)
                        {
                            blockThisUserControl();

                            var appointmentWithoutAuth = new DoctorAppointmentWithoutAuthorization
                            {
                                LastName = fillUserDataPage.LastName,
                                FirstName = fillUserDataPage.FirstName,
                                Patronymic = fillUserDataPage.Patronymic,
                                BirthDate = fillUserDataPage.BirthDate,
                                Email = fillUserDataPage.Email,
                                Phone = fillUserDataPage.Phone,
                                DoctorId = _doctor.DoctorId,
                                AppointmentDate = _selecteddate,
                                AppointmentTime = TimeOnly.FromDateTime(selectedTime),
                            };

                            context.DoctorAppointmentsWithoutAuthorization.Add(appointmentWithoutAuth);

                            var emailService = new EmailService();
                            await emailService.SendEmailAsync(appointmentWithoutAuth.Email, appointmentWithoutAuth, _user, appointmentWithoutAuth.FirstName, _doctor);
                        }
                    }
                    else
                    {
                        blockThisUserControl();

                        context.Appointments.Add(appointment);
                        var emailService = new EmailService();
                        await emailService.SendEmailAsync(_user.Useremail, appointment, _user, null, _doctor);
                    }

                    context.SaveChanges();

                    MessageBox.Show("Запись успешно добавлена и отправлена на ваш email.");

                    if (_user == null)
                    {
                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow.ContentControlPage.Content = new ServicesWithoutAuth();
                    }

                    else
                    {
                        // Переход на другую страницу после успешной записи
                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow.ContentControlPage.Content = new MainMenu(_user);
                        mainWindow.CloseSideMenu();
                    }
                }
            }
            else
            {
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow.ContentControlPage.Content = new DoctorAppointment(_user); 
                mainWindow.CloseSideMenu();
            }
        }

        //private void blockThisUsercontrol()
        //{
        //    this.IsEnabled = false;
        //}

        private async void UseLoyaltyCard_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Этот метод теперь просто вызывает асинхронный метод для обработки карты лояльности
            await UseLoyaltyCard();
        }

        private async Task UseLoyaltyCard()
        {
            if (_user != null)
            {
                using (var context = new PolycCursContext())
                {
                    // Получаем данные о карте лояльности пользователя
                    var loyaltyCard = context.LoyaltyCards
                        .FirstOrDefault(lc => lc.UserId == _user.UserId);

                    if (loyaltyCard == null)
                    {
                        //MessageBox.Show("У вас нет карты лояльности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (loyaltyCard.Points >= 10)
                    {
                        // Если у пользователя достаточно баллов для бесплатного посещения
                        MessageBoxResult result = MessageBox.Show("Вы хотите использовать 10 баллов для бесплатного посещения? Баллы будут обнулены.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            doctorprice = 0; // Бесплатное посещение
                            Doctorprice = 0;
                            loyaltyCard.Points -= 10; // Уменьшение баллов на 10
                        }
                    }

                    context.SaveChanges(); // Сохранение изменений в карте лояльности
                }
            }  
        }

        private void UpdatePriceSale()
        {
            if (_user != null)
            {
                using (var context = new PolycCursContext())
                {
                    var completedAppointmentsCount = context.Appointments
                           .Count(a => a.UserId == _user.UserId && a.AppointmentStatus == "Завершенные");

                    decimal discount = 0;
                    if (completedAppointmentsCount >= 20)
                        discount = 0.10m; // 10% скидка
                    else if (completedAppointmentsCount >= 15)
                        discount = 0.06m; // 6% скидка
                    else if (completedAppointmentsCount >= 5)
                        discount = 0.03m; // 3% скидка
                
                    doctorprice -= doctorprice * discount;
                    Doctorprice = doctorprice;
                }
            }      
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
