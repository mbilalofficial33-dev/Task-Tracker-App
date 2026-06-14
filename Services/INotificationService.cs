using TaskTrackerApp.Dtos;

namespace Task_Tracker_App.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message, int? taskId = null);
        Task<List<NotificationDto>> GetNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
    }
}