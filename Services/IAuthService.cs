using Task_Tracker_App.DTOs;
using TaskTrackerApp.Models;

namespace TaskTrackerApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterDto dto);
        Task<AuthResponse?> LoginAsync(LoginDto dto);
        Task<User?> GetUserByIdAsync(int id);
    }
}