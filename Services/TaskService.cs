using Task_Tracker_App.DTOs;
using TaskTrackerApp.Data;
using TaskTrackerApp.Models;
using TaskStatus = TaskTrackerApp.Models.TaskStatus;
using Microsoft.EntityFrameworkCore;
using AppTaskStatus = TaskTrackerApp.Models.TaskStatus;

namespace Task_Tracker_App.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public TaskService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int currentUserId)
        {
            var assignedTo = await _context.Users.FindAsync(dto.AssignedToId);
            if (assignedTo == null)
                throw new Exception("User not found");

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                AssignedById = currentUserId,
                AssignedToId = dto.AssignedToId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var createdTask = await _context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .FirstAsync(t => t.Id == task.Id);

            return MapToTaskDto(createdTask);
        }

        public async Task<List<TaskDto>> GetAssignedToMeAsync(int currentUserId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.AssignedToId == currentUserId)
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(MapToTaskDto).ToList();
        }

        public async Task<List<TaskDto>> GetAssignedByMeAsync(int currentUserId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.AssignedById == currentUserId)
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(MapToTaskDto).ToList();
        }

        public async Task<TaskDto?> UpdateTaskStatusAsync(int taskId, TaskStatus newStatus, int currentUserId)
        {
            var task = await _context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return null;

            // Authorization checks
            if (newStatus == TaskStatus.InProgress || newStatus == TaskStatus.Completed)
            {
                if (task.AssignedToId != currentUserId)
                    throw new UnauthorizedAccessException("Only assignee can update to InProgress/Completed");
            }
            else if (newStatus == TaskStatus.Approved)
            {
                if (task.AssignedById != currentUserId)
                    throw new UnauthorizedAccessException("Only assigner can approve task");
            }

            // State transition validation
            if (!IsValidStateTransition(task.Status, newStatus))
                throw new InvalidOperationException("Invalid state transition");

            task.Status = newStatus;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send notifications
            await SendStatusChangeNotification(task, newStatus);

            return MapToTaskDto(task);
        }

        private bool IsValidStateTransition(TaskStatus current, TaskStatus next)
        {
            return current switch
            {
                TaskStatus.Assigned => next == TaskStatus.InProgress || next == TaskStatus.Completed,
                TaskStatus.InProgress => next == TaskStatus.Completed,
                TaskStatus.Completed => next == TaskStatus.Approved,
                _ => false
            };
        }

        private async Task SendStatusChangeNotification(TaskItem task, TaskStatus newStatus)
        {
            if (newStatus == TaskStatus.Completed)
            {
                // Notify assigner
                await _notificationService.CreateNotificationAsync(
                    task.AssignedById,
                    $"Task '{task.Title}' has been completed by {task.AssignedTo.Username}",
                    task.Id);
            }
            else if (newStatus == TaskStatus.Approved)
            {
                // Notify assignee
                await _notificationService.CreateNotificationAsync(
                    task.AssignedToId,
                    $"Task '{task.Title}' has been approved by {task.AssignedBy.Username}",
                    task.Id);
            }
        }

        private TaskDto MapToTaskDto(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            AssignedById = task.AssignedById,
            AssignedToId = task.AssignedToId,
            AssignedByName = task.AssignedBy?.Username ?? string.Empty,
            AssignedToName = task.AssignedTo?.Username ?? string.Empty,
            CreatedAt = task.CreatedAt
        };

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await _context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            return task != null ? MapToTaskDto(task) : null;
        }
    }
}