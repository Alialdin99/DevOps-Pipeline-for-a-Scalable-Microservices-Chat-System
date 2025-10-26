﻿namespace AuthenticationService.Messaging.Events
{
    public class UserRegistered
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
