using ChattingService.DTOs;
using ChattingService.Hubs;
using ChattingService.Hubs;
using ChattingService.Models;
using ChattingService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChattingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(ChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var message = await _chatService.SendMessage(request.SenderId, request.ReceiverId, request.Content);

            await _hubContext.Clients.User(request.ReceiverId)
                .SendAsync("ReceiveMessage", request.SenderId, request.Content, message.SentAt);

            return Ok(message);
        }

        [HttpGet("GetAllConvos/{userId}")]
        public async Task<IActionResult> GetConversationsOfUser(string userID)
        {
            List<Conversation> conversations = await _chatService.GetConversationsOfUserAsync(userID);

            if (conversations == null || !conversations.Any())
                return NotFound("User doesn't have any conversations yet");

            return Ok(conversations);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetConversationMessagesAsync(string conversationId)
        {
            var messages = await _chatService.GetConversationMessagesAsync(conversationId);
            return Ok(messages);
        }
    }
}
