using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTrackerApp.Models
{
    public enum TaskStatus
    {
        Assigned,
        InProgress,
        Completed,
        Approved
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Assigned;
        public int AssignedById { get; set; }
        public User AssignedBy { get; set; } = null!;
        public int AssignedToId { get; set; }
        public User AssignedTo { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}