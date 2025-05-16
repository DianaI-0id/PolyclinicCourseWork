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
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using Microsoft.EntityFrameworkCore;

namespace PolyclinicProjectKurs
{
    public partial class MyLoyaltyCard : UserControl
    {
        private User _user;
        public MyLoyaltyCard()
        {
            InitializeComponent();
        }

        public MyLoyaltyCard(User user)
        {
            InitializeComponent();
            _user = user;
            UpdateAppointmentsAndLoyaltyPoints(_user);

            LoadData();
        }

        //private void LoadData()
        //{
        //    using (var context = new PolycCursContext())
        //    {
        //        var loyaltyCardData = context.LoyaltyCardViews
        //            .FirstOrDefault(lc => lc.user_id == _user.UserId);

        //        var loaltycard = context.LoyaltyCards
        //            .FirstOrDefault(lc => lc.UserId == _user.UserId);

        //        if (loaltycard == null)
        //        {
        //            return;
        //        }

        //        if (loyaltyCardData != null)
        //        {
        //            var completedAppointments = context.Appointments
        //                .Where(a => a.UserId == _user.UserId && a.AppointmentStatus == "Завершенные")
        //                .ToList();

        //            DataContext = new
        //            {
        //                card_number = loyaltyCardData.card_number,
        //                card_sale = loyaltyCardData.card_sale,
        //                appointment_count = completedAppointments.Count,
        //                points = loaltycard.Points
        //            };
        //        }
        //        else
        //        {
        //            DataContext = new
        //            {
        //                card_number = loaltycard.CardNumber,
        //                card_sale = 0,
        //                appointment_count = 0,
        //                points = 0
        //            };
        //        }
        //    }
        //}

        private void LoadData()
        {
            using (var context = new PolycCursContext())
            {
                var loyaltyCardData = context.LoyaltyCardViews
                    .FirstOrDefault(lc => lc.user_id == _user.UserId);

                var loaltycard = context.LoyaltyCards
                    .FirstOrDefault(lc => lc.UserId == _user.UserId);

                if (loaltycard == null)
                {
                    return;
                }

                if (loyaltyCardData != null)
                {
                    var completedAppointments = context.Appointments
                        .Where(a => a.UserId == _user.UserId && a.AppointmentStatus == "Завершенные")
                        .ToList();

                    DataContext = new
                    {
                        card_number = loyaltyCardData.card_number,
                        card_sale = loyaltyCardData.card_sale,
                        appointment_count = completedAppointments.Count,
                        points = loaltycard.Points
                    };
                }
                else
                {
                    DataContext = new
                    {
                        card_number = loaltycard.CardNumber,
                        card_sale = 0,
                        appointment_count = 0,
                        points = 0
                    };
                }
            }
        }

        private void UpdateAppointmentsAndLoyaltyPoints(User user)
        {
            using (var context = new PolycCursContext())
            {
                var appointmentList = context.Appointments
                    .Include(m => m.Doctor)
                    .Where(m => m.UserId == user.UserId)
                    .ToList();

                var loyaltyCardData = context.LoyaltyCards
                    .FirstOrDefault(lc => lc.UserId == user.UserId);

                if (loyaltyCardData == null) return;

                foreach (var appointment in appointmentList)
                {
                    if (DateOnly.FromDateTime(DateTime.Now) > appointment.Appointmenttime && TimeOnly.FromDateTime(DateTime.Now) >= appointment.AppointmentTime1)
                    {
                        // Обновление статуса на "Завершенные"
                        appointment.AppointmentStatus = "Завершенные";

                        // Проверяем, существует ли уже медицинская запись для этого посещения
                        bool recordExists = context.Medicalrecords
                               .Any(mr => mr.UserId == appointment.UserId &&
                               mr.DoctorId == appointment.DoctorId &&
                               mr.AppointmentId == appointment.Id);

                        // Если медицинской записи нет, создаем новую
                        if (!recordExists)
                        {
                            var medicalRecord = new Medicalrecord
                            {
                                UserId = appointment.UserId,
                                DoctorId = appointment.DoctorId,
                                AppointmentId = appointment.Id,
                                Diagnosis = "Не указан"
                            };

                            context.Medicalrecords.Add(medicalRecord);

                            // Начисляем баллы лояльности только один раз для каждого завершенного посещения
                            loyaltyCardData.Points++;
                        }
                    }
                }

                // Сохранение изменений в базе данных
                context.SaveChanges();
            }
        }
    }
}
