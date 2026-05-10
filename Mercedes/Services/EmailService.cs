using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Mercedes.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService()
    {
        _smtpHost = "smtp.yandex.ru";
        _smtpPort = 587;
        _smtpPassword = "xrrsltpouznxtmvc";
        _fromEmail = "melodyhub@yandex.com";
        _fromName = "Mercedes";
    }

    public async Task<bool> SendTestDriveConfirmationAsync(string toEmail, string userName, string carModel, DateTime testDriveDate)
    {
        try
        {
            var subject = "Подтверждение записи на тест-драйв";
            var body = $@"
                Уважаемый(ая) {userName}!

                Ваша запись на тест-драйв успешно оформлена.

                Автомобиль: {carModel}
                Дата: {testDriveDate:dd.MM.yyyy HH:mm}
                Время: {testDriveDate:HH:mm}

                Пожалуйста, приезжайте в наш автосалон за 10 минут до назначенного времени.
                Не забудьте взять с собой водительское удостоверение.

                С уважением,
                Mercedes
                ";

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_fromEmail, _smtpPassword),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка отправки email: {ex.Message}");
            return false;
        }
    }
}