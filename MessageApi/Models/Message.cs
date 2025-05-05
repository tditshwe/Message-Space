using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    public class MessageRetrieve
    {
        public string? Sender { get; set; }
        public string? SenderName { get; set; }
        public DateTime DateSent { get; set; }
        public string? Text { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime DateSent { get; set; }

        [ForeignKey("Account")]
        public string? SenderUsername { get; set; }
        public virtual Account? Sender { get; set; }
        public virtual Chat? Chat { get; set; }
        public virtual IList<AccountMessage>? AccountMessages { get; set; }
        public virtual IList<GroupMessage>? GroupMessages { get; set; }
    }
}