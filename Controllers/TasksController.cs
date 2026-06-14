using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Tracker_App.DTOs;
using Task_Tracker_App.Services;

namespace Task_Tracker_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var task = await _taskService.CreateTaskAsync(dto, userId);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("assigned-to-me")]
        public async Task<IActionResult> GetAssignedToMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tasks = await _taskService.GetAssignedToMeAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("assigned-by-me")]
        public async Task<IActionResult> GetAssignedByMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tasks = await _taskService.GetAssignedByMeAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            var assignedToMe = task.AssignedToId == userId;
            var assignedByMe = task.AssignedById == userId;
            if (!assignedToMe && !assignedByMe)
                return Forbid();

            return Ok(task);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var task = await _taskService.UpdateTaskStatusAsync(id, dto.Status, userId);
                if (task == null) return NotFound();

                return Ok(task);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
