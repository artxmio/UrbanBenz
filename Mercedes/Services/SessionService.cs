using Mercedes.Data.Models;

namespace Mercedes.Services;

public class SessionService : ISessionService
{
    private static readonly SessionService _instance = new();
    public static SessionService Instance => _instance;
    
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public void Login(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}