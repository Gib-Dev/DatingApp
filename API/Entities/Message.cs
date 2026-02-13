namespace API.Entities;

public class Message
{
    public int Id { get; set; }
    public required string SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;
    public required string RecipientId { get; set; }
    public AppUser Recipient { get; set; } = null!;
    public required string Content { get; set; }
    public DateTime DateSent { get; set; } = DateTime.UtcNow;
    public DateTime? DateRead { get; set; }
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
}
