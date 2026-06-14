using Task_Tracker_App.DTOs;
using AppTaskStatus = TaskTrackerApp.Models.TaskStatus;

namespace Task_Tracker_App.Services
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int currentUserId);

        System.Threading.Tasks.Task<List<TaskDto>> GetAssignedToMeAsync(int currentUserId);

        System.Threading.Tasks.Task<List<TaskDto>> GetAssignedByMeAsync(int currentUserId);

        System.Threading.Tasks.Task<TaskDto?> UpdateTaskStatusAsync(int taskId, AppTaskStatus status, int currentUserId);

        System.Threading.Tasks.Task<TaskDto?> GetTaskByIdAsync(int taskId);
    }
}