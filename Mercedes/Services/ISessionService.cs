using Mercedes.Data.Models;

namespace Mercedes.Services;

public interface ISessionService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    void Login(User user);
    void Logout();
}