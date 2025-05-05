using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MessageApi.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int LastMessageId { get; set; }
        public string? ReceiverUsername { get; set; }
        public string? SenderUsername { get; set; }
        public virtual Account? Sender { get; set; }
        public virtual Account? Receiver { get; set; }
        public virtual Message? LastMessage { get; set; }
    }
}