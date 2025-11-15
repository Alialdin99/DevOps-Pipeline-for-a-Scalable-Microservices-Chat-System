using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationRepository _repository;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(NotificationRepository repository, ILogger<NotificationsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] string? receiverId)
        {
            try
            {
                if (string.IsNullOrEmpty(receiverId))
                {
                    // If no receiverId provided, return empty list or all notifications
                    // For now, return empty list as we need receiverId to filter
                    return Ok(new List<Notification>());
                }

                var notifications = await _repository.GetByReceiverIdAsync(receiverId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for receiverId: {ReceiverId}", receiverId);
                return StatusCode(500, new { message = "An error occurred while retrieving notifications." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(string id)
        {
            try
            {
                var notification = await _repository.GetByIdAsync(id);
                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found." });
                }
                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the notification." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Notification not found." });
                }
                return Ok(new { message = "Notification deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the notification." });
            }
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            try
            {
                var updated = await _repository.MarkAsReadAsync(id);
                if (!updated)
                {
                    return NotFound(new { message = "Notification not found." });
                }
                return Ok(new { message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the notification." });
            }
        }
    }
}

