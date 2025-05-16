using System;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using System.Windows;
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using MailKit.Security;

namespace PolyclinicProjectKurs
{
    public partial class ForgotPasswordPage : Window
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        private void SendNewPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;

            using (var context = new PolycCursContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Useremail == email);
                if (user != null)
                {
                    string newPassword = GenerateRandomPassword();
                    user.Userpassword = newPassword;

                    // Обновляем пароль в базе данных
                    context.Users.Update(user);
                    context.SaveChanges();

                    try
                    {
                        SendPasswordEmail(email, newPassword); // Отправляем новый пароль на электронную почту
                        MessageBox.Show("Новый пароль отправлен на ваш email.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        AuthorizationPage page = new AuthorizationPage();
                        page.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось отправить email: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с таким email не найден.");
                }
            }
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async void SendPasswordEmail(string email, string newPassword)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Восстановление пароля", "polyclinic530@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Ваш новый пароль";
            message.Body = new TextPart("plain")
            {
                Text = $"Ваш новый пароль: {newPassword}. В личном кабинете вы сможете изменить его на более удобный."
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    // Подключение к серверу
                    await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    // Аутентификация
                    await client.AuthenticateAsync("polyclinic530@gmail.com", "mpdo qwyn xhxy fwhj");

                    // Отправка сообщения
                    await client.SendAsync(message);

                    MessageBox.Show("Новый пароль успешно отправлен на ваш email.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось отправить email: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // Отключение клиента
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
