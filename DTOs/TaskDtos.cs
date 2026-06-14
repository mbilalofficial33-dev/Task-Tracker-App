namespace Task_Tracker_App.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int AssignedToId { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskTrackerApp.Models.TaskStatus Status { get; set; }
        public int AssignedById { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedByName { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public TaskTrackerApp.Models.TaskStatus Status { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
