namespace Mercedes.Services;

public interface IEmailService
{
    Task<bool> SendTestDriveConfirmationAsync(string toEmail, string userName, string carModel, DateTime testDriveDate);
}