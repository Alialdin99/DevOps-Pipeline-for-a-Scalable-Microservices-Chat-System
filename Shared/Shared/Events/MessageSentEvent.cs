using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class MessageSentEvent
    {

        public string MessageId { get; init; }
        public string ConversationId { get; init; }
        public string SenderId { get; init; }
        public string ReceiverId { get; init; }
        public string Content { get; init; }
        public DateTime SentAt { get; init; }

    }
}
