namespace ChattingService.DTOs
{
    public record SendMessageRequest(string SenderId, string ReceiverId, string Content);

}
